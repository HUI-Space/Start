using System;
using System.Net;
using System.Net.Sockets;


namespace Start
{
    public class UdpClient : IClient
    {
        public uint Id { get; private set; }
        public IPEndPoint EndPoint { get; private set; }
        public bool IsDisposed { get; private set; }
        public bool IsConnected { get; private set; }
        public long BytesSent { get; private set; }
        public long BytesPending { get; private set; }
        public long BytesSending { get; private set; }
        public long BytesReceived { get; private set; }
        public int OptionReceiveBufferLimit { get; set; } = 0;
        public int OptionReceiveBufferSize { get; set; } = 8192;
        public int OptionSendBufferLimit { get; set; } = 0;
        public int OptionSendBufferSize { get; set; } = 8192;
        /// <summary>
        /// 客户端发送的数据报数
        /// </summary>
        public long DatagramsSent { get; private set; }
        /// <summary>
        /// 客户端接收到的数据报数
        /// </summary>
        public long DatagramsReceived { get; private set; }
        
        public event Action OnConnecting;
        public event Action OnConnected;
        public event Action<byte[], long, long> OnReceived;
        public event Action OnDisconnecting;
        public event Action OnDisconnected;
        public event Action<SocketError> OnError;
        
        private bool _sending;
        private bool _receiving;
        private bool _socketDisposed;
        
        private Socket _socket;
        private Buffer _sendBuffer;
        private Buffer _receiveBuffer;
        private EndPoint _sendEndpoint;
        private EndPoint _receiveEndpoint;
        private SocketAsyncEventArgs _sendEventArg;
        private SocketAsyncEventArgs _receiveEventArg;
        
        public UdpClient(uint id,IPEndPoint endPoint)
        {
            Id = id;
            EndPoint = endPoint;
        }
        
        public bool ConnectAsync()
        {
            if (IsConnected && !_socketDisposed)
            {
                return true;
            }

            if (_socketDisposed)
            {
                return false;
            }

            _sendBuffer = new Buffer();
            _receiveBuffer = new Buffer();
            _sendEventArg = new SocketAsyncEventArgs();
            _receiveEventArg = new SocketAsyncEventArgs();
            
            _sendEventArg.Completed += OnAsyncCompleted;
            _receiveEventArg.Completed += OnAsyncCompleted;
            
            _sendEndpoint = EndPoint;
            _receiveEndpoint = new IPEndPoint((EndPoint.AddressFamily == AddressFamily.InterNetworkV6) ? IPAddress.IPv6Any : IPAddress.Any, 0);
            _socket = new Socket(EndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            _socketDisposed = false;
            
            OnConnecting?.Invoke();

            try
            {
                _socket.Bind(_receiveEndpoint);
            }
            catch (SocketException ex)
            {
                SendError(ex.SocketErrorCode);
                
                _sendEventArg.Completed -= OnAsyncCompleted;
                _receiveEventArg.Completed -= OnAsyncCompleted;
                
                OnDisconnecting?.Invoke();
                
                _socket.Close();
                _socket.Dispose();
                _sendEventArg.Dispose();
                _receiveEventArg.Dispose();
                
                OnDisconnected?.Invoke();

                return false;
            }
            
            _receiveBuffer.Reserve(OptionReceiveBufferSize);
            
            BytesSent = 0;
            BytesPending = 0;
            BytesSending = 0;
            BytesReceived = 0;
            DatagramsSent = 0;
            DatagramsReceived = 0;
            
            IsConnected = true;

            OnConnected?.Invoke();
            TryReceive();
            return true;
        }

        public bool SendAsync(byte[] buffer)
        {
            if (_sending || !IsConnected)
            {
                return false;
            }
            
            if (_sendBuffer.Size + buffer.Length > OptionSendBufferLimit && OptionSendBufferLimit > 0)
            {
                SendError(SocketError.NoBufferSpaceAvailable);
                return false;
            }
            
            _sendBuffer.Append(buffer);
            BytesSending = _sendBuffer.Size;
            
            TrySend();

            return true;
        }
        
        public bool ReconnectAsync()
        {
            if (!Disconnect())
                return false;

            return ConnectAsync();
        }
        
        public bool Disconnect()
        {
            if (!IsConnected)
                return false;
            
            _sendEventArg.Completed -= OnAsyncCompleted;
            _receiveEventArg.Completed -= OnAsyncCompleted;
            
            OnDisconnecting?.Invoke();
            
            try
            {
                _socket.Close();
                _socket.Dispose();
                _sendEventArg.Dispose();
                _receiveEventArg.Dispose();
                _socketDisposed = true;
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
            }
            
            _sending = false;
            _receiving = false;
            _sendBuffer.Clear();
            
            BytesPending = 0;
            BytesSending = 0;
            IsConnected = false;

            OnDisconnected?.Invoke();
            
            return true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        private void OnAsyncCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (_socketDisposed)
                return;
            
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.ReceiveFrom:
                    if (ProcessReceiveFrom(e))
                        TryReceive();
                    break;
                case SocketAsyncOperation.SendTo:
                    ProcessSendTo(e);
                    break;
                default:
                    throw new ArgumentException("The last operation completed on the socket was not a receive or send");
            }
        }
        
        private void TrySend()
        {
            if (_sending || !IsConnected)
                return;
            
            try
            {
                _sending = true;
                _sendEventArg.RemoteEndPoint = _sendEndpoint;
                _sendEventArg.SetBuffer(_sendBuffer.Data, 0, (int)(_sendBuffer.Size));
                if (!_socket.SendToAsync(_sendEventArg))
                    ProcessSendTo(_sendEventArg);
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
            }
        }
        
        private void TryReceive()
        {
            if (_receiving || !IsConnected)
                return;
            
            try
            {
                _receiving = true;
                _receiveEventArg.RemoteEndPoint = _receiveEndpoint;
                _receiveEventArg.SetBuffer(_receiveBuffer.Data, 0, (int)_receiveBuffer.Capacity);
                if (!_socket.ReceiveFromAsync(_receiveEventArg))
                {
                    ProcessReceiveFrom(_receiveEventArg);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
            }
        }
        
        private bool ProcessReceiveFrom(SocketAsyncEventArgs e)
        {
            if (!IsConnected)
            {
                return false;
            }
            
            long size = e.BytesTransferred;
            if (size > 0)
            {
                DatagramsReceived++;
                BytesReceived += size;

                OnReceived?.Invoke(_receiveBuffer.Data, 0, size);
            
                if (_receiveBuffer.Capacity == size)
                {
                    // Check the receive buffer limit
                    if (((2 * size) > OptionReceiveBufferLimit) && (OptionReceiveBufferLimit > 0))
                    {
                        SendError(SocketError.NoBufferSpaceAvailable);
                        Disconnect();
                        return false;
                    }

                    _receiveBuffer.Reserve(2 * size);
                }
            }
            
            _receiving = false;
            if (e.SocketError == SocketError.Success)
            {
                if (size > 0)
                    return true;
                Disconnect();
            }
            else
            {
                SendError(e.SocketError);
                Disconnect();
            }

            return false;
        }
        
        private void ProcessSendTo(SocketAsyncEventArgs e)
        {
            _sending = false;

            if (!IsConnected)
            {
                return;
            }
            
            long sent = e.BytesTransferred;
            
            if (sent > 0)
            {
                BytesSending = 0;
                BytesSent += sent;
                
                _sendBuffer.Clear();
            }

            if (e.SocketError == SocketError.Success)
            {
                return;
            }

            SendError(e.SocketError);
            Disconnect();
        }
        
        private void SendError(SocketError error)
        {
            // Skip disconnect errors
            if ((error == SocketError.ConnectionAborted) ||
                (error == SocketError.ConnectionRefused) ||
                (error == SocketError.ConnectionReset) ||
                (error == SocketError.OperationAborted) ||
                (error == SocketError.Shutdown))
                return;
            
            OnError?.Invoke(error);
        }
        
        private void Dispose(bool disposed)
        {
            if (!IsDisposed)
            {
                if (disposed)
                {
                    Disconnect();
                }
                IsDisposed = true;
            }
        }
    }
}
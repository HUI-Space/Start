/*using System;
using System.Buffers;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Net.Sockets.Kcp;
using Start.Framework;
using Buffer = Start.Framework.Buffer;

namespace Start.Runtime
{
    public class OldKcpClient : IClient, IKcpCallback
    {
        public uint Id { get; private set; }
        public IPEndPoint EndPoint { get; private set; }
        public bool IsConnected { get; private set; }
        public bool IsDisposed { get; private set; }
        public long BytesPending { get; private set; }
        public long BytesSending { get; private set; }
        public long BytesSent { get; private set; }
        public long BytesReceived { get; private set; }
        public int OptionReceiveBufferLimit { get; set; } = 0;
        public int OptionReceiveBufferSize { get; set; } = 8192;
        public int OptionSendBufferLimit { get; set; } = 0;
        public int OptionSendBufferSize { get; set; } = 8192;
        public event Action OnConnecting;
        public event Action OnConnected;
        public event Action<byte[], long, long> OnReceived;
        public event Action OnDisconnecting;
        public event Action OnDisconnected;
        public event Action<SocketError> OnError;
        
        /// <summary>
        /// 客户端发送的数据报数
        /// </summary>
        public long DatagramsSent { get; private set; }
        /// <summary>
        /// 客户端接收到的数据报数
        /// </summary>
        public long DatagramsReceived { get; private set; }
        
        /// <summary>
        /// 下次更新时间 【kcp优化方案】
        /// </summary>
        private DateTimeOffset _nextUpdateTime = DateTimeOffset.UtcNow;
        /// <summary>
        /// 无网络数据更新次数  【kcp优化方案】
        /// </summary>
        private int _noNetDataCount;
        private bool _socketDisposed;
        
        private Socket _socket;
        private Buffer _receiveBuffer;
        private EndPoint _sendEndpoint;
        private EndPoint _receiveEndpoint;
        private SimpleSegManager.Kcp _kcp;
        private SocketAsyncEventArgs _sendEventArg;
        private SocketAsyncEventArgs _receiveEventArg;
        
        /// <summary>
        /// 当前UTC时间
        /// </summary>
        static DateTime UtcNow => new DateTime(DateTime.Now.Ticks + Stopwatch.StartNew().ElapsedMilliseconds * TimeSpan.TicksPerMillisecond).ToUniversalTime();
        
        public OldKcpClient(uint conv,IPEndPoint endPoint)
        {
            Id = conv;
            EndPoint = endPoint;
        }

        public void Update()
        {
            if (IsConnected)
            {
                lock (_kcp)
                {
                    _kcp.Update(DateTimeOffset.UtcNow);
                    if (_noNetDataCount >= 100) return;
                    ++_noNetDataCount;

                    DateTimeOffset utc = DateTimeOffset.UtcNow;
                    if (_nextUpdateTime <= utc)
                    {
                        _kcp.Update(utc);
                        _nextUpdateTime = _kcp.Check(utc);
                    }
                }
            }
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
            
            _sendEndpoint = EndPoint;
            _receiveBuffer = new Buffer();
            
            _sendEventArg = new SocketAsyncEventArgs();
            _receiveEventArg = new SocketAsyncEventArgs();
            
            _sendEventArg.Completed += OnAsyncCompleted;
            _receiveEventArg.Completed += OnAsyncCompleted;
            
            _kcp = new SimpleSegManager.Kcp(Id,this);
            _socket = new Socket(EndPoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);
            _receiveEndpoint = new IPEndPoint((EndPoint.AddressFamily == AddressFamily.InterNetworkV6) ? IPAddress.IPv6Any : IPAddress.Any, 0);
            
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
            
            BytesPending = 0;
            BytesSending = 0;
            BytesSent = 0;
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
            if (IsConnected)
            {
                lock (_kcp)
                {
                    _kcp.Send(buffer);
                    _nextUpdateTime = UtcNow;
                    _noNetDataCount = 0;
                    return true;
                }
            }
            return false;
        }
        
        public void Output(IMemoryOwner<byte> buffer, int validLength)
        {
            SendToAsync(buffer.Memory.Length == validLength
                ? buffer.Memory.ToArray()
                : buffer.Memory.Slice(0, validLength).ToArray());
        }

        public bool Disconnect()
        {
            if (!IsConnected)
                return false;
            
            _receiveEventArg.Completed -= OnAsyncCompleted;
            _sendEventArg.Completed -= OnAsyncCompleted;
            
            OnDisconnecting?.Invoke();

            try
            {
                _socket.Close();
                
                _socket.Dispose();
                
                _receiveEventArg.Dispose();
                _sendEventArg.Dispose();

                _socketDisposed = true;
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
            }

            IsConnected = false;

            BytesPending = 0;
            BytesSending = 0;
            
            OnDisconnected?.Invoke();
            
            return true;
        }

        public bool ReconnectAsync()
        {
            if (!Disconnect())
                return false;

            return ConnectAsync();
        }
        
        public IMemoryOwner<byte> RentBuffer(int length)
        {
            return null;
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
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

        private void SendToAsync(byte[] buffer)
        {
            if (!IsConnected)
                return;
            BytesPending = buffer.Length;
            _sendEventArg.RemoteEndPoint = _sendEndpoint;
            _sendEventArg.SetBuffer(buffer,0, buffer.Length);
            var willRaiseEvent = _socket.SendToAsync(_sendEventArg);
            if (!willRaiseEvent)
                ProcessSend(_sendEventArg);
        }
        
        private void TryReceive()
        {
            while (IsConnected)
            {
                try
                {
                    _receiveEventArg.RemoteEndPoint = _receiveEndpoint;
                    _receiveEventArg.SetBuffer(_receiveBuffer.Data, 0, (int)_receiveBuffer.Capacity);
                    if (!_socket.ReceiveFromAsync(_receiveEventArg))
                    {
                        ProcessReceiveFrom(_receiveEventArg, false);
                    }
                    else
                    {
                        return;
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                    return;
                }
            }
        }

        private void ProcessSend(SocketAsyncEventArgs e)
        {
            if (e.SocketError == SocketError.Success && e.BytesTransferred > 0)
            {
                BytesPending = 0;
                DatagramsSent++;
                BytesSent += e.BytesTransferred;
            }
        }

        private void ProcessReceiveFrom(SocketAsyncEventArgs e, bool isFirst)
        {
            if (e.BytesTransferred > 0 && e.SocketError == SocketError.Success)
            {
                // Update statistic
                DatagramsReceived++;
                BytesReceived += e.BytesTransferred;

                Span<byte> bufferSpan = e.Buffer.AsSpan().Slice(e.Offset, e.BytesTransferred);
                    
                Receive(bufferSpan);
            }

            if (isFirst)
            {
                TryReceive();
            }
        }

        private void OnAsyncCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (_socketDisposed)
                return;
            
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.ReceiveFrom:
                    ProcessReceiveFrom(e,true);
                    break;
                case SocketAsyncOperation.SendTo:
                    ProcessSend(e);
                    break;
                default:
                    throw new ArgumentException("The last operation completed on the socket was not a receive or send");
            }
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

        private void Receive(Span<byte> bufferSpan)
        {
            if (IsConnected)
            {
                lock (_kcp)
                {
                    _kcp.Input(bufferSpan);
                    int len;
                    // 检查接收
                    while ((len = _kcp.PeekSize()) > 0)
                    {
                        var buffer = new byte[len];
                        if (_kcp.Recv(buffer) > 0)
                        {
                            OnReceived?.Invoke(buffer, 0, len);
                        }
                        else break;
                    }

                    _nextUpdateTime = UtcNow;
                    _noNetDataCount = 0;
                }
            }
        }
    }
}*/
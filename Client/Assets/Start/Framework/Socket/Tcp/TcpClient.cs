using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Start
{
    public class TcpClient : IClient
    {
        public uint Id { get; }
        public IPEndPoint EndPoint { get; }
        public bool IsConnected { get; private set; }
        public bool IsDisposed { get; private set; }
        public long BytesSent { get; private set; }
        public long BytesPending { get; private set; }
        public long BytesSending { get; private set; }
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

        private readonly object _sendLock = new object();

        private bool _sending;
        private bool _receiving;
        private bool _connecting;
        private bool _socketDisposed;
        private long _sendBufferFlushOffset;

        private Socket _socket;
        private Buffer _sendBufferMain;
        private Buffer _sendBufferFlush;
        private Buffer _receiveBuffer;

        private SocketAsyncEventArgs _sendEventArg;
        private SocketAsyncEventArgs _connectEventArg;
        private SocketAsyncEventArgs _receiveEventArg;

        public TcpClient(uint id, IPEndPoint endPoint)
        {
            Id = id;
            EndPoint = endPoint;
        }

        public bool ConnectAsync()
        {
            if (IsConnected || _connecting)
            {
                return false;
            }

            _receiveBuffer = new Buffer();
            _sendBufferMain = new Buffer();
            _sendBufferFlush = new Buffer();

            _sendEventArg = new SocketAsyncEventArgs();
            _receiveEventArg = new SocketAsyncEventArgs();
            _connectEventArg = new SocketAsyncEventArgs();

            _sendEventArg.Completed += OnAsyncCompleted;
            _receiveEventArg.Completed += OnAsyncCompleted;
            _connectEventArg.Completed += OnAsyncCompleted;
            _connectEventArg.RemoteEndPoint = EndPoint;

            _socket = new Socket(EndPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            _socketDisposed = false;
            _connecting = true;

            OnConnecting?.Invoke();
            if (!_socket.ConnectAsync(_connectEventArg))
            {
                ProcessConnect(_connectEventArg);
            }

            return true;
        }

        public bool SendAsync(byte[] buffer)
        {
            if (!IsConnected)
            {
                return false;
            }

            lock (_sendLock)
            {
                if (_sendBufferMain.Size + buffer.Length > OptionSendBufferLimit && OptionSendBufferLimit > 0)
                {
                    SendError(SocketError.NoBufferSpaceAvailable);
                    return false;
                }

                _sendBufferMain.Append(buffer);
                BytesPending = _sendBufferMain.Size;

                if (_sending)
                {
                    return true;
                }

                _sending = true;

                TrySend();
            }

            return true;
        }

        public bool ReconnectAsync()
        {
            if (!Disconnect())
            {
                return false;
            }

            return ConnectAsync();
        }

        public bool Disconnect()
        {
            if (!IsConnected && !_connecting)
            {
                return false;
            }

            if (_connecting)
            {
                Socket.CancelConnectAsync(_connectEventArg);
            }

            _sendEventArg.Completed -= OnAsyncCompleted;
            _connectEventArg.Completed -= OnAsyncCompleted;
            _receiveEventArg.Completed -= OnAsyncCompleted;

            OnDisconnecting?.Invoke();
            try
            {
                _socket.Shutdown(SocketShutdown.Both);
            }
            catch (SocketException socketException)
            {
                Logger.Error("Disconnect Socket Error :" + socketException.Message);
            }

            try
            {
                _socket.Close();
                _socket.Dispose();
                _sendEventArg.Dispose();
                _connectEventArg.Dispose();
                _receiveEventArg.Dispose();
                _socketDisposed = true;
            }
            catch (ObjectDisposedException exception)
            {
                Logger.Error("Disconnect Socket Error :" + exception.Message);
            }

            IsConnected = false;
            _receiving = false;
            _sending = false;

            lock (_sendLock)
            {
                _sendBufferMain.Clear();
                _sendBufferFlush.Clear();
                _sendBufferFlushOffset = 0;

                BytesPending = 0;
                BytesSending = 0;
            }

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
                case SocketAsyncOperation.Connect:
                    ProcessConnect(e);
                    break;
                case SocketAsyncOperation.Receive:
                    if (ProcessReceiveFrom(e))
                        TryReceive();
                    break;
                case SocketAsyncOperation.Send:
                    if (ProcessSendTo(e))
                        TrySend();
                    break;
                default:
                    throw new ArgumentException("The last operation completed on the socket was not a receive or send");
            }
        }

        private void TrySend()
        {
            if (!IsConnected) return;

            var empty = false;
            var process = true;

            while (process)
            {
                process = false;

                lock (_sendLock)
                {
                    if (_sendBufferFlush.IsEmpty)
                    {
                        //交换刷新缓冲区和主缓冲区
                        _sendBufferFlush = Interlocked.Exchange(ref _sendBufferMain, _sendBufferFlush);
                        _sendBufferFlushOffset = 0;

                        BytesPending = 0;
                        BytesSending += _sendBufferFlush.Size;

                        if (_sendBufferFlush.IsEmpty)
                        {
                            empty = true;
                            _sending = false;
                        }
                    }
                    else
                    {
                        return;
                    }
                }

                if (empty) return;

                try
                {
                    // Async write with the write handler
                    _sendEventArg.SetBuffer(_sendBufferFlush.Data, (int)_sendBufferFlushOffset,
                        (int)(_sendBufferFlush.Size - _sendBufferFlushOffset));
                    if (!_socket.SendAsync(_sendEventArg))
                        process = ProcessSendTo(_sendEventArg);
                }
                catch (ObjectDisposedException)
                {
                }
            }
        }

        private void TryReceive()
        {
            if (_receiving || !IsConnected) return;

            var process = true;
            while (process)
            {
                process = false;
                try
                {
                    _receiving = true;
                    _receiveEventArg.SetBuffer(_receiveBuffer.Data, 0, (int)_receiveBuffer.Capacity);
                    if (!_socket.ReceiveAsync(_receiveEventArg))
                    {
                        process = ProcessReceiveFrom(_receiveEventArg);
                    }
                }
                catch (ObjectDisposedException)
                {
                }
            }
        }

        private void ProcessConnect(SocketAsyncEventArgs e)
        {
            _connecting = false;

            if (e.SocketError == SocketError.Success)
            {
                _receiveBuffer.Reserve(OptionReceiveBufferSize);
                _sendBufferMain.Reserve(OptionSendBufferSize);
                _sendBufferFlush.Reserve(OptionSendBufferSize);

                BytesPending = 0;
                BytesSending = 0;
                BytesSent = 0;
                BytesReceived = 0;

                IsConnected = true;

                TryReceive();

                if (_socketDisposed)
                {
                    return;
                }

                OnConnected?.Invoke();
            }
            else
            {
                SendError(e.SocketError);
                OnDisconnected?.Invoke();
            }
        }

        private bool ProcessReceiveFrom(SocketAsyncEventArgs e)
        {
            if (!IsConnected) return false;

            long size = e.BytesTransferred;
            //从服务器接收到一些数据
            if (size > 0)
            {
                BytesReceived += size;
                OnReceived?.Invoke(_receiveBuffer.Data, 0, size);
                //如果接收缓冲区已满，则增加其大小
                if (_receiveBuffer.Capacity == size)
                {
                    if (2 * size > OptionReceiveBufferLimit && OptionReceiveBufferLimit > 0)
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

        private bool ProcessSendTo(SocketAsyncEventArgs e)
        {
            if (!IsConnected)
            {
                return false;
            }

            long size = e.BytesTransferred;
            //向服务器发送一些数据
            if (size > 0)
            {
                BytesSending -= size;
                BytesSent += size;

                // 增加刷新缓冲区偏移量
                _sendBufferFlushOffset += size;

                // 成功发送整个刷新缓冲区
                if (_sendBufferFlushOffset == _sendBufferFlush.Size)
                {
                    // 清除刷新缓冲区
                    _sendBufferFlush.Clear();
                    _sendBufferFlushOffset = 0;
                }
            }

            if (e.SocketError == SocketError.Success)
                return true;

            SendError(e.SocketError);
            Disconnect();
            return false;
        }

        private void SendError(SocketError error)
        {
            // 跳过断开连接错误
            if (error == SocketError.ConnectionAborted ||
                error == SocketError.ConnectionRefused ||
                error == SocketError.OperationAborted ||
                error == SocketError.ConnectionReset ||
                error == SocketError.Shutdown)
                return;
            OnError?.Invoke(error);
        }

        private void Dispose(bool dispose)
        {
            if (!IsDisposed)
            {
                if (dispose) Disconnect();

                IsDisposed = true;
            }
        }
    }
}
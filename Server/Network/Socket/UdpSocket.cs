using System.Net;
using System.Net.Sockets;

namespace Start.Server
{
    public class UdpSocket : IDisposable
    {
        /// <summary>
        /// Server Id
        /// </summary>
        public int Id { get; private set; }
        
        /// <summary>
        /// UDP server address
        /// </summary>
        public string Address { get; private set; }
        
        /// <summary>
        /// UDP server port
        /// </summary>
        public int Port { get; private set; }
        
        /// <summary>
        /// Endpoint
        /// </summary>
        public EndPoint Endpoint { get; private set; }
        
        /// <summary>
        /// Socket
        /// </summary>
        public Socket Socket { get; private set; }
        
        /// <summary>
        /// 服务器发送的待处理字节数
        /// </summary>
        public long BytesPending { get; private set; }
        
        /// <summary>
        /// 服务器正在发送的字节数
        /// </summary>
        public long BytesSending { get; private set; }
        
        /// <summary>
        /// 服务器已经发送的字节数
        /// </summary>
        public long BytesSent { get; private set; }
        
        /// <summary>
        /// 服务器已经接收到的字节数
        /// </summary>
        public long BytesReceived { get; private set; }
        
        /// <summary>
        /// 服务器发送的数据报数
        /// </summary>
        public long DatagramsSent { get; private set; }
        
        /// <summary>
        /// 服务器接收到的数据报数
        /// </summary>
        public long DatagramsReceived { get; private set; }

        /// <summary>
        /// Option: 双模插座
        /// </summary>
        /// <remarks>
        /// 指定Socket是否为IPv4和IPv6双模式Socket。
        /// 只在socket绑定IPv6地址时生效
        /// </remarks>
        public bool OptionDualMode { get; set; }
        
        /// <summary>
        /// 选项：重用地址
        /// </summary>
        /// <remarks>
        /// 启用或禁用SO_REUSEADDR选项，允许套接字地址重用，前提是操作系统支持此功能
        /// </remarks>
        public bool OptionReuseAddress { get; set; }
        
        /// <summary>
        /// Option：表示将socket绑定为独占访问
        /// </summary>
        /// <remarks>
        /// 启用或禁用SO_EXCLUSIVEADDRUSE选项，确保套接字绑定地址的独占访问，前提是操作系统支持此功能。
        /// </remarks>
        public bool OptionExclusiveAddressUse { get; set; }
        
        /// <summary>
        /// 选项: 接收缓冲区限制
        /// </summary>
        public int OptionReceiveBufferLimit { get; set; } = 0;
        
        /// <summary>
        /// 选项: 接收缓冲区大小
        /// </summary>
        public int OptionReceiveBufferSize { get; set; } = 8192;
        
        /// <summary>
        /// 选项: 发送缓冲区限制
        /// </summary>
        public int OptionSendBufferLimit { get; set; } = 0;
        
        /// <summary>
        /// 选项：发送缓冲区大小
        /// </summary>
        public int OptionSendBufferSize { get; set; } = 8192;
        
        /// <summary>
        /// 服务器是否开始?
        /// </summary>
        public bool IsStarted { get; private set; }
        
        /// <summary>
        /// 处理标记
        /// </summary>
        public bool IsDisposed { get; private set; }

        /// <summary>
        /// 服务器socket处理标志  
        /// </summary>
        public bool IsSocketDisposed { get; private set; } = true;
        
        protected EndPoint _sendEndpoint;
        private EndPoint _receiveEndpoint;
        
        // Receive buffer
        private bool _receiving;
        private Buffer _receiveBuffer;
        private SocketAsyncEventArgs _receiveEventArg;
        // Send buffer
        private bool _sending;
        private Buffer _sendBuffer;
        private SocketAsyncEventArgs _sendEventArg;
        
        public UdpSocket(int id, EndPoint endpoint, string address, int port)
        {
            Id = id;
            Endpoint = endpoint;
            Address = address;
            Port = port;
        }
        
        /// <summary>
        /// 开始服务器 (同步)
        /// </summary>
        /// <returns>如果服务器启动成功，则为“true”；如果服务器启动失败，则为“false”</returns>
        public virtual bool Start()
        {
            if (IsStarted)
                return false;

            // 设置缓冲
            _receiveBuffer = new Buffer();
            _sendBuffer = new Buffer();
            
            // 设置事件参数
            _receiveEventArg = new SocketAsyncEventArgs();
            _receiveEventArg.Completed += OnAsyncCompleted;
            _sendEventArg = new SocketAsyncEventArgs();
            _sendEventArg.Completed += OnAsyncCompleted;

            // 创建一个新的服务器socket
            Socket = new Socket(Endpoint.AddressFamily, SocketType.Dgram, ProtocolType.Udp);

            // 更新服务器socket已处置标志
            IsSocketDisposed = false;

            // 应用选项：重用地址
            Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, OptionReuseAddress);
            // 应用选项：独占地址使用
            Socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ExclusiveAddressUse, OptionExclusiveAddressUse);
            // 应用选项：双模式（此选项必须在接收前应用）
            if (Socket.AddressFamily == AddressFamily.InterNetworkV6)
            {
                Socket.DualMode = OptionDualMode;
            }
                

            // 将服务器Socket绑定到端点
            Socket.Bind(Endpoint);
            // 根据创建的实际端点刷新端点属性
            Endpoint = (IPEndPoint)Socket.LocalEndPoint;

            // 调用服务器启动处理程序
            OnStarting();

            // 准备接收端点
            _receiveEndpoint = new IPEndPoint(Endpoint.AddressFamily == AddressFamily.InterNetworkV6 ? 
                IPAddress.IPv6Any : IPAddress.Any, 0);

            // 准备接收和发送缓冲区
            _receiveBuffer.Reserve(OptionReceiveBufferSize);

            // 重置统计
            BytesPending = 0;
            BytesSending = 0;
            BytesSent = 0;
            BytesReceived = 0;
            DatagramsSent = 0;
            DatagramsReceived = 0;

            // 更新启动标志
            IsStarted = true;

            // 调用服务器启动处理程序
            OnStarted();
            // 调用接收处理程序
            TryReceive();
            return true;
        }
        
        /// <summary>
        /// 停止服务器 (同步)
        /// </summary>
        /// <returns>如果服务器已成功停止，则为true；如果服务器已停止，则为false</returns>
        public virtual bool Stop()
        {
            if (!IsStarted)
                return false;

            // 重置事件参数
            _receiveEventArg.Completed -= OnAsyncCompleted;
            _sendEventArg.Completed -= OnAsyncCompleted;

            // 调用服务器停止处理程序
            OnStopping();

            try
            {
                // 关闭服务器Socket
                Socket.Close();

                // Dispose the server socket
                Socket.Dispose();

                // Dispose event arguments
                _receiveEventArg.Dispose();
                _sendEventArg.Dispose();

                // Update the server socket disposed flag
                IsSocketDisposed = true;
            }
            catch (ObjectDisposedException) {}

            // Update the started flag
            IsStarted = false;

            // Update sending/receiving flags
            _receiving = false;
            _sending = false;

            // Clear send/receive buffers
            ClearBuffers();

            // Call the server stopped handler
            OnStopped();

            return true;
        }
        
        
        /// <summary>
        /// 重新启动服务器 (同步)
        /// </summary>
        /// <returns>如果服务器成功重启，则为“true”；如果服务器重启失败，则为“false”</returns>
        public virtual bool Restart()
        {
            if (!Stop())
                return false;

            return Start();
        }
        
        /// <summary>
        /// 将数据报发送到给定端点 (异步)
        /// </summary>
        /// <param name="endpoint">Endpoint to send</param>
        /// <param name="buffer">Datagram buffer to send as a span of bytes</param>
        /// <returns>如果数据报已成功发送，则为true；如果数据报未发送，则为false</returns>
        public virtual bool SendAsync(EndPoint endpoint, byte[] buffer)
        {
            if (_sending)
                return false;

            if (!IsStarted)
                return false;

            if (buffer == null || buffer.Length == 0)
                return true;

            // 检查发送缓冲区限制
            if (_sendBuffer.Size + buffer.Length > OptionSendBufferLimit && OptionSendBufferLimit > 0)
            {
                SendError(SocketError.NoBufferSpaceAvailable);
                return false;
            }

            // 填充主发送缓冲区
            _sendBuffer.Append(buffer);

            // 更新统计数据
            BytesSending = _sendBuffer.Size;

            // 更新发送端点
            _sendEndpoint = endpoint;

            // 尝试发送主缓冲区
            TrySend();

            return true;
        }
        
        /// <summary>
        /// 尝试接收新数据
        /// </summary>
        private void TryReceive()
        {
            if (_receiving)
                return;

            if (!IsStarted)
                return;

            try
            {
                // 使用接收处理程序进行异步接收
                _receiving = true;
                _receiveEventArg.RemoteEndPoint = _receiveEndpoint;
                _receiveEventArg.SetBuffer(_receiveBuffer.Data, 0, (int)_receiveBuffer.Capacity);
                if (!Socket.ReceiveFromAsync(_receiveEventArg))
                    ProcessReceiveFrom(_receiveEventArg);
            }
            catch (ObjectDisposedException) {}
        }

        /// <summary>
        /// 尝试发送挂起的数据
        /// </summary>
        private void TrySend()
        {
            if (_sending)
                return;

            if (!IsStarted)
                return;

            try
            {
                // 使用写处理程序进行异步写
                _sending = true;
                _sendEventArg.RemoteEndPoint = _sendEndpoint;
                _sendEventArg.SetBuffer(_sendBuffer.Data, 0, (int)(_sendBuffer.Size));
                if (!Socket.SendToAsync(_sendEventArg))
                    ProcessSendTo(_sendEventArg);
            }
            catch (ObjectDisposedException) {}
        }
        
        private void ClearBuffers()
        {
            // Clear send buffers
            _sendBuffer.Clear();

            // Update statistic
            BytesPending = 0;
            BytesSending = 0;
        }
        
        /// <summary>
        /// 每当在socket上完成接收或发送操作时，就调用此方法
        /// </summary>
        private void OnAsyncCompleted(object sender, SocketAsyncEventArgs e)
        {
            if (IsSocketDisposed)
                return;

            // 确定刚刚完成的操作类型并调用关联的处理程序
            switch (e.LastOperation)
            {
                case SocketAsyncOperation.ReceiveFrom:
                    ProcessReceiveFrom(e);
                    break;
                case SocketAsyncOperation.SendTo:
                    ProcessSendTo(e);
                    break;
                default:
                    throw new ArgumentException("The last operation completed on the socket was not a receive or send");
            }

        }

        /// <summary>
        /// 当异步接收操作完成时调用此方法
        /// </summary>
        private void ProcessReceiveFrom(SocketAsyncEventArgs e)
        {
            _receiving = false;

            if (!IsStarted)
                return;

            // 检查错误
            if (e.SocketError != SocketError.Success)
            {
                SendError(e.SocketError);

                // 调用接收到的数据报零处理程序
                OnReceived(e.RemoteEndPoint, _receiveBuffer.Data, 0, 0);
                
                TryReceive();
                return;
            }

            // 从客户端收到一些数据
            long size = e.BytesTransferred;

            // 更新属性
            DatagramsReceived++;
            BytesReceived += size;

            // 调用接收到的数据报处理程序
            OnReceived(e.RemoteEndPoint, _receiveBuffer.Data, 0, size);
            
            TryReceive();
            
            // 如果接收缓冲区已满，则增加其大小
            if (_receiveBuffer.Capacity == size)
            {
                // 检查接收缓冲区限制
                if (2 * size > OptionReceiveBufferLimit && OptionReceiveBufferLimit > 0)
                {
                    SendError(SocketError.NoBufferSpaceAvailable);

                    // 调用接收到的数据报零处理程序
                    OnReceived(e.RemoteEndPoint, _receiveBuffer.Data, 0, 0);
                    
                    TryReceive();
                    return;
                }

                _receiveBuffer.Reserve(2 * size);
            }
        }

        /// <summary>
        /// 当异步发送操作完成时调用此方法
        /// </summary>
        private void ProcessSendTo(SocketAsyncEventArgs e)
        {
            _sending = false;

            if (!IsStarted)
                return;

            // 检查错误
            if (e.SocketError != SocketError.Success)
            {
                SendError(e.SocketError);

                // 调用缓冲区发送零处理程序
                OnSent(_sendEndpoint, 0);

                return;
            }

            long sent = e.BytesTransferred;

            // 向客户端发送一些数据
            if (sent > 0)
            {
                // 更新统计数据
                BytesSending = 0;
                BytesSent += sent;

                // 清除发送缓冲区
                _sendBuffer.Clear();

                // 调用缓冲区发送处理程序
                OnSent(_sendEndpoint, sent);
            }
        }

        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        protected virtual void Dispose(bool disposingManagedResources)
        {
            if (!IsDisposed)
            {
                if (disposingManagedResources)
                {
                    Stop();
                }
                IsDisposed = true;
            }
        }

        /// <summary>
        /// 处理服务器开始通知
        /// </summary>
        protected virtual void OnStarting(){}

        /// <summary>
        /// 处理服务器已经开始通知
        /// </summary>
        protected virtual void OnStarted()
        {
            
        }
        
        /// <summary>
        /// 在对象停止时调用。可以被子类重写。
        /// </summary>
        protected virtual void OnStopping()
        {
            
        }

        /// <summary>
        /// 在对象已停止时调用。可以被子类重写。
        /// </summary>
        protected virtual void OnStopped()
        {
            
        }

        /// <summary>
        /// 当接收到数据时调用。可以被子类重写。
        /// </summary>
        /// <param name="endpoint">发送数据的终结点。</param>
        /// <param name="buffer">包含接收数据的缓冲区。</param>
        /// <param name="offset">缓冲区中接收数据的起始位置。</param>
        /// <param name="size">接收数据的大小。</param>
        protected virtual void OnReceived(EndPoint endpoint, byte[] buffer, long offset, long size)
        {
            
        }

        /// <summary>
        /// 当数据发送完成时调用。可以被子类重写。
        /// </summary>
        /// <param name="endpoint">发送数据的目标终结点。</param>
        /// <param name="sent">已发送的数据量。</param>
        protected virtual void OnSent(EndPoint endpoint, long sent)
        {
            
        }

        /// <summary>
        /// 当发生错误时调用。可以被子类重写。
        /// </summary>
        /// <param name="error">发生的套接字错误。</param>
        protected virtual void OnError(SocketError error)
        {
            
        }

        /// <summary>
        /// 发送错误信息。
        /// </summary>
        /// <param name="error">发生的套接字错误。</param>
        private void SendError(SocketError error)
        {
            // 忽略断开连接相关的错误
            if ((error == SocketError.ConnectionAborted) ||
                (error == SocketError.ConnectionRefused) ||
                (error == SocketError.ConnectionReset) ||
                (error == SocketError.OperationAborted) ||
                (error == SocketError.Shutdown))
                return;

            // 触发错误事件
            OnError(error);
        }

    }
}

    
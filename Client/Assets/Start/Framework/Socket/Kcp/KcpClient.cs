using System;
using System.Net;
using System.Net.Sockets;


namespace Start
{
    public class KcpClient : IClient
    {
        public uint Id => _udpClient.Id;
        public IPEndPoint EndPoint => _udpClient.EndPoint;
        public bool IsDisposed => _udpClient.IsDisposed;
        public bool IsConnected => _udpClient.IsConnected;
        public long BytesSent => _udpClient.BytesSent;
        public long BytesPending => _udpClient.BytesPending;
        public long BytesSending => _udpClient.BytesSending;
        public long BytesReceived => _udpClient.BytesReceived;
        public int OptionReceiveBufferLimit => _udpClient.OptionReceiveBufferLimit;
        public int OptionReceiveBufferSize => _udpClient.OptionReceiveBufferSize;
        public int OptionSendBufferLimit => _udpClient.OptionSendBufferLimit;
        public int OptionSendBufferSize => _udpClient.OptionSendBufferSize;
        /// <summary>
        /// 客户端发送的数据报数
        /// </summary>
        public long DatagramsSent => _udpClient.DatagramsSent;
        /// <summary>
        /// 客户端接收到的数据报数
        /// </summary>
        public long DatagramsReceived => _udpClient.DatagramsReceived;
        
        public event Action OnConnecting;
        public event Action OnConnected;
        public event Action<byte[], long, long> OnReceived;
        public event Action OnDisconnecting;
        public event Action OnDisconnected;
        public event Action<SocketError> OnError;
        
        private UdpClient _udpClient;
        private IKcpHelper _kcpHelper;

        public KcpClient(uint id,IPEndPoint endPoint,IKcpHelper kcpHelper)
        {
            _kcpHelper = kcpHelper;
            _udpClient = new UdpClient(id, endPoint);
            _kcpHelper.OnSendKcp += SendKcpHelper;
            _kcpHelper.OnReceiveKcp += ReceiveKcpHelper;
            _udpClient.OnConnecting += Connecting;
            _udpClient.OnConnected += Connected;
            _udpClient.OnReceived += Received;
            _udpClient.OnDisconnecting += Disconnecting;
            _udpClient.OnDisconnected += Disconnected;
            _udpClient.OnError += Error;
        }
        
        public bool ConnectAsync()
        {
            return _udpClient.ConnectAsync();
        }

        public bool SendAsync(byte[] buffer)//1.发送到Kcp
        {
            try
            {
                _kcpHelper.SendKcp(buffer);
                return true;
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
                return false;
            }
        }

        private void SendKcpHelper(byte[] buffer)//2.Kcp处理完成后，通过Udp发送
        {
            _udpClient.SendAsync(buffer);
        }
        
        private void Received(byte[] bytes, long offset, long count)//3.Udp接收到数据，通过Kcp处理
        {
            _kcpHelper.ReceiveKcp(bytes,offset,count);
        }
        
        private void ReceiveKcpHelper(byte[] buffer)//4.Kcp处理完数据，通知回调
        {
            OnReceived?.Invoke(buffer, 0, buffer.Length);
        }
        
        public bool ReconnectAsync()
        {
            return _udpClient.ReconnectAsync();
        }

        public bool Disconnect()
        {
            return _udpClient.Disconnect();
        }
        
        public void Dispose()
        {
            _udpClient.Dispose();
            _udpClient.OnConnecting -= Connecting;
            _udpClient.OnConnected -= Connected;
            _udpClient.OnReceived -= Received;
            _udpClient.OnDisconnecting -= Disconnecting;
            _udpClient.OnDisconnected -= Disconnected;
            _udpClient.OnError -= Error;
            _udpClient = null;
            
            _kcpHelper.OnSendKcp -= SendKcpHelper;
            _kcpHelper.OnReceiveKcp -= ReceiveKcpHelper;
            _kcpHelper = null;
        }
        
        private void Connected()
        {
            OnConnected?.Invoke();
        }

        private void Connecting()
        {
            OnConnecting?.Invoke();
        }
        
        private void Disconnecting()
        {
            OnDisconnecting?.Invoke();
        }

        private void Disconnected()
        {
            OnDisconnected?.Invoke();
        }
        
        private void Error(SocketError err)
        {
            OnError?.Invoke(err);
        }
    }
}
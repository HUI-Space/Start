using System.Buffers;
using System.Net;
using System.Net.Sockets.Kcp;



namespace Start.Server ;

    public class KcpChannel : IChannel , IKcpCallback
    {
        public int SessionId { get; private set; }
        
        public EndPoint RemoteAddress { get; private set; }
        
        /// <summary>
        /// 是否开始
        /// </summary>
        private bool _isStarted;
        
        /// <summary>
        /// 无网络数据更新次数  【kcp优化方案】
        /// </summary>
        private int _noNetDataCount;
        
        /// <summary>
        /// 下次更新时间 【kcp优化方案】
        /// </summary>
        private DateTimeOffset _nextUpdateTime;
        
        private SimpleSegManager.Kcp _kcp;

        private KcpServer _kcpServer;
        
        public static KcpChannel Create(uint conv,int sessionId, EndPoint remoteAddress,KcpServer kcpServer)
        {
            KcpChannel kcpChannel = RecyclableObjectPool.Acquire<KcpChannel>();
            kcpChannel.SessionId = sessionId;
            kcpChannel.RemoteAddress = remoteAddress;
            kcpChannel._isStarted = true;
            kcpChannel._noNetDataCount = 0;
            kcpChannel._nextUpdateTime = DateTimeOffset.Now;
            kcpChannel._kcp = new SimpleSegManager.Kcp(conv, kcpChannel);
            kcpChannel._kcpServer = kcpServer;
            return kcpChannel;
        }

        public void Send(Span<byte> data)
        {
            if (_isStarted)
            {
                lock (_kcp)
                {
                    _kcp.Send(data);
                    _nextUpdateTime =  DateTime.UtcNow;
                    _noNetDataCount = 0;
                }
            }
        }
        
        public void Receive(byte[] data)
        {
            if (_isStarted)
            {
                lock (_kcp)
                {
                    _kcp.Input(data);
                    int len;
                    // 检查接收
                    while ((len = _kcp.PeekSize()) > 0)
                    {
                        var buffer = new byte[len];
                        if (_kcp.Recv(buffer) > 0)
                        {
                            KcpReceive(buffer);
                        }
                        else break;
                    }
                    _nextUpdateTime = DateTime.UtcNow;
                    _noNetDataCount = 0;
                }
            }
        }

        public void Update()
        {
            if (_isStarted)
            {
                lock (_kcp)
                {
                    // 更新周期10ms 此处次数大于100 则为 1s 无数据跳出
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
        
        public void Output(IMemoryOwner<byte> buffer, int avalidLength)
        {
            if (buffer.Memory.Length == avalidLength)
            {
                _kcpServer.SendAsync(RemoteAddress,  KcpHelper.EncodePacket(SessionId, buffer.Memory.ToArray()));
            }
            else
            {
                _kcpServer.SendAsync(RemoteAddress, KcpHelper.EncodePacket(SessionId,buffer.Memory.Slice(0, avalidLength).ToArray()));
            }
        }
        
        
        public void Reset()
        {
            SessionId = default;
            RemoteAddress = default;
            _isStarted = false;
            _noNetDataCount = 0;
            _nextUpdateTime = default;
            _kcp.Dispose();
            _kcp = default;
            _kcpServer = default;
        }
        
        private void KcpReceive(byte[] buffer)
        {
            EMessageID messageId = (EMessageID)BitConverter.ToInt32(buffer.Take(4).ToArray());
            Type type = MessageBinding.GetTypeByMessageID(messageId);
            IMessage message = MessageHelper.Deserialize<IMessage>(type, buffer.Skip(4).ToArray());
            IGenericData genericData = GenericData<int,IMessage>.Create(SessionId, message);
            Console.WriteLine($"KcpReceive SessionId:{SessionId} MessageId:{messageId} Message:{message}");

            EventManager.Instance.SendMessage((int)EMessageType.Kcp, (int)messageId, genericData);
            RecyclableObjectPool.Recycle(genericData);
        }
    }
    
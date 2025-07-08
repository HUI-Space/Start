using System;
using System.Linq;
using System.Net.Sockets.Kcp;

using BufferOwner = System.Buffers.IMemoryOwner<byte>;

namespace Start
{
    public class KcpHelper : IKcpHelper, IKcpCallback
    {
        public event Action<byte[]> OnSendKcp;
        public event Action<byte[]> OnReceiveKcp;
        private readonly SimpleSegManager.Kcp _kcp;
        
        /// <summary>
        /// 下次更新时间 【kcp优化方案】
        /// </summary>
        private DateTimeOffset _nextUpdateTime = DateTimeOffset.UtcNow;
        /// <summary>
        /// 无网络数据更新次数  【kcp优化方案】
        /// </summary>
        private int _noNetDataCount;
        
        private bool _isClosed;

        public KcpHelper(uint conv , int mtu, int winSize, EKcpMode eKcpMode)
        {
            _kcp = new SimpleSegManager.Kcp(conv, this);
            if (eKcpMode == EKcpMode.Normal) _kcp.NoDelay(0, 40, 0, 0);
            else if (eKcpMode == EKcpMode.Fast) _kcp.NoDelay(1, 10, 2, 1);
            _kcp.WndSize(winSize, winSize);
            _kcp.SetMtu(mtu);
        }
        
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (_isClosed) return;

            lock (_kcp)
            {
                if (_isClosed) return;
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

        public void SendKcp(byte[] buffer)
        {
            if (_isClosed)
            {
                return;
            }

            lock (_kcp)
            {
                if (_isClosed)
                {
                    return;
                }
                _kcp.Send(buffer);
                _nextUpdateTime = DateTimeOffset.UtcNow;;
                _noNetDataCount = 0;
            }
        }

        public void ReceiveKcp(byte[] data, long offset, long count)
        {
            byte[] result = DecodePacket(data,count);
            if (_isClosed)
            {
                return;
            }

            lock (_kcp)
            {
                if (_isClosed)
                {
                    return;
                }
                _kcp.Input(result);
                int len;
                // 检查接收
                while ((len = _kcp.PeekSize()) > 0)
                {
                    var buffer = new byte[len];
                    if (_kcp.Recv(buffer) > 0)
                    {
                        OnReceiveKcp?.Invoke(buffer);
                    }
                    else break;
                }
                _nextUpdateTime = DateTimeOffset.UtcNow;;
                _noNetDataCount = 0;
            }
        }

        public void Output(BufferOwner buffer, int validLength)
        {
            byte[] data = buffer.Memory.Length == validLength
                ? buffer.Memory.ToArray()
                : buffer.Memory.Slice(0, validLength).ToArray();
            
            OnSendKcp?.Invoke(EncodePacket(SessionId,data));
        }
        
        
        public static int SessionId = -1;
        
        public static uint GetSessionId(byte[] buffer)
        {
            return BitConverter.ToUInt32(buffer, 1);
        }
        
        /// <summary>
        /// 编码包
        /// </summary>
        /// <returns></returns>
        public static byte[] EncodePacket(int sessionId, byte[] message)
        {
            byte[] buffer = new byte[4 + message.Length];
            Array.Copy(BitConverter.GetBytes(sessionId), 0, buffer, 0, 4);
            Array.Copy(message, 0, buffer, 4, message.Length);
            return buffer;
        }
        
        /// <summary>
        /// 解码包
        /// </summary>
        public static byte[] DecodePacket(byte[] bytes, long size)
        {
            return bytes.Skip(4).Take((int)size).ToArray();
        }
    }
}
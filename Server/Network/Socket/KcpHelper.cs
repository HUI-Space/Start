

namespace Start.Server ;

    public static class KcpHelper
    {
        private static int _sessionId = 0;
        
        /// <summary>
        /// 简易的生成会话ID
        /// </summary>
        /// <returns></returns>
        public static int GenerateSessionId()
        {
            return Interlocked.Add(ref _sessionId, 1);
        }
        
        public static int GetSessionId(byte[] buffer)
        {
            return BitConverter.ToInt32(buffer, 0);
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
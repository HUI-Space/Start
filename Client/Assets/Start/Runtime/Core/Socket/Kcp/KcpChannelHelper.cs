using System;

namespace Start
{
    public class KcpChannelHelper : IChannelHelper
    {
        public uint ErrCode => 1;
        public uint GetMessageId(Type type)
        {
            throw new NotImplementedException();
        }

        public byte[] Serialize<T>(T t)
        {
            throw new NotImplementedException();
        }

        public T Deserialize<T>(byte[] data)
        {
            throw new NotImplementedException();
        }

        public byte[] EncodePacket(uint messageId, byte[] message, uint serialNumber)
        {
            throw new NotImplementedException();
        }

        public void DecodePacket(byte[] bytes, out uint errCode, out uint serialNumber, out uint messageId, out byte[] message)
        {
            throw new NotImplementedException();
        }
    }
}
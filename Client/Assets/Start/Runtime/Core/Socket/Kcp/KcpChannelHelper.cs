using System;

namespace Start
{
    public class KcpChannelHelper : IChannelHelper
    {
        public uint ErrCode => 1;
        public uint GetMessageId(Type type)
        {
            return (uint)MessageBinding.GetMessageIDByType(type);
        }

        public byte[] Serialize<T>(T t)
        {
            return MessageHelper.Serialize(t);
        }

        public T Deserialize<T>(byte[] data)
        {
            return MessageHelper.Deserialize<T>(data);
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
using System;
using System.Text;
using Newtonsoft.Json;


namespace Start
{
    public class TcpChannelHelper : ITcpChannelHelper
    {
        /// <summary>
        /// 最大缓存包大小
        /// </summary>
        public const int MAX_CACHE_PACKAGE_LENGTH = 65536;

        /// <summary>
        /// 消息包包长部分所占长度
        /// </summary>
        public const int PACKAGE_LENGTH_PART_LENGTH = 2;

        /// <summary>
        /// 消息包发包序号部分所占长度
        /// </summary>
        public const int PACKAGE_SEQUENCE_PART_LENGTH = 4;

        /// <summary>
        /// 消息包消息ID部分所占长度
        /// </summary>
        public const int PACKAGE_MESSAGE_ID_PART_LENGTH = 2;

        /// <summary>
        /// 消息包错误码部分所占长度
        /// </summary>
        public const int PACKAGE_ERROR_PART_LENGTH = 2;


        public uint ErrCode => 1;
        public uint PacketLength => PACKAGE_LENGTH_PART_LENGTH;
        public uint GetPacketLength(byte[] bytes)
        {
            var packageLengthPart = new byte[PACKAGE_LENGTH_PART_LENGTH];
            Array.Copy(bytes, 0, packageLengthPart, 0, PACKAGE_LENGTH_PART_LENGTH);
            return SwapUInt16(BitConverter.ToUInt16(packageLengthPart, 0));
        }

        public uint GetMessageId(Type type)
        {
            return (uint)MessageBinding.GetMessageIDByType(type);
        }

        public byte[] EncodePacket(uint messageId, byte[] message, uint serialNumber)
        {
            var sequencePart = BitConverter.GetBytes(SwapUInt32(serialNumber));
            var messageIdPart = BitConverter.GetBytes(SwapUInt16((ushort)messageId));
            ushort errorCode = 0;
            var errorCodePart = BitConverter.GetBytes(SwapUInt16(errorCode));
            var packageLength = (ushort)(PACKAGE_LENGTH_PART_LENGTH
                                         + PACKAGE_SEQUENCE_PART_LENGTH
                                         + PACKAGE_MESSAGE_ID_PART_LENGTH
                                         + PACKAGE_ERROR_PART_LENGTH
                                         + message.Length);
            var packageLengthPart = BitConverter.GetBytes(SwapUInt16(packageLength));
            var package = new byte[packageLength];

            var offset = 0;
            packageLengthPart.CopyTo(package, 0);
            offset += PACKAGE_LENGTH_PART_LENGTH;
            sequencePart.CopyTo(package, offset);
            offset += PACKAGE_SEQUENCE_PART_LENGTH;
            messageIdPart.CopyTo(package, offset);
            offset += PACKAGE_MESSAGE_ID_PART_LENGTH;
            errorCodePart.CopyTo(package, offset);
            offset += PACKAGE_ERROR_PART_LENGTH;
            message.CopyTo(package, offset);
            return package;
        }

        public void DecodePacket(byte[] bytes, out uint errCode, out uint serialNumber, out uint messageId, out byte[] message)
        {
            var offset = 0;
            //packageLength
            var packageLengthPart = new byte[PACKAGE_LENGTH_PART_LENGTH];
            Array.Copy(bytes, offset, packageLengthPart, 0, PACKAGE_LENGTH_PART_LENGTH);
            int packageLength = SwapUInt16(BitConverter.ToUInt16(packageLengthPart, 0));
            offset += PACKAGE_LENGTH_PART_LENGTH;

            //sequence
            var sequencePart = new byte[PACKAGE_SEQUENCE_PART_LENGTH];
            Array.Copy(bytes, offset, sequencePart, 0, PACKAGE_SEQUENCE_PART_LENGTH);
            serialNumber = SwapUInt32(BitConverter.ToUInt32(sequencePart, 0));
            offset += PACKAGE_SEQUENCE_PART_LENGTH;

            //messageId
            var messageIdPart = new byte[PACKAGE_MESSAGE_ID_PART_LENGTH];
            Array.Copy(bytes, offset, messageIdPart, 0, PACKAGE_MESSAGE_ID_PART_LENGTH);
            messageId = SwapUInt16(BitConverter.ToUInt16(messageIdPart, 0));
            offset += PACKAGE_MESSAGE_ID_PART_LENGTH;

            //errorCode
            var errorCodePart = new byte[PACKAGE_ERROR_PART_LENGTH];
            Array.Copy(bytes, offset, errorCodePart, 0, PACKAGE_ERROR_PART_LENGTH);
            errCode = SwapUInt16(BitConverter.ToUInt16(errorCodePart, 0));
            offset += PACKAGE_ERROR_PART_LENGTH;

            //message
            message = new byte[packageLength - PACKAGE_LENGTH_PART_LENGTH - PACKAGE_SEQUENCE_PART_LENGTH -
                               PACKAGE_MESSAGE_ID_PART_LENGTH - PACKAGE_ERROR_PART_LENGTH];
            Array.Copy(bytes, offset, message, 0, message.Length);
        }

        public byte[] Serialize<T>(T t)
        {
            var json = JsonConvert.SerializeObject(t);
            return Encoding.UTF8.GetBytes(json);
        }

        public T Deserialize<T>(byte[] data)
        {
            var json = Encoding.UTF8.GetString(data);
            return JsonConvert.DeserializeObject<T>(json);
        }
        
        private static ushort SwapUInt16(ushort sX)
        {
            return (ushort)((sX >> 8) | (sX << 8));
        }

        private static uint SwapUInt32(uint sX)
        {
            uint result = default;
            var temp1 = (sX >> 24) | (sX << 24);
            var temp2 = ((16776960 & sX) >> 12) | ((16776960 & sX) << 12);
            result = temp1 | temp2;
            return result;
        }
    }
}
namespace Start
{
    public interface ITcpChannelHelper : IChannelHelper
    {
        /// <summary>
        /// 消息包包长部分所占长度
        /// </summary>
        uint PacketLength { get; }

        /// <summary>
        /// 获取包长度
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        uint GetPacketLength(byte[] bytes);
    }
}
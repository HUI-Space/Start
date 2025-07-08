using System.Net;

namespace Start
{
    public class UdpChannel : ChannelBase
    {
        public override EChannelType EChannelType => EChannelType.Udp;

        public UdpChannel(uint id, IPEndPoint endPoint, IChannelHelper channelHelper)
            : base(new UdpClient(id, endPoint), channelHelper)
        {
        }

        protected override void Received(byte[] bytes, long offset, long count)
        {
            PacketQueue.Enqueue(bytes);
        }
    }
}
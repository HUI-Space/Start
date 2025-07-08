using System.Net;

namespace Start
{
    public class KcpChannel : ChannelBase
    {
        public override EChannelType EChannelType => EChannelType.Kcp;
        private readonly IKcpHelper _kcpHelper;
        
        public KcpChannel(uint id, IPEndPoint endPoint, IChannelHelper channelHelper, IKcpHelper kcpHelper)
            : base(new KcpClient(id, endPoint, kcpHelper), channelHelper)
        {
            _kcpHelper = kcpHelper;
        }

        public override void Update(float elapseSeconds, float realElapseSeconds)
        {
            _kcpHelper?.Update(elapseSeconds, realElapseSeconds);
            base.Update(elapseSeconds, realElapseSeconds);
        }

        protected override void Received(byte[] bytes, long offset, long count)
        {
            PacketQueue.Enqueue(bytes);
        }
    }
}
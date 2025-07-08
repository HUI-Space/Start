using System.Net;


namespace Start.Server
{
    public class KcpServer : UdpSocket
    {
        Dictionary<int, KcpChannel> _channels = new ();

        public KcpServer(int id, EndPoint endpoint, string address, int port) : base(id, endpoint, address, port)
        {
            
        }

        protected override void OnReceived(EndPoint endpoint, byte[] buffer, long offset, long size)
        {
            IPEndPoint remoteAddress = (IPEndPoint)endpoint;
            if (buffer.Length < 4 && size < 4)
            {
                return;
            }
            int sessionId = KcpHelper.GetSessionId(buffer);
            
            if (sessionId == -1)
            {
                sessionId = KcpHelper.GenerateSessionId();
                _channels.Add(sessionId, KcpChannel.Create(0,sessionId,  remoteAddress, this));
            }
            if (_channels.TryGetValue(sessionId, out KcpChannel channel))
            {
                channel.Receive(KcpHelper.DecodePacket(buffer, size));
            }
        }

        public bool KcpSendAsync(int sessionId, IMessage message)
        {
            if (_channels.TryGetValue(sessionId, out KcpChannel channel))
            {
                EMessageID messageId = MessageBinding.GetMessageIDByType(message.GetType());
                byte[] data = MessageHelper.Serialize(message);
                byte[] buffer = new byte[4 + data.Length];
                Array.Copy(BitConverter.GetBytes((int)messageId), 0, buffer, 0, 4);
                Array.Copy(data, 0, buffer, 4, data.Length);
                channel.Send(buffer);
                return true;
            }
            return false;
        }
        

        public void Update()
        {
            foreach (KcpChannel channel in _channels.Values)
            {
                channel.Update();
            }
        }
    }
}

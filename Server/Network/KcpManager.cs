using System.Net;



namespace Start.Server
{
    public class KcpManager : ManagerBase<KcpManager>, IUpdateManger
    {
        public override int Priority => 2;

        private KcpServer _kcpServer;
        public override Task Initialize()
        {
            int id = 1;
            string address = "127.0.0.1";
            int port = 3333;
            EndPoint endpoint = new IPEndPoint(IPAddress.Parse(address), port);
            _kcpServer = new KcpServer(id, endpoint, address, port);
            _kcpServer.Start();
            return base.Initialize();
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            _kcpServer.Update();
        }

        public void SendAsync(int sessionId, IMessage message)
        {
            _kcpServer.KcpSendAsync(sessionId, message);
        }
    }
}

    
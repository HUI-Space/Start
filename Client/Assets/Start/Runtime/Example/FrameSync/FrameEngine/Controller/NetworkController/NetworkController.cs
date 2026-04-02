

namespace Start
{
    public class NetworkController : SingletonBase<NetworkController>
    {
        public bool Started { get; private set; }

        public override void Initialize()
        {
            base.Initialize();
            //SocketManager.Instance.KcpConnect();
        }
    }
}
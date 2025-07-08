


namespace Start.Server.Login ;

    public class LoginManager : ManagerBase<TimeManager>
    {
        public override int Priority => 10;

        public override Task Initialize()
        {
            EventManager.Instance.AddListener((int)EMessageType.Kcp,(int)EMessageID.Login_C2S,Login);
            return base.Initialize();
        }

        public override Task DeInitialize()
        {
            EventManager.Instance.RemoveListener((int)EMessageType.Kcp,(int)EMessageID.Login_C2S,Login);
            return base.DeInitialize();
        }

        private void Login(IGenericData genericData)
        {
            int sessionId = genericData.GetData1<int>();
            Login_C2S roomMatchC2S = genericData.GetData2<Login_C2S>();
            Login_S2C login_S2C = new Login_S2C();
            login_S2C.Session = sessionId;
            KcpManager.Instance.SendAsync(sessionId, login_S2C);
        }
    }
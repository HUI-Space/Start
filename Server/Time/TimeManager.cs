


namespace Start.Server
{
    public class TimeManager : ManagerBase<TimeManager>
    {
        // 重写Priority属性，返回值为1，表示该管理器的优先级
        public override int Priority => 1;

        public override Task Initialize()
        {
            EventManager.Instance.AddListener((int)EMessageType.Kcp,(int)EMessageID.Ping_C2S,Ping);
            return base.Initialize();
        }

        public override Task DeInitialize()
        {
            EventManager.Instance.AddListener((int)EMessageType.Kcp,(int)EMessageID.Ping_C2S,Ping);
            return base.DeInitialize();
        }

        private void Ping(IGenericData genericData)
        {
            int sessionId = genericData.GetData1<int>();
            Ping_C2S pingC2S = genericData.GetData2<Ping_C2S>();
            Ping_S2C pingS2C = new Ping_S2C();
            pingS2C.TimeStamp = TimeUtility.TimeStamp();
            KcpManager.Instance.SendAsync(sessionId, pingS2C);
        }
    }
}

    
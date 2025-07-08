using System;

namespace Start
{
    [Serializable]
    public partial class Ping_C2S : IMessage
    {

    }

    [Serializable]
    public partial class Ping_S2C : IMessage
    {
        /// <summary>
        /// 服务器时间
        /// </summary>
        public long TimeStamp { get; set; }
    }
}
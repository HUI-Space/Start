using System;

namespace Start
{
    
    public enum EMatchStatus
    {
        None = 0,
        Ready = 1,
        Waiting = 2,
        GiveUp = 3,
        GiveUpSuccess = 4,
    }
    
    [Serializable]
    public partial class RoomMatch_C2S : IMessage
    {
        public int MatchStatus { get; set; }
    }

    [Serializable]
    public partial class RoomMatch_S2C : IMessage
    {
        public int MatchStatus { get; set; }
    }

    [Serializable]
    public partial class RoomMatchSuccess_S2C : IMessage
    {
        public int PlayerId { get; set; }
        
        public int RoomId { get; set; }
    }
    
    /// <summary>
    /// 房间准备
    /// </summary>
    [Serializable]
    public partial class RoomReady_C2S : IMessage
    {
        public int RoomId { get; set; }
    }

    /// <summary>
    /// 房间开始
    /// </summary>
    [Serializable]
    public partial class RoomStart_S2C : IMessage
    {
        public long StartTime { get; set; }
    }
    
    [Serializable]
    public partial class RoomFrame_C2S : IMessage
    {
        
        public int RoomId { get; set; }
        
        /// <summary>
        /// 权威帧
        /// </summary>
        public int AuthorityFrame { get; set; }
        
        /// <summary>
        /// 帧输入
        /// </summary>
        public uint FrameInput { get; set; }
    }
    
    [Serializable]
    public partial class RoomFrame_S2C : IMessage
    {
        /// <summary>
        /// 权威帧
        /// </summary>
        public int AuthorityFrame { get; set; }

        /// <summary>
        /// FrameInput 数组
        /// </summary>
        public uint[] FrameInput { get; set; }
    }

    [Serializable]
    public partial class RoomUpdateTime_S2C : IMessage
    {
        public long Time { get; set; }
    }
    
    [Serializable]
    public partial class RoomCheckHash_S2C : IMessage
    {
        
    }

    [Serializable]
    public partial class RoomCheckHashFail_S2C : IMessage
    {
        
    }

    [Serializable]
    public partial class Reconnect_S2C : IMessage
    {
        
    }
}
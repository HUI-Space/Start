using System.Collections.Generic;

namespace Start
{
    public class BattleData : IReusable
    {
        /// <summary>
        /// 战斗类型
        /// </summary>
        public EBattleType BattleType
        {
            get;
            set;
        }
        
        /// <summary>
        /// 帧间隔
        /// </summary>
        public int FrameInterval { get; set; }

        public List<int> Player = new List<int>();
        public static BattleData Create()
        {
            BattleData battleData = RecyclableObjectPool.Acquire<BattleData>();
            return battleData;
        }
        
        public void Reset()
        {
            
        }
    }
}
using System.Collections.Generic;


namespace Start
{
    public partial class MatchEntity : IReference
    {
        /// <summary>
        /// 当前MatchEntity 帧
        /// </summary>
        public int AuthorityFrame = -1;
        
        /// <summary>
        /// 时间差或帧间隔时间
        /// </summary>
        public FixedPointNumber DeltaTime;
        
        /// <summary>
        /// 时间缩放
        /// </summary>
        public FixedPointNumber TimeScale;
        
        public List<PlayerEntity> PlayerList = new List<PlayerEntity>();

        public static MatchEntity Copy(MatchEntity matchEntity)
        {
            MatchEntity newMatchEntity = ReferencePool.Acquire<MatchEntity>();
            //TODO 把 matchEntity 复制到 newMatchEntity
            
            return newMatchEntity;
        }
        
        public void Clear()
        {
            
        }
    }
}
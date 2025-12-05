using System.Collections.Generic;

namespace Start
{
    public partial class MatchEntity : IReusable
    {
        /// <summary>
        /// 当前MatchEntity 帧
        /// </summary>
        public int AuthorityFrame;

        /// <summary>
        /// 时间
        /// </summary>
        public FP Time;
        
        /// <summary>
        /// 时间差或帧间隔时间
        /// </summary>
        public FP DeltaTime;
        
        /// <summary>
        /// 时间缩放
        /// </summary>
        public FP TimeScale;
        
        /// <summary>
        /// 玩家列表
        /// </summary>
        public List<PlayerEntity> PlayerList;

        #region Component
        public StateComponent State = new StateComponent();

        
        #endregion

        public static MatchEntity Copy(MatchEntity matchEntity)
        {
            MatchEntity newMatchEntity = RecyclableObjectPool.Acquire<MatchEntity>();
            newMatchEntity.AuthorityFrame = matchEntity.AuthorityFrame;
            newMatchEntity.DeltaTime = matchEntity.DeltaTime;
            newMatchEntity.TimeScale = matchEntity.TimeScale;
            newMatchEntity.PlayerList = new List<PlayerEntity>(matchEntity.PlayerList.Count);
            for (int i = 0; i < matchEntity.PlayerList.Count; i++)
            {
                PlayerEntity playerEntity = matchEntity.PlayerList[i];
                newMatchEntity.PlayerList.Add(PlayerEntity.Copy(playerEntity));
            }
            return newMatchEntity;
        }
        
        public void Reset()
        {
            AuthorityFrame = 0;
            DeltaTime = default;
            TimeScale = default;
            foreach (PlayerEntity playerEntity in PlayerList)
            {
                RecyclableObjectPool.Recycle(playerEntity);
            }
            PlayerList.Clear();
        }
    }
}
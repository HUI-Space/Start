using System.Collections.Generic;

namespace Start
{
    public class MatchEntity : IObject
    {
        /// <summary>
        /// 当前MatchEntity 帧
        /// </summary>
        public int AuthorityFrame { get; set; } = -1;

        /// <summary>
        /// 时间
        /// </summary>
        public FP Time { get; set; } = FP.Zero;

        /// <summary>
        /// 时间差或帧间隔时间
        /// </summary>
        public FP DeltaTime { get; set; } = FP.Zero;

        /// <summary>
        /// 时间缩放
        /// </summary>
        public FP TimeScale { get; set; } = FP.One;

        /// <summary>
        /// 状态组件
        /// </summary>
        public StateComponent State { get; private set; } = new StateComponent();
        
        /// <summary>
        /// 玩家列表
        /// </summary>
        public List<PlayerEntity> PlayerList { get; private set; } = new List<PlayerEntity>();
        
        /// <summary>
        /// 复制
        /// </summary>
        /// <param name="matchEntity">复制完成之后的对象</param>
        public void CopyTo(MatchEntity matchEntity)
        {
            matchEntity.AuthorityFrame = AuthorityFrame;
            matchEntity.DeltaTime = DeltaTime;
            matchEntity.TimeScale = TimeScale;
            
            for (int i = 0; i < PlayerList.Count; i++)
            {
                PlayerEntity playerEntity = PlayerList[i];
                if (i <= matchEntity.PlayerList.Count - 1)
                {
                    playerEntity.CopyTo(matchEntity.PlayerList[i]);
                }
                else
                {
                    PlayerEntity spawnPlayerEntity = MatchController.Instance.SpawnPlayerEntity(playerEntity);
                    matchEntity.PlayerList.Add(spawnPlayerEntity);
                }
            }
        }

        public void OnRelease()
        {
            AuthorityFrame = 0;
            Time = default;
            DeltaTime = default;
            TimeScale = default;
            State = null;
            PlayerList.Clear();
            PlayerList = null;
        }
    }
}
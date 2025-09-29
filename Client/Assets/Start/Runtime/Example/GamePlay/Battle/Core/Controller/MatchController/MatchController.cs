using System.Collections.Generic;

namespace Start
{
    public class MatchController : SingletonBase<MatchController>
    {
        /// <summary>
        /// 比赛实体
        /// </summary>
        public MatchEntity MatchEntity { get; private set; }

        /// <summary>
        /// 预测实体
        /// </summary>
        public MatchEntity PredictMatchEntity { get; private set; }
        /// <summary>
        /// 渲染实体
        /// </summary>
        public MatchEntity RenderMatchEntity { get; private set; }
        /// <summary>
        /// 已确认实体
        /// </summary>
        public Queue<MatchEntity> ConfirmedMatchEntity = new Queue<MatchEntity>();

        public override void Initialize()
        {
            base.Initialize();
            MatchEntity = ReferencePool.Acquire<MatchEntity>();
        }

        public override void DeInitialize()
        {
            base.DeInitialize();
            ReferencePool.Release(MatchEntity);
            
        }

        public void LogicUpdate(FrameData frame)
        {
            
        }
        
        /// <summary>
        /// 拷贝玩家输入到MatchEntity
        /// </summary>
        /// <param name="matchEntity">实体</param>
        /// <param name="frame">帧数据</param>
        public static void UpdateMatchEntity(MatchEntity matchEntity,FrameData frame)
        {
            matchEntity.AuthorityFrame = frame.AuthorityFrame;
            List<PlayerEntity> playerList = matchEntity.PlayerList;
            for (int i = 0; i < playerList.Count; i++)
            {
                PlayerEntity playerEntity = playerList[i];
                if (frame.GetInputByIndex(playerEntity.Id,out FrameInput playerInput))
                {
                    playerEntity.InputComponent.Yaw = (playerInput.Yaw & 0x1F) - 1;
                }
            }
        }

        /// <summary>
        /// MatchEntity 逻辑更新，主要是各种系统开始生效
        /// </summary>
        /// <param name="match"></param>
        public static void LogicUpdateState(MatchEntity match)
        {
            
        }
    }
}
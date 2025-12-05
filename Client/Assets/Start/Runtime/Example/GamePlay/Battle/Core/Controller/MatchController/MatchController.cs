using System.Collections.Generic;

namespace Start
{
    public class MatchController : SingletonBase<MatchController>
    {
        private MatchStateMachine _matchStateMachine = new MatchStateMachine();
        
        private PlayerStateMachine _playerStateMachine = new PlayerStateMachine();

        /// <summary>
        /// 拷贝玩家输入到MatchEntity
        /// </summary>
        /// <param name="matchEntity">实体</param>
        /// <param name="frame">帧数据</param>
        public void CopyFrameDataToMatchEntity(MatchEntity matchEntity,FrameData frame)
        {
            matchEntity.AuthorityFrame = frame.AuthorityFrame;
            List<PlayerEntity> playerList = matchEntity.PlayerList;
            for (int i = 0; i < playerList.Count; i++)
            {
                PlayerEntity playerEntity = playerList[i];
                if (frame.GetInputByIndex(playerEntity.Id,out FrameInput playerInput))
                {
                    playerEntity.Input.Yaw = (playerInput.Yaw & 0x1F) - 1;
                }
            }
        }
        
        /// <summary>
        /// MatchEntity 逻辑更新，主要是各种系统开始生效
        /// </summary>
        /// <param name="matchEntity"></param>
        public void LogicUpdateState(MatchEntity matchEntity)
        {
            // 更新玩家状态
            UpdatePlayerState(matchEntity);
            
            // 更新游戏
            _matchStateMachine.Update(matchEntity);
        }

        private void UpdatePlayerState(MatchEntity matchEntity)
        {
            List<PlayerEntity> playerList = matchEntity.PlayerList;
            
            // 更新玩家状态
            foreach (PlayerEntity playerEntity in playerList)
            {
                _playerStateMachine.OnUpdate(matchEntity, playerEntity);
            }
            
            // 晚更新
            foreach (PlayerEntity playerEntity in playerList)
            {
                _playerStateMachine.OnLateUpdate(matchEntity, playerEntity);
            }
            
            // 状态切换
            foreach (PlayerEntity playerEntity in playerList)
            {
                // 状态切换
                _playerStateMachine.ChangeState(matchEntity, playerEntity);
                // 强制状态切换
                _playerStateMachine.ForceChangeState(matchEntity, playerEntity);
            }
        }
    }
}
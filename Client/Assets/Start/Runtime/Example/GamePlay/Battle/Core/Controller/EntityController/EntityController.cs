using System.Collections.Generic;

namespace Start
{
    public static class EntityController
    {
        /// <summary>
        /// 拷贝玩家输入到MatchEntity
        /// </summary>
        /// <param name="matchEntity">实体</param>
        /// <param name="frame">帧数据</param>
        public static void UpdateMatchEntity(MatchEntity matchEntity,Frame frame)
        {
            matchEntity.AuthorityFrame = frame.AuthorityFrame;
            List<PlayerEntity> playerList = matchEntity.PlayerList;
            for (int i = 0; i < playerList.Count; i++)
            {
                PlayerEntity playerEntity = playerList[i];
                if (frame.GetPlayerInputByIndex(playerEntity.Id,out PlayerInput playerInput))
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
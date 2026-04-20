namespace Start
{
    /// <summary>
    /// 比赛系统系统
    /// 目的是修改比赛的各种状态逻辑
    /// 例如：以后有相关的状态逻辑时，只需要在这里添加即可
    /// </summary>
    public static class MatchSystem
    {
        /// <summary>
        /// 逻辑更新
        /// </summary>
        /// <param name="matchEntity">比赛实体</param>
        public static void LogicUpdate(MatchEntity matchEntity)
        {
            UpdateGiveUp(matchEntity);
        }

        /// <summary>
        /// 放弃
        /// </summary>
        private static void UpdateGiveUp(MatchEntity matchEntity)
        {
            for (int i = 0; i < matchEntity.PlayerList.Count; i++)
            {
                if (BattleManager.Instance.BattleType == EBattleType.Remote)
                {
                    if ((matchEntity.PlayerList[i].Input.GiveUp & 1) != 0)
                    {
                        //TODO
                    } 
                }
                else
                {
                    if ((matchEntity.PlayerList[i].Input.GiveUp & 1) != 0)
                    {
                        matchEntity.State.NextState = (int)EMatchState.End;
                    }
                }
            }
        }
    }
}
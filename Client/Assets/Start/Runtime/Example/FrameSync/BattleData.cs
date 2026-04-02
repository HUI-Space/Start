using System.Collections.Generic;

namespace Start
{
    public class BattleData : IRecycle
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
        
        /// <summary>
        /// 场景名称
        /// </summary>
        public string SceneName { get; set; }
        
        /// <summary>
        /// 玩家数据
        /// </summary>
        public List<PlayerData> PlayerData = new List<PlayerData>();
        
        public static BattleData Create()
        {
            BattleData battleData = RecyclablePool.Acquire<BattleData>();
            return battleData;
        }
        
        public void Recycle()
        {
            BattleType = EBattleType.None;
            FrameInterval = 0;
            SceneName = null;
            foreach (PlayerData playerDate in PlayerData)
            {
                RecyclablePool.Recycle(playerDate);
            }
            PlayerData.Clear();
        }
    }

    public class PlayerData : IRecycle
    {
        /// <summary>
        /// 玩家ID
        /// </summary>
        public int Id { get; set; }
        
        /// <summary>
        /// 玩家名称
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// 模型路径
        /// </summary>
        public string ModelPath { get; set; }
        
        public void Recycle()
        {
            Id = 0;
            Name = null;
            ModelPath = null;
        }
    }
}
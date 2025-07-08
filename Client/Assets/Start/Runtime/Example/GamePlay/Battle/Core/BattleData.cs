

namespace Start
{
    public class BattleData : IReference
    {
        public int SelfPlayerId;

        public static BattleData Create()
        {
            BattleData battleData = ReferencePool.Acquire<BattleData>();
            return battleData;
        }
        
        public void Clear()
        {
            
        }
    }
}
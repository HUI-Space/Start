namespace Start
{
    public class LocalBattleController : SingletonBase<LocalBattleController>,  IBattleController
    {
        public bool Paused { get; }
        public float Time { get; }



        public void StartBattle(BattleData battleData)
        {
            
        }
        
        public void LogicUpdate()
        {
            throw new System.NotImplementedException();
        }

        public void RenderUpdate()
        {
            throw new System.NotImplementedException();
        }

        public void Pause()
        {
            throw new System.NotImplementedException();
        }

        public void Resume()
        {
            throw new System.NotImplementedException();
        }


    }
}
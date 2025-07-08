

namespace Start
{
    public partial class BattleManager : ManagerBase<BattleManager> ,IUpdateManger
    {
        public override int Priority => 999999999;


        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            RenderUpdate();
        }
    }

    #region Framework

    public partial class BattleManager
    {
        public void StartBattle(EBattleType battleType, BattleData battleData)
        {
            switch (battleType)
            {
                case EBattleType.Local:
                    StartLocalBattle(battleData);
                    break;
                case EBattleType.Remote:
                    StartRemoteBattle(battleData);
                    break;
            }
        }

        public void StopBattle()
        {
            
        }

        public void StopEngine()
        {
            FrameEngine.StopEngine();
        }
        
        public void Pause()
        {
            FrameEngine.Pause();
        }

        public void Resume()
        {
            FrameEngine.Resume();
        }
        
        private void RenderUpdate()
        {
            FrameEngine.RenderUpdate();
        }
    }

    #endregion

    #region RemoteBattleController
    public partial class BattleManager
    {
        public void StartRemoteBattle(BattleData battleData)
        {
            FixedPointNumber frameInterval = new FixedPointNumber(BattleConst.FrameInterval,1000);
            RemoteBattleController.Instance.StartBattle(battleData);
            FrameEngine.StartEngine(frameInterval,RemoteBattleLogicUpdate,RemoteBattleNetworkUpdate,RemoteBattleRenderUpdate);
        }
        
        private void RemoteBattleLogicUpdate()
        {
            RemoteBattleController.Instance.LogicUpdate();
        }
        
        private void RemoteBattleNetworkUpdate()
        {
            RemoteBattleController.Instance.NetworkUpdate();
        }

        private void RemoteBattleRenderUpdate()
        {
            RemoteBattleController.Instance.RenderUpdate();
        }
    }
    #endregion
    
    #region LocalBattleController
    public partial class BattleManager
    {
        public void StartLocalBattle(BattleData battleData)
        {
            FixedPointNumber frameInterval = new FixedPointNumber(BattleConst.FrameInterval,1000);
            RemoteBattleController.Instance.StartBattle(battleData);
            FrameEngine.StartEngine(frameInterval,LocalLogicUpdate,LocalBattleRenderUpdate);
        }
        
        private void LocalLogicUpdate()
        {
            LocalBattleController.Instance.LogicUpdate();
        }
        
        private void LocalBattleRenderUpdate()
        {
            LocalBattleController.Instance.RenderUpdate();
        }
    }
    #endregion
    
}
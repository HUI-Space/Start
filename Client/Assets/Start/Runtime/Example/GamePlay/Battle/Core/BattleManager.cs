

namespace Start
{
    public partial class BattleManager : ManagerBase<BattleManager> ,IUpdateManger
    {
        public override int Priority => 999999999;

        private IBattleFrameEngine _battleFrameEngine;

        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            RenderUpdate();
        }
        
        public void StartEngine(EBattleType battleType, BattleData battleData)
        {
            switch (battleType)
            {
                case EBattleType.Local:
                    _battleFrameEngine = new LocalBattleFrameEngine();
                    break;
                case EBattleType.Remote:
                    _battleFrameEngine = new RemoteBattleFrameEngine();
                    break;
            }
            _battleFrameEngine.StartEngine(battleType,battleData);
        }

        public void StopEngine()
        {
            _battleFrameEngine?.StopEngine();
        }
        
        public void Pause()
        {
            _battleFrameEngine?.Pause();
        }

        public void Resume()
        {
            _battleFrameEngine?.Resume();
        }
        
        private void RenderUpdate()
        {
            _battleFrameEngine?.RenderEngineUpdate();
        }
    }
}
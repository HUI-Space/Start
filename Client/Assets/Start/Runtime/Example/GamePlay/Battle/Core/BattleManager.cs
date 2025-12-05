using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Start
{
    public partial class BattleManager : ManagerBase<BattleManager> ,IUpdateManger
    {
        public override int Priority => 999999999;

        private IBattleFrameEngine _battleFrameEngine;

        public override Task Initialize()
        {
            Application.quitting += OnApplicationQuit; // 
            return base.Initialize();
        }
        
        private void OnApplicationQuit()
        {
            StopEngine();
        }
        
        public override Task DeInitialize()
        {
            Application.quitting -= OnApplicationQuit; // 
            return base.DeInitialize();
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            RenderUpdate();
        }

        public async void StartBattle(BattleData battleData)
        {
            //加载场景
            AsyncOperationHandle<Scene> asyncOperationHandle = SceneManager.Instance.LoadSceneAsync<Scene>("BattleScene", true);
            await asyncOperationHandle.Task;
            //加载模型
            
            //加载UI
            await UIActions.OpenUI(nameof(BattlePanel));
            //初始化
            switch (battleData.BattleType)
            {
                case EBattleType.Local:
                    _battleFrameEngine = new LocalBattleFrameEngine();
                    break;
                case EBattleType.Remote:
                    _battleFrameEngine = new RemoteBattleFrameEngine();
                    break;
            }
            _battleFrameEngine.StartBattle(battleData);
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
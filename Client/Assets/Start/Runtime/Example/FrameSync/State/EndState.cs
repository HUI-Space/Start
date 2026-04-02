using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace Start
{
    public class EndState : AsyncFsmState<BattleManager>
    {
        protected override async Task OnEnter()
        {
            BattleData battleData = Fsm.GetData<BattleData>(nameof(BattleData));
            await UIActions.ShowLoading();
            
            ReplayController.Instance.SaveReplay(BattleConst.ReplayPath);
            
            BattleManager.Instance.StopEngine();
            MatchController.Instance.DeInitializeData();
            InputController.Instance.DeInitializeData();
            ReplayController.Instance.DeInitializeData();
            RenderController.Instance.DeInitializeData();
            SingletonManager.Instance.DestroySingleton<MatchController>();
            SingletonManager.Instance.DestroySingleton<InputController>();
            SingletonManager.Instance.DestroySingleton<ReplayController>();
            SingletonManager.Instance.DestroySingleton<RenderController>();
            
            await UIActions.UpdateLoadingProgress(0.3f);
            //关闭所有UI
            await UIActions.CloseAllUI();
            await UIActions.UpdateLoadingProgress(0.6f);
            SceneManager.Instance.UnloadScene(battleData.SceneName);
            AsyncOperationHandle<Scene> asyncOperationHandle = SceneManager.Instance.LoadSceneAsync<Scene>("Assets/Asset/Scene/MainScene.unity",false);
            await asyncOperationHandle.Task;
            await UIActions.OpenUI(nameof(MainPanel));
            await UIActions.UpdateLoadingProgress(1f);
            await UIActions.HideLoading();
            FsmManager.Instance.DestroyFsm<BattleManager>();
        }
    }
}
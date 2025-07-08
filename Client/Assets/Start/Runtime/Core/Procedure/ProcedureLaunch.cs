using System.Threading.Tasks;

namespace Start
{
    /// <summary>
    /// 8.游戏开始咯
    /// </summary>
    public class ProcedureLaunch : ProcedureBase
    {
        protected override async Task OnEnter()
        {
            //初始化依赖资源
            if (GameConfig.EnableAssetbundle && ResourceHelper.ResourceLoader is ResourceLoader resourceLoader)
            {
                resourceLoader.InitializeManifest(ResourceConfig.LocalManifestPath);
            }
            
            //处理预加载
            //TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>();
            //预加载
            /*HashSet<string> preloadAssets = new HashSet<string>();
            Dictionary<string,IAsyncOperationHandle> preloadAssetHandles = new Dictionary<string, IAsyncOperationHandle>();
            foreach (var item in preloadAssets)
            {
                AsyncOperationHandle<Object> handle = ResourceManager.Instance.LoadAssetAsync<Object>(item);
                await handle.Task;
                preloadAssetHandles.Add(item,handle);
            }
            Fsm.SetData(ProcedureConst.PreloadAssets,preloadAssetHandles);*/
            
            Logger.Info("游戏开始咯");
            //AudioController.Instance.PlayBackGround();
            await UIActions.OpenUI(nameof(MainPanel));
            
            /*await TALController.Instance.LogicModule.Prepare();
            await UIActions.OpenUI(nameof(TALMenuPanel));*/
        }

        protected override void OnUpdate(float elapseSeconds, float realElapseSeconds)
        {
            base.OnUpdate(elapseSeconds, realElapseSeconds);
            TALController.Instance.LogicModule.Update();
        }
    }
}
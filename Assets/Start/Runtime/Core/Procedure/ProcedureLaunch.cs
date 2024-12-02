using System.Collections.Generic;
using System.Threading.Tasks;
using Start.Framework;
using UnityEngine;

namespace Start.Runtime
{
    /// <summary>
    /// 游戏开始咯
    /// </summary>
    public class ProcedureLaunch : ProcedureBase
    {
        protected override async Task OnEnter()
        {
            Log.Info("游戏开始咯");
            //处理延迟初始化
            
            
            //预加载
            HashSet<string> preloadAssets = new HashSet<string>();
            Dictionary<string,IAsyncOperationHandle> preloadAssetHandles = new Dictionary<string, IAsyncOperationHandle>();
            foreach (var item in preloadAssets)
            {
                AsyncOperationHandle<Object> handle = ResourceManager.Instance.LoadAssetAsync<Object>(item);
                await handle.Task;
                preloadAssetHandles.Add(item,handle);
            }
            Fsm.SetData(ProcedureConst.PreloadAssets,preloadAssetHandles);
        }
    }
}
using System;
using System.Threading.Tasks;

namespace Start.Framework
{
    [Manager]
    public class ResourceManager : ManagerBase<ResourceManager>, IUpdateManger
    {
        public override int Priority => 3;
        public static IResourceHelper ResourceHelper { get; private set; }
        public static void SetHelper(IResourceHelper resourceHelper)
        {
            ResourceHelper = resourceHelper;
            ResourceHelper.Initialize();
        }
        
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            ResourceHelper?.Update(elapseSeconds,realElapseSeconds);
        }
        
        public override Task DeInitialize()
        {
            ResourceHelper.DeInitialize();
            return base.DeInitialize();
        }
        
        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <param name="path">路径</param>
        /// <typeparam name="T">资源类型</typeparam>
        /// <returns></returns>
        public AsyncOperationHandle<T> LoadAsset<T>(string path)
        {
            return ResourceHelper.LoadAsset<T>(path);
        }
        
        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="path">路径</param>
        /// <typeparam name="T">资源类型</typeparam>
        /// <returns></returns>
        public AsyncOperationHandle<T> LoadAssetAsync<T>(string path)
        {
            return ResourceHelper.LoadAssetAsync<T>(path);
        }

        /// <summary>
        /// 同步加载场景
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="isAdditive">是否叠加</param>
        /// <typeparam name="T">场景类型</typeparam>
        /// <returns></returns>
        public AsyncOperationHandle<T> LoadScene<T>(string path,bool isAdditive = true)
        {
            return ResourceHelper.LoadScene<T>(path,isAdditive);
        }
        
        /// <summary>
        /// 异步加载场景
        /// </summary>
        /// <param name="path">路径</param>
        /// <param name="isAdditive">是否叠加</param>
        /// <typeparam name="T">场景类型</typeparam>
        /// <returns></returns>
        public AsyncOperationHandle<T> LoadSceneAsync<T>(string path,bool isAdditive = true) 
        {
            return ResourceHelper.LoadSceneAsync<T>(path,isAdditive);
        }
        
        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="handle">资源句柄</param>
        public void UnloadAsset(IAsyncOperationHandle handle)
        {
            ResourceHelper.UnloadAsset(handle);
        }
        
        /// <summary>
        /// 是否存在游戏资源
        /// </summary>
        /// <param name="assetName">资源名称</param>
        /// <returns></returns>
        public bool HasAsset(string assetName)
        {
            return ResourceHelper.HasAsset(assetName);
        }
        
        /// <summary>
        /// 是否存在资源包
        /// </summary>
        /// <param name="resourceName">资源包名称</param>
        /// <returns></returns>
        public bool HasResource(string resourceName)
        {
            return ResourceHelper.HasResource(resourceName);
        }
    }
}
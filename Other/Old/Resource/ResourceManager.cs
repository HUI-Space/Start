using System.Threading.Tasks;


namespace Start
{
    public class ResourceManager : ManagerBase<ResourceManager>, IUpdateManger
    {
        public override int Priority => 3;
        
        private IResourceHelper _resourceHelper;

        public override Task Initialize()
        {
            _resourceHelper = Helper.CreateHelper<IResourceHelper>();
            _resourceHelper.Initialize();
            return base.Initialize();
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            _resourceHelper?.Update(elapseSeconds,realElapseSeconds);
        }
        
        public override Task DeInitialize()
        {
            _resourceHelper.DeInitialize();
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
            return _resourceHelper.LoadAsset<T>(path);
        }
        
        /// <summary>
        /// 异步加载资源
        /// </summary>
        /// <param name="path">路径</param>
        /// <typeparam name="T">资源类型</typeparam>
        /// <returns></returns>
        public AsyncOperationHandle<T> LoadAssetAsync<T>(string path)
        {
            return _resourceHelper.LoadAssetAsync<T>(path);
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
            return _resourceHelper.LoadScene<T>(path,isAdditive);
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
            return _resourceHelper.LoadSceneAsync<T>(path,isAdditive);
        }
        
        /// <summary>
        /// 卸载资源
        /// </summary>
        /// <param name="handle">资源句柄</param>
        public void Unload(IAsyncOperationHandle handle)
        {
            _resourceHelper.Unload(handle);
        }
        
        /// <summary>
        /// 是否存在游戏资源
        /// </summary>
        /// <param name="assetName">资源名称</param>
        /// <returns></returns>
        public bool HasAsset(string assetName)
        {
            return _resourceHelper.HasAsset(assetName);
        }
        
    }
}
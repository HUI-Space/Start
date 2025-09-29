namespace Start
{
    /// <summary>
    /// 定义资源管理助手的接口，用于资源的初始化、更新和加载等操作。
    /// </summary>
    public interface IResourceHelper
    {
        /// <summary>
        /// 初始化资源管理助手。
        /// </summary>
        void Initialize();
    
        /// <summary>
        /// 反初始化资源管理助手。
        /// </summary>
        void DeInitialize();
    
        /// <summary>
        /// 更新资源管理助手的状态。
        /// </summary>
        /// <param name="elapseSeconds">自上次更新以来的秒数。</param>
        /// <param name="realElapseSeconds">实际经过的秒数。</param>
        void Update(float elapseSeconds, float realElapseSeconds);
    
        /// <summary>
        /// 同步加载一个资源。
        /// </summary>
        /// <typeparam name="T">要加载的资源类型。</typeparam>
        /// <param name="path">资源的路径。</param>
        /// <returns>一个异步操作句柄。</returns>
        AsyncOperationHandle<T> LoadAsset<T>(string path);
    
        /// <summary>
        /// 异步加载一个资源。
        /// </summary>
        /// <typeparam name="T">要加载的资源类型。</typeparam>
        /// <param name="path">资源的路径。</param>
        /// <returns>一个异步操作句柄。</returns>
        AsyncOperationHandle<T> LoadAssetAsync<T>(string path);
    
        /// <summary>
        /// 同步加载一个场景。
        /// </summary>
        /// <typeparam name="T">要加载的场景类型。</typeparam>
        /// <param name="path">场景的路径。</param>
        /// <param name="isAdditive">是否以附加模式加载场景，默认为true。</param>
        /// <returns>一个异步操作句柄。</returns>
        AsyncOperationHandle<T> LoadScene<T>(string path, bool isAdditive = true);
    
        /// <summary>
        /// 异步加载一个场景。
        /// </summary>
        /// <typeparam name="T">要加载的场景类型。</typeparam>
        /// <param name="path">场景的路径。</param>
        /// <param name="isAdditive">是否以附加模式加载场景，默认为true。</param>
        /// <returns>一个异步操作句柄。</returns>
        AsyncOperationHandle<T> LoadSceneAsync<T>(string path, bool isAdditive = true);
    
        /// <summary>
        /// 卸载一个资源。
        /// </summary>
        /// <param name="handle">资源的异步操作句柄。</param>
        void Unload(IAsyncOperationHandle handle);
    
        /// <summary>
        /// 检查是否拥有指定名称的资源。
        /// </summary>
        /// <param name="assetName">资源的名称。</param>
        /// <returns>如果拥有指定名称的资源，则返回true；否则返回false。</returns>
        bool HasAsset(string assetName);
    }
}
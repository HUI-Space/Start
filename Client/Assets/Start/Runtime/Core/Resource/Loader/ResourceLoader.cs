using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Start
{
    /// <summary>
    /// 进度可优化，暂时不处理吧
    /// </summary>
    public partial class ResourceLoader : IResourceLoader
    {
        // 对象池
        private IObjectPool<ResourceObject, AssetBundle> _resourcePool;

        // 缓存AssetBundle
        private readonly Dictionary<string, AssetBundle> _cachedResources = new Dictionary<string, AssetBundle>();

        // 所有的获取的句柄(资源全称作为key)
        private readonly Dictionary<string, IAsyncOperationHandle> _asyncOperationHandles = new Dictionary<string, IAsyncOperationHandle>();
        
        // 异步正在加载的Resource
        private readonly ConcurrentDictionary<string, KeyValuePair<Task<ResourceObject>, int>> _asyncLoadingResource = new ConcurrentDictionary<string, KeyValuePair<Task<ResourceObject>, int>>();
        
        
        public void Initialize()
        {
            _resourcePool = ObjectPoolManager.Instance.CreateObjectPool<ResourceObject, AssetBundle>(allowMultiSpawn: true);
        }

        public void DeInitialize()
        {
            _resourcePool = null;
        }

        public AsyncOperationHandle<T> LoadAsset<T>(string assetName)
        {
            ResourceInfo resourceInfo = GetResourceInfoByAssetName(assetName);
            if (_asyncOperationHandles.TryGetValue(assetName, out IAsyncOperationHandle asyncOperationHandle))
            {
                return LoadAsyncOperationHandle<T>(asyncOperationHandle, resourceInfo);
            }

            AsyncOperationHandle<T> handle = AsyncOperationHandle<T>.Create();
            _asyncOperationHandles.Add(assetName, handle);
            //先加载依赖资源
            handle.SetAssetName(assetName);
            handle.SetResourceName(resourceInfo.Name);
            handle.SetStatus(EAsyncOperationStatus.Processing);
            LoadResourceDepend(resourceInfo.Depends);
            //加载主资源
            ResourceObject resourceObject = LoadResource(resourceInfo);
            UnityEngine.Object asset = resourceObject.Target.LoadAsset(assetName);
            if (asset is T t)
            {
                handle.SetResult(t);
                handle.SetStatus(EAsyncOperationStatus.Succeeded);
            }
            else
            {
                handle.SetStatus(EAsyncOperationStatus.Failed);
                throw new Exception("加载资源的类型错误或加载失败！！");
            }
            return handle;
        }

        public AsyncOperationHandle<T> LoadAssetAsync<T>(string assetName)
        {
            ResourceInfo resourceInfo = GetResourceInfoByAssetName(assetName);
            if (_asyncOperationHandles.TryGetValue(assetName, out IAsyncOperationHandle asyncOperationHandle))
            {
                return LoadAsyncOperationHandle<T>(asyncOperationHandle, resourceInfo);
            }
            AsyncOperationHandle<T> handle = AsyncOperationHandle<T>.Create();
            _asyncOperationHandles.Add(assetName, handle);
            handle.SetAssetName(assetName);
            handle.SetResourceName(resourceInfo.Name);
            handle.SetStatus(EAsyncOperationStatus.Processing);

            //先加载依赖资源
            List<Task<ResourceObject>> tasks = new List<Task<ResourceObject>>();
            foreach (string depend in resourceInfo.Depends)
            {
                ResourceInfo dependResourceInfo = GetResourceInfo(depend);
                Task<ResourceObject> dependTask = LoadResourceAsync(dependResourceInfo);
                tasks.Add(dependTask);
            }

            //加载主资源
            Task<ResourceObject> task = LoadResourceAsync(resourceInfo);
            tasks.Add(task);

            //加载游戏资源
            LoadAssetAsync(tasks, handle, assetName);
            return handle;
        }

        public AsyncOperationHandle<T> LoadScene<T>(string sceneName, bool isAdditive = true)
        {
            ResourceInfo resourceInfo = GetResourceInfoByAssetName(sceneName);
            if (_asyncOperationHandles.TryGetValue(sceneName, out IAsyncOperationHandle asyncOperationHandle))
            {
                return LoadAsyncOperationHandle<T>(asyncOperationHandle, resourceInfo);
            }
            AsyncOperationHandle<T> handle = AsyncOperationHandle<T>.Create();
            _asyncOperationHandles.Add(sceneName, handle);
            //先加载依赖资源
            handle.SetAssetName(sceneName);
            handle.SetResourceName(resourceInfo.Name);
            handle.SetStatus(EAsyncOperationStatus.Processing);
            LoadResourceDepend(resourceInfo.Depends);
            //加载主资源
            LoadResource(resourceInfo);
            LoadSceneParameters parameters = new LoadSceneParameters(isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single);
            Scene scene = UnityEngine.SceneManagement.SceneManager.LoadScene(Path.GetFileNameWithoutExtension(sceneName), parameters);
            if (scene is T t)
            {
                handle.SetResult(t);
                handle.SetStatus(EAsyncOperationStatus.Succeeded);
            }
            else
            {
                handle.SetStatus(EAsyncOperationStatus.Failed);
                throw new Exception("加载资源的类型错误或加载失败！！");
            }

            return handle;
        }

        public AsyncOperationHandle<T> LoadSceneAsync<T>(string sceneName, bool isAdditive = true)
        {
            ResourceInfo resourceInfo = GetResourceInfoByAssetName(sceneName);
            if (_asyncOperationHandles.TryGetValue(sceneName, out IAsyncOperationHandle asyncOperationHandle))
            {
                return LoadAsyncOperationHandle<T>(asyncOperationHandle, resourceInfo);
            }
            AsyncOperationHandle<T> handle = AsyncOperationHandle<T>.Create();
            _asyncOperationHandles.Add(sceneName, handle);
            handle.SetAssetName(sceneName);
            handle.SetResourceName(resourceInfo.Name);
            handle.SetStatus(EAsyncOperationStatus.Processing);
            //先加载依赖资源
            List<Task<ResourceObject>> tasks = new List<Task<ResourceObject>>();
            foreach (string depend in resourceInfo.Depends)
            {
                ResourceInfo dependResourceInfo = GetResourceInfo(depend);
                Task<ResourceObject> dependTask = LoadResourceAsync(dependResourceInfo);
                tasks.Add(dependTask);
            }

            //加载主资源
            Task<ResourceObject> task = LoadResourceAsync(resourceInfo);
            tasks.Add(task);

            LoadSceneAsync(tasks, handle, sceneName);
            return handle;
        }

        public void UnloadScene(IAsyncOperationHandle handle)
        {
            if (handle == null)
            {
                throw new Exception("Unload回收的handle为空");
            }

            UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(handle.AssetName);
            UnloadAsyncOperationHandle(handle);
            // 可选：强制GC回收内存
            // TODO
            GC.Collect();
        }

        public void UnloadScene(string sceneName)
        {
            if (_asyncOperationHandles.TryGetValue(sceneName, out IAsyncOperationHandle asyncOperationHandle))
            {
                UnloadScene(asyncOperationHandle);
            }
        }

        public void Unload(IAsyncOperationHandle handle)
        {
            UnloadAsyncOperationHandle(handle);
        }

        public void Unload(string assetName)
        {
            if (_asyncOperationHandles.TryGetValue(assetName, out IAsyncOperationHandle asyncOperationHandle))
            {
                UnloadAsyncOperationHandle(asyncOperationHandle);
            }
        }

        private void UnloadAsyncOperationHandle(IAsyncOperationHandle handle)
        {
            if (handle == null)
            {
                throw new Exception("Unload回收的handle为空");
            }

            ResourceInfo info = GetResourceInfo(handle.ResourceName);
            if (info != null)
            {
                foreach (string depend in info.Depends)
                {
                    if (_cachedResources.TryGetValue(depend, out AssetBundle dependResource))
                    {
                        _resourcePool.UnSpawn(dependResource);
                        if (!_resourcePool.CanSpawn(depend))
                        {
                            _cachedResources.Remove(depend);
                        }
                    }
                }

                if (_cachedResources.TryGetValue(info.Name, out AssetBundle resource))
                {
                    _resourcePool.UnSpawn(resource);
                    if (!_resourcePool.CanSpawn(info.Name))
                    {
                        _cachedResources.Remove(info.Name);
                        _asyncOperationHandles.Remove(handle.ResourceName);
                        ReferencePool.Release(handle);
                    }
                }
                _resourcePool.ReleaseAllUnused();
            }
        }

        #region 同步

        private AsyncOperationHandle<T> LoadAsyncOperationHandle<T>(IAsyncOperationHandle asyncOperationHandle, ResourceInfo resourceInfo)
        {
            if (asyncOperationHandle is AsyncOperationHandle<T> handle)
            {
                foreach (string depend in resourceInfo.Depends)
                {
                    ResourceInfo dependResourceInfo = GetResourceInfo(depend);
                    _resourcePool.Spawn(dependResourceInfo.Name);
                }
                _resourcePool.Spawn(resourceInfo.Name);
                return handle;
            }

            throw new Exception("资源名称重复,类型不适配！！");
        }
        
        /// <summary>
        ///同步加载依赖
        /// </summary>
        /// <param name="depends">依赖</param>
        private void LoadResourceDepend(string[] depends)
        {
            foreach (string depend in depends)
            {
                ResourceInfo dependResourceInfo = GetResourceInfo(depend);
                LoadResource(dependResourceInfo);
            }
        }

        /// <summary>
        /// 同步加载Resource
        /// </summary>
        /// <param name="resourceInfo"></param>
        /// <returns></returns>
        private ResourceObject LoadResource(ResourceInfo resourceInfo)
        {
            ResourceObject resourceObject = _resourcePool.Spawn(resourceInfo.Name);
            if (resourceObject == null)
            {
                string resourcePath = ResourceConfig.GetResourcePath(resourceInfo.ResourceType, resourceInfo.Name);
                AssetBundle assetBundle = AssetBundle.LoadFromFile(resourcePath, resourceInfo.CRC, resourceInfo.Offset);
                resourceObject = ResourceObject.Create(resourceInfo.Name, assetBundle);
                _resourcePool.Register(resourceObject, true);
                _cachedResources[resourceInfo.Name] = assetBundle;
            }

            return resourceObject;
        }

        #endregion

        #region 异步

        private Task<ResourceObject> LoadResourceAsync(ResourceInfo resourceInfo)
        {
            ResourceObject resourceObject = _resourcePool.Spawn(resourceInfo.Name);
            if (resourceObject == null)
            {
                if (_asyncLoadingResource.TryGetValue(resourceInfo.Name, out KeyValuePair<Task<ResourceObject>, int> keyValuePair))
                {
                    KeyValuePair<Task<ResourceObject>, int> newValue = new KeyValuePair<Task<ResourceObject>, int>(keyValuePair.Key, keyValuePair.Value + 1);
                    _asyncLoadingResource[resourceInfo.Name] = newValue;
                    return keyValuePair.Key;
                }
                else
                {
                    Task<ResourceObject> task = LoadResourceObjectAsync(resourceInfo);
                    _asyncLoadingResource.TryAdd(resourceInfo.Name, new KeyValuePair<Task<ResourceObject>, int>(task, 0));
                    return task;
                }
            }

            return Task.FromResult(resourceObject);
        }

        private async Task<ResourceObject> LoadResourceObjectAsync(ResourceInfo resourceInfo)
        {
            string resourcePath = ResourceConfig.GetResourcePath(resourceInfo.ResourceType, resourceInfo.Name);
            AssetBundleCreateRequest assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(resourcePath, resourceInfo.CRC, resourceInfo.Offset);
            await assetBundleCreateRequest;
            ResourceObject resourceObject = ResourceObject.Create(resourceInfo.Name, assetBundleCreateRequest.assetBundle);
            _resourcePool.Register(resourceObject, true);
            if (_asyncLoadingResource.Remove(resourceInfo.Name, out var keyValuePair))
            {
                for (int i = 0; i < keyValuePair.Value; i++)
                {
                    _resourcePool.Spawn(resourceInfo.Name);
                }
            }

            _cachedResources[resourceInfo.Name] = assetBundleCreateRequest.assetBundle;
            return resourceObject;
        }

        private async void LoadAssetAsync<T>(List<Task<ResourceObject>> tasks, AsyncOperationHandle<T> handle, string assetName)
        {
            try
            {
                for (int i = 0; i < tasks.Count - 2; i++)
                {
                    await tasks[i];
                    handle.SetProgress((1f + i) / tasks.Count);
                }

                ResourceObject main = await tasks[^1];

                if (main == null || main.Target == null)
                {
                    handle.SetStatus(EAsyncOperationStatus.Failed);
                    throw new Exception("主资源加载失败");
                }

                AssetBundleRequest assetBundleRequest = main.Target.LoadAssetAsync<T>(assetName);
                await assetBundleRequest;

                if (assetBundleRequest.asset is T t)
                {
                    handle.SetResult(t);
                    handle.SetStatus(EAsyncOperationStatus.Succeeded);
                }
                else
                {
                    handle.SetStatus(EAsyncOperationStatus.Failed);
                    throw new Exception("加载资源的类型错误或加载失败！！");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"加载资源失败: {ex.Message}");
                handle.SetStatus(EAsyncOperationStatus.Failed);
            }
        }

        private async void LoadSceneAsync<T>(List<Task<ResourceObject>> tasks, AsyncOperationHandle<T> handle, string sceneName, bool isAdditive = true)
        {
            try
            {
                for (int i = 0; i < tasks.Count - 2; i++)
                {
                    await tasks[i];
                    handle.SetProgress((1f + i) / tasks.Count);
                }

                ResourceObject main = await tasks[^1];
                if (main == null || main.Target == null)
                {
                    handle.SetStatus(EAsyncOperationStatus.Failed);
                    throw new Exception("主资源加载失败");
                }

                LoadSceneParameters parameters = new LoadSceneParameters(isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single);
                AsyncOperation asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(Path.GetFileNameWithoutExtension(sceneName), parameters);
                await asyncOperation;
                Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(UnityEngine.SceneManagement.SceneManager.sceneCount - 1);

                if (scene is T t)
                {
                    handle.SetResult(t);
                    handle.SetStatus(EAsyncOperationStatus.Succeeded);
                }
                else
                {
                    handle.SetStatus(EAsyncOperationStatus.Failed);
                    throw new Exception("加载资源的类型错误或加载失败！！");
                }
            }
            catch (Exception ex)
            {
                Logger.Error($"加载资源失败: {ex.Message}");
                handle.SetStatus(EAsyncOperationStatus.Failed);
            }
        }

        #endregion
    }

    /// <summary>
    /// 依赖相关
    /// </summary>
    public partial class ResourceLoader
    {
        //依赖相关
        private readonly Dictionary<string, AssetInfo> _assetInfos = new Dictionary<string, AssetInfo>();

        //依赖相关
        private readonly Dictionary<string, ResourceInfo> _resourceInfos = new Dictionary<string, ResourceInfo>();

        /// <summary>
        /// 初始化依赖资源
        /// </summary>
        public void InitializeManifest(string path)
        {
            if (!File.Exists(path))
            {
                throw new Exception($"本地创建的依赖文件不存在，路径：{path}");
            }

            _assetInfos.Clear();
            _resourceInfos.Clear();
            Manifest manifest = SerializerUtility.DeserializeObject<Manifest>(path);
            foreach (ResourceInfo resourceInfo in manifest.Resources)
            {
                _resourceInfos[resourceInfo.Name] = resourceInfo;
            }

            foreach (AssetInfo assetInfo in manifest.Assets)
            {
                _assetInfos[assetInfo.Path] = assetInfo;
            }
        }

        public bool HasAsset(string assetName)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                throw new Exception("资源名(Asset)为空");
            }

            return _assetInfos.ContainsKey(assetName);
        }

        private AssetInfo GetAssetInfo(string assetName)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                throw new Exception("资源名(Asset)为空");
            }

            if (_assetInfos == null)
            {
                return null;
            }

            return _assetInfos.GetValueOrDefault(assetName);
        }

        private ResourceInfo GetResourceInfo(string resourceName)
        {
            if (string.IsNullOrEmpty(resourceName))
            {
                throw new Exception("资源包名(Resource)为空");
            }

            if (_resourceInfos == null)
            {
                return null;
            }

            if (_resourceInfos.TryGetValue(resourceName, out ResourceInfo resourceInfo))
            {
                return resourceInfo;
            }

            return null;
        }

        private ResourceInfo GetResourceInfoByAssetName(string assetName)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                throw new Exception("资源名(Asset)为空");
            }

            AssetInfo assetInfo = GetAssetInfo(assetName);

            if (assetInfo == null)
            {
                throw new Exception("资源(Asset)为空");
            }

            ResourceInfo resourceInfo = GetResourceInfo(assetInfo.Resource);
            if (resourceInfo == null)
            {
                throw new Exception("资源包(Resource)为空");
            }

            return resourceInfo;
        }

        //可寻址资源
        /*private readonly Dictionary<string, AssetInfo> _addressAssetInfos = new Dictionary<string, AssetInfo>();

        public AssetInfo GetAssetInfoByAddress(string address)
        {
            if (string.IsNullOrEmpty(address))
            {
                throw new Exception("Address name is invalid.");
            }

            if (_addressAssetInfos == null)
            {
                return null;
            }

            if (_addressAssetInfos.TryGetValue(address, out AssetInfo assetInfo))
            {
                return assetInfo;
            }

            return null;
        }*/
    }
}
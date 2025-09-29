using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Start
{
    /// <summary>
    /// 资源加载器
    /// </summary>
    public partial class ResourceLoader : IResourceLoader
    {
        public int LoadResourceAgentCount { get; set; } = 5;

        public HashSet<string> LoadingResourceNames { get; private set; }
        public IObjectPool<ResourceObject,AssetBundle> ResourcePool { get; private set; }
        public Dictionary<string, AssetBundle> CachedResources { get; private set; }
        public Dictionary<string, string> CachedResourceNames { get; private set; }

        private ILoadResourceHelper _iLoadResourceHelper;
        private TaskPool<LoadResourceBaseTask> _taskPool;
        private Dictionary<string, AssetInfo> _assetInfos;
        private Dictionary<string, ResourceInfo> _resourceInfos;

        public void Initialize()
        {
            CachedResources = new Dictionary<string, AssetBundle>();
            LoadingResourceNames = new HashSet<string>(StringComparer.Ordinal);
            CachedResourceNames = new Dictionary<string, string>(StringComparer.Ordinal);
            ResourcePool = ObjectPoolManager.Instance.CreateObjectPool<ResourceObject,AssetBundle>(allowMultiSpawn: true);

            _iLoadResourceHelper = (ILoadResourceHelper)Activator.CreateInstance(ResourceHelper.LoadResourceHelper);
            _taskPool = new TaskPool<LoadResourceBaseTask>();
            _assetInfos = new Dictionary<string, AssetInfo>();
            _resourceInfos = new Dictionary<string, ResourceInfo>();

            for (int i = 0; i < LoadResourceAgentCount; i++)
            {
                ILoadResourceAgentHelper loadResourceAgentHelper =
                    (ILoadResourceAgentHelper)Activator.CreateInstance(ResourceHelper.LoadResourceAgentHelper);
                _taskPool.AddAgent(new LoadResourceAgent(this, loadResourceAgentHelper));
            }
        }

        public void DeInitialize()
        {
            ObjectPoolManager.Instance.DestroyObjectPool<ResourceObject, AssetBundle>();
            CachedResources.Clear();
            CachedResourceNames.Clear();
            LoadingResourceNames.Clear();

            _assetInfos.Clear();
            _resourceInfos.Clear();
            _taskPool.DeInitialize();

            ResourcePool = null;
            CachedResources = null;
            CachedResourceNames = null;
            LoadingResourceNames = null;

            _taskPool = null;
            _iLoadResourceHelper = null;
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            _taskPool?.Update(elapseSeconds, realElapseSeconds);
        }
        

        public AsyncOperationHandle<TObject> LoadAssetAsync<TObject>(string assetName)
        {
            ResourceInfo resourceInfo = GetResourceInfoByAssetName(assetName);

            //先加载依赖资源
            string[] depends = resourceInfo.Depends;
            LoadResourceAsync(depends);

            //在加载主资源
            AsyncOperationHandle<TObject> handle = AsyncOperationHandle<TObject>.Create();
            LoadResourceTask main = LoadResourceTask.Create(resourceInfo.Name, assetName, handle);
            _taskPool.AddTask(main);
            return handle;
        }

        public AsyncOperationHandle<TObject> LoadSceneAsync<TObject>(string sceneName, bool isAdditive)
        {
            ResourceInfo resourceInfo = GetResourceInfoByAssetName(sceneName);

            //先加载依赖资源
            string[] depends = resourceInfo.Depends;
            foreach (string depend in depends)
            {
                ResourceInfo dependResourceInfo = GetResourceInfo(depend);
                LoadResourceTask LoadResourceTask = LoadResourceTask.Create(dependResourceInfo.Name);
                _taskPool.AddTask(LoadResourceTask);
            }

            //在加载主资源
            AsyncOperationHandle<TObject> handle = AsyncOperationHandle<TObject>.Create();
            LoadSceneTask main = LoadSceneTask.Create(resourceInfo.Name, sceneName, isAdditive, handle);
            _taskPool.AddTask(main);
            return handle;
        }

        public AsyncOperationHandle<TObject> LoadAsset<TObject>(string assetName)
        {
            ResourceInfo resourceInfo = GetResourceInfoByAssetName(assetName);
            AsyncOperationHandle<TObject> handle = AsyncOperationHandle<TObject>.Create();
            //先加载依赖资源
            string[] depends = resourceInfo.Depends;
            handle.SetStatus(EAsyncOperationStatus.Processing);
            LoadResource(depends);
            //在加载主资源
            string resourcePath = ResourceConfig.GetResourcePath(resourceInfo.ResourceType, resourceInfo.Name);
            ResourceObject resourceObject = ResourcePool.Spawn(resourceInfo.Name);
            if (resourceObject == null)
            {
                if (LoadingResourceNames.Add(resourceInfo.Name))
                {
                    AssetBundle resource = _iLoadResourceHelper.LoadResource(resourcePath, resourceInfo.CRC, resourceInfo.Offset);
                    CachedResources[resourceInfo.Name] = resource;
                    LoadingResourceNames.Remove(resourceInfo.Name);
                    resourceObject = ResourceObject.Create(resourceInfo.Name, resource);
                    ResourcePool.Register(resourceObject, true);
                    object asset = _iLoadResourceHelper.LoadAsset(resource, assetName);
                    handle.SetStatus(EAsyncOperationStatus.Succeeded);
                    handle.SetResourceName(resourceInfo.Name);
                    handle.SetAssetName(assetName);
                    handle.SetResult(asset);
                    CachedResourceNames.Add(resourceInfo.Name, resourcePath);
                }
                else
                {
                    handle.SetStatus(EAsyncOperationStatus.Failed);
                    Logger.Error("LoadAsset is Failed.");
                }
            }
            else
            {
                
                object asset = _iLoadResourceHelper.LoadAsset(resourceObject.Target, assetName);
                handle.SetResourceName(resourceInfo.Name);
                handle.SetAssetName(assetName);
                handle.SetResult(asset);
                handle.SetStatus(EAsyncOperationStatus.Succeeded);
            }

            return handle;
        }

        public AsyncOperationHandle<TObject> LoadScene<TObject>(string sceneName, bool isAdditive)
        {
            ResourceInfo resourceInfo = GetResourceInfoByAssetName(sceneName);

            //先加载依赖资源
            string[] depends = resourceInfo.Depends;
            AsyncOperationHandle<TObject> handle = AsyncOperationHandle<TObject>.Create();
            handle.SetStatus(EAsyncOperationStatus.Processing);
            LoadResource(depends);
            //在加载主资源
            string resourcePath = ResourceConfig.GetResourcePath(resourceInfo.ResourceType, resourceInfo.Name);
            ResourceObject resourceObject = ResourcePool.Spawn(resourceInfo.Name);
            if (resourceObject == null)
            {
                if (LoadingResourceNames.Add(resourceInfo.Name))
                {
                    AssetBundle resource = _iLoadResourceHelper.LoadResource(resourcePath, resourceInfo.CRC,
                        resourceInfo.Offset);
                    CachedResources[resourceInfo.Name] = resource;
                    LoadingResourceNames.Remove(resourceInfo.Name);
                    resourceObject = ResourceObject.Create(resourceInfo.Name, resource);
                    ResourcePool.Register(resourceObject, true);
                    
                    CachedResourceNames.Add(resourceInfo.Name, resourcePath);
                    //切换场景
                    TObject result = _iLoadResourceHelper.LoadScene<TObject>(sceneName, isAdditive);
                    handle.SetStatus(EAsyncOperationStatus.Succeeded);
                    handle.SetResourceName(resourceInfo.Name);
                    handle.SetAssetName(sceneName);
                    handle.SetResult(result);
                }
                else
                {
                    handle.SetStatus(EAsyncOperationStatus.Failed);
                    Logger.Error("LoadScene is Failed.");
                }
            }
            else
            {
                
                TObject result = _iLoadResourceHelper.LoadScene<TObject>(sceneName, isAdditive);
                handle.SetStatus(EAsyncOperationStatus.Succeeded);
                handle.SetResourceName(resourceInfo.Name);
                handle.SetAssetName(sceneName);
                handle.SetResult(result);
            }
            return handle;
        }

        public void Unload(IAsyncOperationHandle handle)
        {
            ResourceInfo info = GetResourceInfo(handle.ResourceName);
            if (info != null)
            {
                foreach (string depend in info.Depends)
                {
                    if (CachedResources.TryGetValue(depend, out AssetBundle dependResource))
                    {
                        ResourcePool.UnSpawn(dependResource);
                        if (!ResourcePool.CanSpawn(depend))
                        {
                            CachedResources.Remove(depend);
                        }
                    }
                }

                if (CachedResources.TryGetValue(info.Name, out AssetBundle resource))
                {
                    ResourcePool.UnSpawn(resource);
                    if (!ResourcePool.CanSpawn(info.Name))
                    {
                        CachedResources.Remove(info.Name);
                    }
                }
                ResourcePool.ReleaseAllUnused();
            }
            ReferencePool.Release(handle);
        }

        private void LoadResourceAsync(string[] depends)
        {
            foreach (string depend in depends)
            {
                ResourceInfo dependResourceInfo = GetResourceInfo(depend);
                LoadResourceTask LoadResourceTask = LoadResourceTask.Create(dependResourceInfo.Name);
                _taskPool.AddTask(LoadResourceTask);
            }
        }

        private void LoadResource(string[] depends)
        {
            foreach (string depend in depends)
            {
                ResourceInfo dependResourceInfo = GetResourceInfo(depend);
                ResourceObject dependResourceObject = ResourcePool.Spawn(dependResourceInfo.Name);
                if (dependResourceObject == null)
                {
                    if (!LoadingResourceNames.Contains(dependResourceInfo.Name))
                    {
                        string dependResourcePath = ResourceConfig.GetResourcePath(dependResourceInfo.ResourceType,
                            depend);
                        LoadingResourceNames.Add(dependResourceInfo.Name);
                        AssetBundle dependResource = _iLoadResourceHelper.LoadResource(dependResourcePath,
                            dependResourceInfo.CRC, dependResourceInfo.Offset);
                        CachedResources[dependResourceInfo.Name] = dependResource;
                        LoadingResourceNames.Remove(dependResourceInfo.Name);
                        dependResourceObject = ResourceObject.Create(dependResourceInfo.Name, dependResource);
                        ResourcePool.Register(dependResourceObject, true);
                        CachedResourceNames.Add(dependResourceInfo.Name, dependResourcePath);
                    }
                    else
                    {
                        throw new Exception("Depend is Loading.");
                    }
                }
            }
        }
    }

    /// <summary>
    /// 依赖相关
    /// </summary>
    public partial class ResourceLoader
    {
        /// <summary>
        /// 初始化依赖资源
        /// </summary>
        public void InitializeManifest(string path)
        {
            if (!File.Exists(path))
            {
                throw new Exception("Manifest is not exist.");
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
                throw new Exception("Path is invalid.");
            }
            return _assetInfos.ContainsKey(assetName);
        }

        private AssetInfo GetAssetInfo(string assetName)
        {
            if (string.IsNullOrEmpty(assetName))
            {
                throw new Exception("Asset name is invalid.");
            }

            if (_assetInfos == null)
            {
                return null;
            }

            if (_assetInfos.TryGetValue(assetName, out AssetInfo assetInfo))
            {
                return assetInfo;
            }

            return null;
        }

        public ResourceInfo GetResourceInfo(string resourceName)
        {
            if (string.IsNullOrEmpty(resourceName))
            {
                throw new Exception("Resource name is invalid.");
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
                throw new Exception("Path is invalid.");
            }

            AssetInfo assetInfo = GetAssetInfo(assetName);

            if (assetInfo == null)
            {
                throw new Exception("AssetInfo is invalid.");
            }

            ResourceInfo resourceInfo = GetResourceInfo(assetInfo.Resource);
            if (resourceInfo == null)
            {
                throw new Exception("ResourceInfo is invalid.");
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
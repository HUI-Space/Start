using System;
using UnityEngine;

namespace Start
{
    public class LoadResourceAgent : ITaskAgent<LoadResourceBaseTask>
    {
        public LoadResourceBaseTask Task { get; private set; }

        private ILoadResourceAgentHelper _loadResourceAgentHelper;

        private readonly ResourceLoader _resourceLoader;
        
        public LoadResourceAgent(ResourceLoader resourceLoader, ILoadResourceAgentHelper loadResourceAgentHelper)
        {
            _resourceLoader = resourceLoader;
            _loadResourceAgentHelper = loadResourceAgentHelper;
        }

        public void Initialize()
        {
            _loadResourceAgentHelper.LoadAssetComplete += OnLoadAssetComplete;
            _loadResourceAgentHelper.LoadSceneComplete += OnLoadSceneComplete;
            _loadResourceAgentHelper.LoadResourceProgress += OnLoadResourceProgress;
            _loadResourceAgentHelper.LoadResourceComplete += OnLoadResourceComplete;
            _loadResourceAgentHelper.LoadResourceStatusType += OnLoadResourceStatusType;
        }

        public EStartTaskStatus Start(LoadResourceBaseTask task)
        {
            Task = task ?? throw new Exception("Task is invalid.");
            ResourceInfo resourceInfo = _resourceLoader.GetResourceInfo(Task.ResourceName);
            if (_resourceLoader.LoadingResourceNames.Contains(resourceInfo.Name))
            {
                return EStartTaskStatus.HasToWait;
            }

            ResourceObject resourceObject = _resourceLoader.ResourcePool.Spawn(resourceInfo.Name);
            if (resourceObject != null)
            {
                if (task.IsScene)
                {
                    LoadScene();
                }
                else
                {
                    LoadAsset(resourceObject.Target);
                }

                return EStartTaskStatus.CanResume;
            }

            _resourceLoader.LoadingResourceNames.Add(resourceInfo.Name);

            if (!_resourceLoader.CachedResourceNames.TryGetValue(resourceInfo.Name, out string fullPath))
            {
                fullPath = ResourceConfig.GetResourcePath(resourceInfo.ResourceType, resourceInfo.Name);
                _resourceLoader.CachedResourceNames.Add(resourceInfo.Name, fullPath);
            }

            _loadResourceAgentHelper.LoadResource(fullPath, resourceInfo.CRC, resourceInfo.Offset);

            return EStartTaskStatus.CanResume;
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            _loadResourceAgentHelper?.Update(elapseSeconds, realElapseSeconds);
        }

        public void Reset()
        {
            _loadResourceAgentHelper.Reset();
            Task = default;
        }

        public void DeInitialize()
        {
            _loadResourceAgentHelper.LoadAssetComplete -= OnLoadAssetComplete;
            _loadResourceAgentHelper.LoadSceneComplete -= OnLoadSceneComplete;
            _loadResourceAgentHelper.LoadResourceProgress -= OnLoadResourceProgress;
            _loadResourceAgentHelper.LoadResourceComplete -= OnLoadResourceComplete;
            _loadResourceAgentHelper.LoadResourceStatusType -= OnLoadResourceStatusType;
            _loadResourceAgentHelper.Reset();
            _loadResourceAgentHelper = default;
            Task = default;
        }
        
        private void OnLoadAssetComplete(string assetName, object asset)
        {
            Task.OnLoadAssetComplete(assetName, asset);
            Task.Done = true;
        }

        private void OnLoadSceneComplete(string sceneName, object scene)
        {
            Task.OnLoadAssetComplete(sceneName, scene);
            Task.Done = true;
        }
        
        private void OnLoadResourceComplete(AssetBundle resource)
        {
            string resourceName = Task.ResourceName;
            ResourceObject resourceObject = ResourceObject.Create(resourceName, resource);
            _resourceLoader.ResourcePool.Register(resourceObject, true);
            _resourceLoader.LoadingResourceNames.Remove(resourceName);
            _resourceLoader.CachedResources[resourceName] = resource;
            Task.OnLoadResourceComplete();
            if (Task.IsScene)
            {
                LoadScene();
            }
            else
            {
                LoadAsset(resource);
            }
        }
        
        private void OnLoadResourceProgress(float progress)
        {
            Task.OnLoadResourceProgress(progress);
        }

        private void OnLoadResourceStatusType(EAsyncOperationStatus type)
        {
            Task.OnLoadResourceStatusType(type);
        }
        
        private void LoadAsset(AssetBundle resource)
        {
            if (Task is LoadResourceTask loadResourceTask)
            {
                if (!string.IsNullOrEmpty(loadResourceTask.AssetName))
                {
                    _loadResourceAgentHelper.LoadAsset(loadResourceTask.AssetName, resource);
                }
                else
                {
                    Task.Done = true;
                }
            }
        }

        private void LoadScene()
        {
            if (Task is LoadSceneTask loadSceneTask)
            {
                if (!string.IsNullOrEmpty(loadSceneTask.SceneName))
                {
                    _loadResourceAgentHelper.LoadScene(loadSceneTask.SceneName, loadSceneTask.IsAdditive);
                }
                else
                {
                    Task.Done = true;
                }
            }
        }
    }
}
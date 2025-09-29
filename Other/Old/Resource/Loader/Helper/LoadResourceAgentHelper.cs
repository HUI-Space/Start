using System;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Start
{
    public class LoadResourceAgentHelper : ILoadResourceAgentHelper
    {
        public event Action<float> LoadResourceProgress;
        public event Action<AssetBundle> LoadResourceComplete;
        public event Action<string, object> LoadAssetComplete;
        public event Action<string, object> LoadSceneComplete;
        public event Action<EAsyncOperationStatus> LoadResourceStatusType;
        
        private string _assetName;
        private string _sceneName;
        private float _lastProgress;
        private AsyncOperation _asyncOperation;
        private AssetBundleRequest _assetBundleRequest;
        private AssetBundleCreateRequest _assetBundleCreateRequest;

        public void LoadResource(string fullPath, uint crc, ulong offset)
        {
            if (LoadResourceProgress == null || LoadResourceComplete == null || LoadResourceStatusType == null)
            {
                return;
            }
            
            _assetBundleCreateRequest = AssetBundle.LoadFromFileAsync(fullPath, crc, offset);
            LoadResourceStatusType?.Invoke(EAsyncOperationStatus.Processing);
        }

        public void LoadAsset(string assetName, AssetBundle resource)
        {
            if (LoadResourceProgress == null || LoadAssetComplete == null || LoadResourceStatusType == null)
            {
                return;
            }

            _assetName = assetName;
            if (resource != null)
            {
                _assetBundleRequest = resource.LoadAssetAsync(assetName);
            }
        }

        public void LoadScene(string sceneName, bool isAdditive)
        {
            if (LoadResourceProgress == null || LoadSceneComplete == null || LoadResourceStatusType == null)
            {
                return;
            }

            _sceneName = sceneName;
            _asyncOperation = UnityEngine.SceneManagement.SceneManager.LoadSceneAsync(Path.GetFileNameWithoutExtension(sceneName),
                isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single);
        }

        public void Reset()
        {
            _assetBundleCreateRequest = default;
            _assetBundleRequest = default;
            _asyncOperation = default;
            _lastProgress = default;
            _assetName = default;
            _sceneName = default;
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            UpdateAssetBundleCreateRequest();
            UpdateAssetBundleRequest();
            UpdateAsyncOperation();
        }

        private void UpdateAssetBundleCreateRequest()
        {
            if (_assetBundleCreateRequest != null)
            {
                if (_assetBundleCreateRequest.isDone)
                {
                    AssetBundle assetBundle = _assetBundleCreateRequest.assetBundle;
                    if (assetBundle != null)
                    {
                        LoadResourceComplete?.Invoke(assetBundle);
                        LoadResourceStatusType?.Invoke(EAsyncOperationStatus.Succeeded);
                        _assetBundleCreateRequest = default;
                        _lastProgress = default;
                    }
                    else
                    {
                        LoadResourceStatusType?.Invoke(EAsyncOperationStatus.Failed);
                    }
                }
                else if (!_assetBundleCreateRequest.progress.Equals(_lastProgress))
                {
                    _lastProgress = _assetBundleCreateRequest.progress;
                    LoadResourceProgress?.Invoke(_lastProgress / 2);
                }
            }
        }

        private void UpdateAssetBundleRequest()
        {
            if (_assetBundleRequest != null)
            {
                if (_assetBundleRequest.isDone)
                {
                    object asset = _assetBundleRequest.asset;
                    if (asset != null)
                    {
                        LoadAssetComplete?.Invoke(_assetName, asset);
                        LoadResourceStatusType?.Invoke(EAsyncOperationStatus.Succeeded);
                        _assetBundleRequest = default;
                        _lastProgress = default;
                    }
                    else
                    {
                        LoadResourceStatusType?.Invoke(EAsyncOperationStatus.Failed);
                    }
                }
                else if (!_assetBundleRequest.progress.Equals(_lastProgress))
                {
                    _lastProgress = _assetBundleRequest.progress;
                    LoadResourceProgress?.Invoke(0.5f + _lastProgress / 2);
                }
            }
        }

        private void UpdateAsyncOperation()
        {
            if (_asyncOperation != null)
            {
                if (_asyncOperation.isDone)
                {
                    if (_asyncOperation.allowSceneActivation)
                    {
                        LoadSceneComplete?.Invoke(_sceneName, UnityEngine.SceneManagement.SceneManager.GetSceneAt(UnityEngine.SceneManagement.SceneManager.sceneCount - 1));
                        LoadResourceStatusType?.Invoke(EAsyncOperationStatus.Succeeded);
                        _asyncOperation = default;
                        _lastProgress = default;
                    }
                    else
                    {
                        LoadResourceStatusType?.Invoke(EAsyncOperationStatus.Failed);
                    }
                }
                else if (!_asyncOperation.progress.Equals(_lastProgress))
                {
                    _lastProgress = _asyncOperation.progress;
                    LoadResourceProgress?.Invoke(0.5f + _lastProgress / 2);
                }
            }
        }
    }
}
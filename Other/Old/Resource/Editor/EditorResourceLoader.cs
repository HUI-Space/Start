using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

#if UNITY_EDITOR

namespace Start
{
    public class EditorResourceLoader : IResourceLoader
    {
        public int LoadResourceAgentCount { get; set; }
        private string _assetPath;
        private AsyncOperation _asyncOperation;
        private IAsyncOperationHandle _asyncOperationHandle;

        public void Initialize()
        {
        }

        public AsyncOperationHandle<T> LoadAsset<T>(string path)
        {
            return Load<T>(path);
        }

        public AsyncOperationHandle<T> LoadAssetAsync<T>(string path)
        {
            return Load<T>(path);
        }

        public AsyncOperationHandle<T> LoadScene<T>(string path, bool isAdditive = true)
        {
            AsyncOperationHandle<T> asyncOperationHandle = AsyncOperationHandle<T>.Create();
            LoadSceneParameters parameters =
                new LoadSceneParameters(isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single);
            Scene scene = UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(path,
                parameters);
            asyncOperationHandle.SetResult(scene);
            return asyncOperationHandle;
        }

        public AsyncOperationHandle<T> LoadSceneAsync<T>(string path, bool isAdditive = true)
        {
            _asyncOperationHandle = AsyncOperationHandle<T>.Create();
            LoadSceneParameters parameters =
                new LoadSceneParameters(isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single);
            _asyncOperation =
                UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(path,parameters);
            _assetPath = path;
            return (AsyncOperationHandle<T>)_asyncOperationHandle;
        }

        private AsyncOperationHandle<T> Load<T>(string path)
        {
            AsyncOperationHandle<T> asyncOperationHandle = AsyncOperationHandle<T>.Create();
            object o = AssetDatabase.LoadAssetAtPath(path, typeof(T));
            asyncOperationHandle.SetResult(o);
            _assetPath = path;
            return asyncOperationHandle;
        }
        

        public void Unload(IAsyncOperationHandle handle)
        {
            ReferencePool.Release(handle);
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (_asyncOperation != null && _asyncOperationHandle != null)
            {
                if (_asyncOperation.isDone)
                {
                    if (_asyncOperation.allowSceneActivation)
                    {
                        _asyncOperationHandle?.SetResult(UnityEngine.SceneManagement.SceneManager.GetSceneAt(
                                UnityEngine.SceneManagement.SceneManager.sceneCount - 1));
                        _asyncOperationHandle = default;
                        _asyncOperation = default;
                    }
                }
                else
                {
                    _asyncOperationHandle?.SetProgress(_asyncOperation.progress);
                }
            }
        }

        public bool HasAsset(string assetName)
        {
            UnityEngine.Object obj = AssetDatabase.LoadMainAssetAtPath(assetName);
            return obj == null;
        }

        public void DeInitialize()
        {
        }
    }
}

#endif
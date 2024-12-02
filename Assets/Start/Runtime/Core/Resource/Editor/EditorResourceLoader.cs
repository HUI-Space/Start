using System;
using System.Collections.Generic;
using System.IO;
using Start.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

#if UNITY_EDITOR


namespace Start.Runtime
{
    public class EditorResourceLoader:IResourceLoader
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
            object o = AssetDatabase.LoadAssetAtPath(path,typeof(T));
            asyncOperationHandle.SetResult(path,o);
            SceneManager.LoadScene(Path.GetFileNameWithoutExtension(path),isAdditive?LoadSceneMode.Additive: LoadSceneMode.Single);
            return asyncOperationHandle;
        }

        public AsyncOperationHandle<T> LoadSceneAsync<T>(string path, bool isAdditive = true)
        {
            AssetDatabase.LoadAssetAtPath(path,typeof(T));
            _asyncOperationHandle = AsyncOperationHandle<T>.Create();
            _asyncOperation = SceneManager.LoadSceneAsync(Path.GetFileNameWithoutExtension(path),isAdditive?LoadSceneMode.Additive: LoadSceneMode.Single);
            _assetPath = path;
            return (AsyncOperationHandle<T>)_asyncOperationHandle;
        }

        private AsyncOperationHandle<T> Load<T>(string path)
        {
            AsyncOperationHandle<T> asyncOperationHandle = AsyncOperationHandle<T>.Create();
            object o = AssetDatabase.LoadAssetAtPath(path,typeof(T));
            asyncOperationHandle.SetResult(path,o);
            _assetPath = path;
            return asyncOperationHandle;
        }


        public bool HasResource(string resourceName)
        {
            throw new NotImplementedException();
        }

        public void Unload(IAsyncOperationHandle handle)
        {
            ReferencePool.Release(handle);
        }
        
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (_asyncOperation != null && _asyncOperationHandle !=null)
            {
                if (_asyncOperation.isDone)
                {
                    if (_asyncOperation.allowSceneActivation)
                    {
                        _asyncOperationHandle?.SetResult(_assetPath,SceneManager.GetSceneAt(SceneManager.sceneCount - 1));
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

        public string GetAssetBundlePath(EResourceType eResourceType, string assetBundleName)
        {
            return default;
        }

        public void DeInitialize()
        {
            
        }
    }
}
#endif
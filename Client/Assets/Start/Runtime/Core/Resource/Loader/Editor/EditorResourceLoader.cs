using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR

namespace Start
{
    public class EditorResourceLoader : IResourceLoader
    {
        private readonly Dictionary<string,(IAsyncOperationHandle,int)> _asyncOperationHandles = new Dictionary<string, (IAsyncOperationHandle,int)>();  
        
        public void Initialize()
        {
        }

        public void DeInitialize()
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

        public AsyncOperationHandle<T> LoadScene<T>(string sceneName, bool isAdditive = true)
        {
            if (_asyncOperationHandles.TryGetValue(sceneName, out (IAsyncOperationHandle,int) item))
            {
                item.Item2++;
                return item.Item1 as AsyncOperationHandle<T>;
            }
            
            AsyncOperationHandle<T> handle = AsyncOperationHandle<T>.Create();
            _asyncOperationHandles.Add(sceneName, (handle,1));
            LoadSceneParameters parameters = new LoadSceneParameters(isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single);
            Scene scene = UnityEditor.SceneManagement.EditorSceneManager.LoadSceneInPlayMode(sceneName, parameters);
            if (scene is T t)
            {
                handle.SetResult(t);
                handle.SetAssetName(sceneName);
                handle.SetStatus(EAsyncOperationStatus.Succeeded);
            }
            else
            {
                throw new Exception($"加载资源类型失败:{typeof(T)} 路径：{sceneName}");
            }
            return handle;
        }

        public AsyncOperationHandle<T> LoadSceneAsync<T>(string sceneName, bool isAdditive = true)
        {
            if (_asyncOperationHandles.TryGetValue(sceneName, out (IAsyncOperationHandle,int) item))
            {
                item.Item2++;
                return item.Item1 as AsyncOperationHandle<T>;
            }
            AsyncOperationHandle<T> handle = AsyncOperationHandle<T>.Create();
            _asyncOperationHandles.Add(sceneName, (handle,1));
            LoadScene(handle, sceneName, isAdditive);
            return handle;
        }

        private async void LoadScene<T>(AsyncOperationHandle<T> handle, string sceneName, bool isAdditive = true)
        {
            LoadSceneParameters parameters = new LoadSceneParameters(isAdditive ? LoadSceneMode.Additive : LoadSceneMode.Single);
            AsyncOperation asyncOperation = UnityEditor.SceneManagement.EditorSceneManager.LoadSceneAsyncInPlayMode(sceneName, parameters);
            await asyncOperation;
            Scene scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt(UnityEngine.SceneManagement.SceneManager.sceneCount - 1);
            if (scene is T t)
            {
                handle.SetResult(t);
                handle.SetAssetName(sceneName);
                handle.SetStatus(EAsyncOperationStatus.Succeeded);
            }
            else
            {
                throw new Exception($"加载资源类型失败:{typeof(T)} 路径：{sceneName}");
            }
        }
        
        public void UnloadScene(IAsyncOperationHandle handle)
        {
            if (_asyncOperationHandles.TryGetValue(handle.AssetName, out (IAsyncOperationHandle, int) item))
            {
                if (item.Item2 > 1)
                {
                    item.Item2--;
                }
                if (item.Item2 <= 0)
                {
                    
                    UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(handle.AssetName);
                    ReferencePool.Release(item.Item1);
                    // 可选：强制GC回收内存
                    GC.Collect();
                    _asyncOperationHandles.Remove(handle.AssetName);
                }
            }
            
        }

        public void UnloadScene(string sceneName)
        {
            if (_asyncOperationHandles.TryGetValue(sceneName, out (IAsyncOperationHandle, int) item))
            {
                UnloadScene(item.Item1);
            }
        }

        public void Unload(IAsyncOperationHandle handle)
        {
            UnLoadAsset(handle.AssetName);
        }

        public void Unload(string assetName)
        {
            UnLoadAsset(assetName);
        }

        private void UnLoadAsset(string assetName)
        {
            if (_asyncOperationHandles.TryGetValue(assetName, out (IAsyncOperationHandle, int) item))
            {
                if (item.Item2 > 1)
                {
                    item.Item2--;
                }
                if (item.Item2 <= 0)
                {
                    ReferencePool.Release(item.Item1);
                    _asyncOperationHandles.Remove(assetName);
                }
            }
        }
        
        public bool HasAsset(string assetName)
        {
            return File.Exists(assetName);
        }
        
        private AsyncOperationHandle<T> Load<T>(string path)
        {
            if (_asyncOperationHandles.TryGetValue(path, out (IAsyncOperationHandle,int) item))
            {
                item.Item2++;
                return item.Item1 as AsyncOperationHandle<T>;
            }
            else
            {
                AsyncOperationHandle<T> handle = AsyncOperationHandle<T>.Create();
                _asyncOperationHandles.Add(path, (handle,1));
                object o = AssetDatabase.LoadAssetAtPath(path, typeof(T));
                if (o is T t)
                {
                    handle.SetResult(t);
                    handle.SetAssetName(path);
                    handle.SetStatus(EAsyncOperationStatus.Succeeded);
                }
                else
                {
                    throw new Exception($"加载资源类型失败:{typeof(T)} 路径：{path}");
                }

                return handle;
            }
        }
    }
}

#endif
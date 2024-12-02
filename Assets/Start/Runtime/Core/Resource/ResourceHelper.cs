using System;
using System.IO;
using Start.Framework;
using UnityEngine;

namespace Start.Runtime
{
    public partial class ResourceHelper : IResourceHelper
    {
        public void Initialize()
        {
            ResourceLoader = new ResourceLoader();
            //ResourceLoader = new EditorResourceLoader();
            ResourceLoader.Initialize();
        }

        public void DeInitialize()
        {
            ResourceLoader.DeInitialize();
            ResourceLoader = default;
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            ResourceLoader?.Update(elapseSeconds, realElapseSeconds);
        }

        public AsyncOperationHandle<T> LoadAsset<T>(string path)
        {
            return ResourceLoader?.LoadAsset<T>(path);
        }

        public AsyncOperationHandle<T> LoadAssetAsync<T>(string path)
        {
            return ResourceLoader?.LoadAssetAsync<T>(path);
        }

        public AsyncOperationHandle<T> LoadScene<T>(string path, bool isAdditive = true)
        {
            return ResourceLoader?.LoadScene<T>(path,isAdditive);
        }

        public AsyncOperationHandle<T> LoadSceneAsync<T>(string path, bool isAdditive = true)
        {
            return ResourceLoader?.LoadSceneAsync<T>(path,isAdditive);
        }

        public void UnloadAsset(IAsyncOperationHandle handle)
        {
            ResourceLoader?.Unload(handle);
        }

        public bool HasAsset(string assetName)
        {
            return ResourceLoader.HasAsset(assetName);
        }
        
        public bool HasResource(string resourceName)
        {
            return ResourceLoader.HasResource(resourceName);
        }
    }
    
    public partial class ResourceHelper
    {
        public static IResourceLoader ResourceLoader { get; private set; }
        public static Type LoadResourceHelper => typeof(LoadResourceHelper);
        public static Type LoadResourceAgentHelper => typeof(LoadResourceAgentHelper);
        
        /// <summary>
        /// 根据资源类型和资源名称获取资源路径
        /// </summary>
        /// <param name="resourceType">资源类型</param>
        /// <param name="resourceName">资源名称</param>
        /// <returns></returns>
        public static string GetResourcePath(EResourceType resourceType, string resourceName)
        {
            return Path.Combine(GetResourcePath(resourceType), resourceName).RegularPath();
        }

        /// <summary>
        /// 根据资源类型获取资源路径
        /// </summary>
        /// <param name="resourceType"></param>
        /// <returns></returns>
        public static string GetResourcePath(EResourceType resourceType)
        {
            switch (resourceType)
            {
                case EResourceType.BuiltIn:
                    return ResourceConfig.BuiltInResourcePath;
                default:
                    return ResourceConfig.ResourcePath;
            }
        }
        
        /// <summary>
        /// 卸载AssetBundle
        /// </summary>
        /// <param name="Target">AssetBundle</param>
        public static void UnloadAssetBundle(object Target)
        {
            if (Target is AssetBundle assetBundle)
            {
                assetBundle.Unload(true);
            }
        }
    }
}
using System.IO;
using Start.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using SceneManager = UnityEngine.SceneManagement.SceneManager;

namespace Start.Runtime
{
    public class LoadResourceHelper : ILoadResourceHelper
    {
        public object LoadResource(string fullPath,uint crc, ulong offset)
        {
            return AssetBundle.LoadFromFile(fullPath,crc,offset);
        }

        public object LoadAsset(object resource, string assetName)
        {
            AssetBundle assetBundle = (AssetBundle) resource;
            return assetBundle.LoadAsset(assetName);
        }

        public T LoadScene<T>(string sceneName,bool isAdditive)
        {
            LoadSceneParameters parameters = new LoadSceneParameters(isAdditive ? LoadSceneMode.Additive: LoadSceneMode.Single);
            Scene scene = SceneManager.LoadScene(Path.GetFileNameWithoutExtension(sceneName),parameters);
            if (scene is T t)
            {
                return t;
            }
            Log.Error($"LoadScene<{typeof(T)}> failed, sceneName:{sceneName}");
            return default;
        }
    }
}
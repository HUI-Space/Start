using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Start
{
    public class LoadResourceHelper : ILoadResourceHelper
    {
        public AssetBundle LoadResource(string fullPath,uint crc, ulong offset)
        {
            return AssetBundle.LoadFromFile(fullPath,crc,offset);
        }

        public object LoadAsset(AssetBundle resource, string assetName)
        {
            return resource.LoadAsset(assetName);
        }

        public T LoadScene<T>(string sceneName,bool isAdditive)
        {
            LoadSceneParameters parameters = new LoadSceneParameters(isAdditive ? LoadSceneMode.Additive: LoadSceneMode.Single);
            Scene scene = UnityEngine.SceneManagement.SceneManager.LoadScene(Path.GetFileNameWithoutExtension(sceneName),parameters);
            if (scene is T t)
            {
                return t;
            }
            Logger.Error($"LoadScene<{typeof(T)}> failed, sceneName:{sceneName}");
            return default;
        }
    }
}
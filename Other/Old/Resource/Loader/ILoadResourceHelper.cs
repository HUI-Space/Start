using UnityEngine;

namespace Start
{
    public interface ILoadResourceHelper
    {
        AssetBundle LoadResource(string fullPath, uint crc, ulong offset);
        
        object LoadAsset(AssetBundle resource,string assetName);
        
        T LoadScene<T>(string sceneName,bool isAdditive);
    }
}
using System;
using UnityEngine;


namespace Start
{
    public interface ILoadResourceAgentHelper
    {
        event Action<float> LoadResourceProgress;
        
        event Action<AssetBundle> LoadResourceComplete;

        event Action<string,object> LoadAssetComplete;

        event Action<string,object> LoadSceneComplete;
        
        event Action<EAsyncOperationStatus> LoadResourceStatusType;
        
        void LoadResource(string fullPath, uint crc, ulong offset);

        void LoadAsset(string assetName, AssetBundle resource);

        void LoadScene(string sceneName, bool isAdditive);

        void Update(float elapseSeconds, float realElapseSeconds);

        void Reset();
    }
}
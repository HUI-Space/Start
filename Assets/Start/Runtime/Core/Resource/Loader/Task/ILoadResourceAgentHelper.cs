using System;
using Start.Framework;

namespace Start.Runtime
{
    public interface ILoadResourceAgentHelper
    {
        event Action<float> LoadResourceProgress;
        
        event Action<object> LoadResourceComplete;

        event Action<string,object> LoadAssetComplete;

        event Action<string,object> LoadSceneComplete;
        
        event Action<EAsyncOperationStatus> LoadResourceStatusType;
        
        void LoadResource(string fullPath, uint crc, ulong offset);

        void LoadAsset(string assetName, object resource);

        void LoadScene(string sceneName, bool isAdditive);

        void Update(float elapseSeconds, float realElapseSeconds);

        void Reset();
    }
}
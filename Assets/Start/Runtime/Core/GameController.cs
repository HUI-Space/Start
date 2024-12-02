using System;
using Start.Framework;

namespace Start.Runtime
{
    public class GameController 
    {
        public void Initialize()
        {
            RuntimeMessage.AddListener((int)EMessageId.GameVersionUpdated, OnGameVersionUpdated);
            RuntimeMessage.AddListener((int)EMessageId.GetLocalVersionFailure, OnGetLocalVersionFailure);
            RuntimeMessage.AddListener((int)EMessageId.GetBuiltInManifestFailure, OnGetBuiltInManifestFailure);
            RuntimeMessage.AddListener((int)EMessageId.GetBuiltInResourceFailure, OnGetBuiltInResourceFailure);
            RuntimeMessage.AddListener((int)EMessageId.DisplayUpdateResourceInfo, OnDisplayUpdateResourceInfo);
            RuntimeMessage.AddListener((int)EMessageId.GetRemoteGameClientFailure, OnGetRemoteGameClientFailure);
            RuntimeMessage.AddListener((int)EMessageId.GetRemoteResourceVersionFailure, OnGetRemoteResourceVersionFailure);
            RuntimeMessage.AddListener((int)EMessageId.GetRemoteOptionalManifestFailure, OnGetRemoteOptionalManifestFailure);
            RuntimeMessage.AddListener((int)EMessageId.GetRemoteMandatoryManifestFailure, OnGetRemoteMandatoryManifestFailure);
        }

        private void OnGetBuiltInManifestFailure(IGenericData genericData)
        {
            
        }

        private void OnGetRemoteOptionalManifestFailure(IGenericData genericData)
        {
            
        }

        private void OnGetRemoteMandatoryManifestFailure(IGenericData genericData)
        {
            
        }

        private void OnGetRemoteResourceVersionFailure(IGenericData genericData)
        {
            
        }

        private void OnDisplayUpdateResourceInfo(IGenericData genericData)
        {
            RuntimeMessage.SendMessage((int)EMessageId.ConfirmUpdateResource, genericData);
        }

        private void OnGetBuiltInResourceFailure(IGenericData genericData)
        {
            
        }

        private void OnGetRemoteGameClientFailure(IGenericData genericData)
        {
            
        }

        private void OnGetLocalVersionFailure(IGenericData genericData)
        {
            
        }

        private void OnGameVersionUpdated(IGenericData genericData)
        {
            
        }

        public void DeInitialize()
        {
            
        }
    }
}
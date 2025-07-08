namespace Start
{
    public static class GameController
    {
        static GameController()
        {
            RuntimeEvent.AddListener((int)EMessageId.GameVersionUpdated, OnGameVersionUpdated);
            RuntimeEvent.AddListener((int)EMessageId.GetLocalVersionFailure, OnGetLocalVersionFailure);
            RuntimeEvent.AddListener((int)EMessageId.GetBuiltInManifestFailure, OnGetBuiltInManifestFailure);
            RuntimeEvent.AddListener((int)EMessageId.GetBuiltInResourceFailure, OnGetBuiltInResourceFailure);
            RuntimeEvent.AddListener((int)EMessageId.DisplayUpdateResourceInfo, OnDisplayUpdateResourceInfo);
            RuntimeEvent.AddListener((int)EMessageId.GetRemoteGameClientFailure, OnGetRemoteGameClientFailure);
            RuntimeEvent.AddListener((int)EMessageId.GetRemoteResourceVersionFailure,
                OnGetRemoteResourceVersionFailure);
            RuntimeEvent.AddListener((int)EMessageId.GetRemoteOptionalManifestFailure,
                OnGetRemoteOptionalManifestFailure);
            RuntimeEvent.AddListener((int)EMessageId.GetRemoteMandatoryManifestFailure,
                OnGetRemoteMandatoryManifestFailure);
        }

        private static void OnGetBuiltInManifestFailure(IGenericData genericData)
        {
        }

        private static void OnGetRemoteOptionalManifestFailure(IGenericData genericData)
        {
        }

        private static void OnGetRemoteMandatoryManifestFailure(IGenericData genericData)
        {
        }

        private static void OnGetRemoteResourceVersionFailure(IGenericData genericData)
        {
        }

        private static void OnDisplayUpdateResourceInfo(IGenericData genericData)
        {
            RuntimeEvent.SendMessage((int)EMessageId.ConfirmUpdateResource, genericData);
        }

        private static void OnGetBuiltInResourceFailure(IGenericData genericData)
        {
        }

        private static void OnGetRemoteGameClientFailure(IGenericData genericData)
        {
        }

        private static void OnGetLocalVersionFailure(IGenericData genericData)
        {
            long data1 = genericData.GetData1<long>();
            string data2 = genericData.GetData1<string>();
        }

        private static void OnGameVersionUpdated(IGenericData genericData)
        {
        }
    }
}
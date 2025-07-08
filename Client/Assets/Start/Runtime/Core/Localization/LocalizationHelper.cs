using System.IO;
using Common;
using UnityEngine;

namespace Start
{
    public class LocalizationHelper : ILocalizationHelper
    {
        public static ELocalization DefaultLocalization = ELocalization.Chinese;
        
        private LocalizationConfig _localLocalizationConfig;
        public bool ChangeLocalization(int LocalizationId)
        {
            if (!LoadLocalization(LocalizationId)) return false;
            return true;
        }

        private bool LoadLocalization(int LocalizationId)
        {
            string path =  AssetConfig.GetAssetPath(EAssetType.Config, $"{nameof(LocalizationConfig)}_{(ELocalization)LocalizationId}{AssetConfig.Json}");
            AsyncOperationHandle<TextAsset> handle = ResourceManager.Instance.LoadAsset<TextAsset>(path);
            if (_localLocalizationConfig != null)
            {
                _localLocalizationConfig.DeInitialize();
                _localLocalizationConfig = default;
            }
            _localLocalizationConfig = (LocalizationConfig)JsonUtility.FromJson(handle.Result.text,typeof(LocalizationConfig));
            _localLocalizationConfig.Initialize();
            ResourceManager.Instance.Unload(handle);
            return true;
        }

        public bool HasString(long key)
        {
            if (_localLocalizationConfig == null)
            {
                LoadLocalization((int)DefaultLocalization);
            }
            return _localLocalizationConfig != null && _localLocalizationConfig.ContainsId(key);
        }

        public string GetString(long key, string defaultValue = default)
        {
            if (_localLocalizationConfig == null)
            {
                LoadLocalization((int)DefaultLocalization);
            }
            if (_localLocalizationConfig != null && _localLocalizationConfig.ContainsId(key))
            {
                return _localLocalizationConfig.GetValueById(key).Text;
            }
            return defaultValue;
        }
    }
}
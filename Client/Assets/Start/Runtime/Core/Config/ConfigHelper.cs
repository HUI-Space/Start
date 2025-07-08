using System;
using UnityEngine;

namespace Start
{
    public class ConfigHelper : IConfigHelper
    {
        public IConfig LoadConfig(Type type)
        {
            string configName = type.Name;
            if (type.Name.Equals(nameof(LocalizationConfig)))
            {
                configName = $"{nameof(LocalizationConfig)}_{(ELocalization)LocalizationManager.Instance.LanguageId}";
            }
            string path = AssetConfig.GetAssetPath(EAssetType.Config, configName + AssetConfig.Json);
            AsyncOperationHandle<TextAsset> handle = ResourceManager.Instance.LoadAsset<TextAsset>(path);
            IConfig data = (IConfig)JsonUtility.FromJson(handle.Result.text, type);
            ResourceManager.Instance.Unload(handle);
            return data;
        }
    }
}
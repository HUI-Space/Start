using System;
using System.IO;
using Start.Framework;
using UnityEngine;

namespace Start.Runtime
{
    public class ConfigHelper:IConfigHelper
    {
        private const string ConfigPath = "Assets/Data/Config";
        private const string ConfigE = ".json";

        public IConfig LoadConfig(Type type)
        {
            string path = Path.Combine(ConfigPath, type.Name + ConfigE).RegularPath();
            AsyncOperationHandle<TextAsset> handle = ResourceManager.Instance.LoadAsset<TextAsset>(path);
            IConfig data = (IConfig)JsonUtility.FromJson(handle.Result.text,type);
            return data;
        }
    }
}
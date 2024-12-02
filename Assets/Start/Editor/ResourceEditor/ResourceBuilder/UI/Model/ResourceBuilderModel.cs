using Start.Framework;
using UnityEditor;
using UnityEngine;

namespace Start.Editor.ResourceEditor
{
    public class ResourceBuilderModel
    {
        public ResourceBuildConfig ResourceBuildConfig;
        
        public void OnEnable()
        {
            Reload();
        }
        public void Reload()
        {
            ResourceBuildConfig = null;
            ResourceBuildConfig = EditorUtility.LoadJsonConfig<ResourceBuildConfig>(ResourcePathConfig.ResourceBuildConfigPath) ?? new ResourceBuildConfig() ;
        }
        
        public void SaveConfig()
        {
            FileUtility.WriteAllText(ResourcePathConfig.ResourceBuildConfigPath, JsonUtility.ToJson(ResourceBuildConfig,true));
            AssetDatabase.Refresh();
        }
        
        public void OnDestroy()
        {
            ResourceBuildConfig = default;
        }
    }
}
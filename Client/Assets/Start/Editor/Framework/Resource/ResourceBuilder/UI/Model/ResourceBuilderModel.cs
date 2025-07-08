using UnityEditor;
using UnityEngine;

namespace Start.Editor
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
            ResourceBuildConfig = Utility.LoadJsonConfig<ResourceBuildConfig>(ResourcePath.ResourceBuildConfigPath) ?? new ResourceBuildConfig() ;
        }
        
        public void SaveConfig()
        {
            FileUtility.WriteAllBytes(ResourcePath.ResourceBuildConfigPath, JsonUtility.ToJson(ResourceBuildConfig,true));
            AssetDatabase.Refresh();
        }
        
        public void OnDestroy()
        {
            ResourceBuildConfig = default;
        }
    }
}
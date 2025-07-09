using System.Collections.Generic;
using Sirenix.Utilities;

namespace Start.Editor
{
    public class PrepareBuiltInResourceTask : IBuildTask
    {
        public void Run(IResourceBuilder builder)
        {
            ResourceBuildConfig resourceBuildConfig =
                builder.GetData<ResourceBuildConfig>(ResourceBuilder.ResourceBuildConfig);

            HashSet<string> builtInResources = new HashSet<string>();
            if (resourceBuildConfig.ResourceModeType == EResourceModeType.Standalone)
            {
                Dictionary<string, MainResource> mainResourceData =
                    builder.GetData<Dictionary<string, MainResource>>(ResourceBuilder.MainResourceData);
                builtInResources.AddRange(mainResourceData.Keys);
            }
            else if (resourceBuildConfig.ResourceModeType == EResourceModeType.Updatable)
            {
                builtInResources.Add("Assets/Asset/UI/AudioPanel.prefab");
            }

            builder.SetData(ResourceBuilder.BuiltInAssets, builtInResources);
        }
    }
}
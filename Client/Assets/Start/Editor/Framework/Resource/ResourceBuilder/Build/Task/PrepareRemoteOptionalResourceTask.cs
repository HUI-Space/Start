using System.Collections.Generic;

namespace Start.Editor
{
    public class PrepareRemoteOptionalResourceTask:IBuildTask
    {
        public void Run(IResourceBuilder builder)
        {
            Dictionary<string, HashSet<string>> optionalResources = new Dictionary<string, HashSet<string>>()
            {
                {"1",new HashSet<string>(){"Assets/Asset/UI/AudioPanel.prefab"}},
                {"2",new HashSet<string>(){"Assets/Asset/UI/ConfigPanel.prefab"}}
            };
            builder.SetData(ResourceBuilder.OptionalAssets,optionalResources);
        }
    }
}
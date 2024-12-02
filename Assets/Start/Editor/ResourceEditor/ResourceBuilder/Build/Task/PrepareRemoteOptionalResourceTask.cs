using System.Collections.Generic;

namespace Start.Editor.ResourceEditor
{
    public class PrepareRemoteOptionalResourceTask:IBuildTask
    {
        public void Run(IResourceBuilder builder)
        {
            Dictionary<string, HashSet<string>> optionalResources = new Dictionary<string, HashSet<string>>()
            {
                {"1",new HashSet<string>(){"Assets/Data/Entities/Cube.prefab"}},
                {"2",new HashSet<string>(){"Assets/Data/Entities/Image.prefab"}}
            };
            builder.SetData(ResourceBuilder.OptionalAssets,optionalResources);
        }
    }
}
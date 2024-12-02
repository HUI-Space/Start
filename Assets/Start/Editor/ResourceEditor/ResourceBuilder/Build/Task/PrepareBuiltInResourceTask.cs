using System.Collections.Generic;

namespace Start.Editor.ResourceEditor
{
    public class PrepareBuiltInResourceTask:IBuildTask
    {
        public void Run(IResourceBuilder builder)
        {
            HashSet<string> builtInResources = new HashSet<string>()
            {
                "Assets/Data/Entities/Cube.prefab"
            };
            builder.SetData(ResourceBuilder.BuiltInAssets,builtInResources);
        }
    }
}
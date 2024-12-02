using Start.Framework;
using Start.Runtime;

namespace Start.Editor.ResourceEditor
{
    public class BuildLocalGameVersionTask:IBuildTask
    {
        public void Run(IResourceBuilder builder)
        {
            ResourceBuildConfig resourceBuildConfig = builder.GetData<ResourceBuildConfig>(ResourceBuilder.ResourceBuildConfig);
            string content = resourceBuildConfig.LocalGameVersion;
            FileUtility.WriteAllText(ResourceConfig.LocalGameVersionPath,content);
            Log.Info($"LocalGameVersion.b:   {content}");
        }
    }
}
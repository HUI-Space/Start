namespace Start.Editor
{
    public class BuildLocalGameVersionTask:IBuildTask
    {
        public void Run(IResourceBuilder builder)
        {
            ResourceBuildConfig resourceBuildConfig = builder.GetData<ResourceBuildConfig>(ResourceBuilder.ResourceBuildConfig);
            string content = resourceBuildConfig.LocalGameVersion;
            FileUtility.WriteAllBytes(ResourceConfig.LocalGameVersionPath,content);
            Logger.Info($"LocalGameVersion.b:   {content}");
        }
    }
}
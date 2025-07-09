namespace Start.Editor
{
    /// <summary>
    /// 准备配置
    /// </summary>
    public class PrepareConfigTask : IBuildTask
    {
        public void Run(IResourceBuilder builder)
        {
            Logger.Info($"ResourcePathConfig.ResourceBuildConfigPath:{ResourcePath.ResourceBuildConfigPath}");
            Logger.Info($"ResourcePathConfig.ResourceGroupConfigPath:{ResourcePath.ResourceGroupConfigPath}");
            ResourceBuildConfig resourceBuildConfig =
                Utility.LoadJsonConfig<ResourceBuildConfig>(ResourcePath.ResourceBuildConfigPath);
            ResourceGroupConfig resourceGroupConfig =
                Utility.LoadJsonConfig<ResourceGroupConfig>(ResourcePath.ResourceGroupConfigPath);
            builder.SetData(ResourceBuilder.ResourceBuildConfig, resourceBuildConfig);
            builder.SetData(ResourceBuilder.ResourceGroupConfig, resourceGroupConfig);
        }
    }
}
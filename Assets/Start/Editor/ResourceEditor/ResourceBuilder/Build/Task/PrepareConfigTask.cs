using Start.Framework;

namespace Start.Editor.ResourceEditor
{
    /// <summary>
    /// 准备配置
    /// </summary>
    public class PrepareConfigTask:IBuildTask
    {
        public void Run(IResourceBuilder builder)
        {
            Log.Info($"ResourcePathConfig.ResourceBuildConfigPath:{ResourcePathConfig.ResourceBuildConfigPath}");
            Log.Info($"ResourcePathConfig.ResourceGroupConfigPath:{ResourcePathConfig.ResourceGroupConfigPath}");
            ResourceBuildConfig resourceBuildConfig = EditorUtility.LoadJsonConfig<ResourceBuildConfig>(ResourcePathConfig.ResourceBuildConfigPath);
            ResourceGroupConfig resourceGroupConfig = EditorUtility.LoadJsonConfig<ResourceGroupConfig>(ResourcePathConfig.ResourceGroupConfigPath);
            builder.SetData(ResourceBuilder.ResourceBuildConfig, resourceBuildConfig);
            builder.SetData(ResourceBuilder.ResourceGroupConfig, resourceGroupConfig);
        }
    }
}
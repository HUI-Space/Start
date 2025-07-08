using System.Collections.Generic;
using System.IO;

namespace Start.Editor
{
    /// <summary>
    /// 强制下载资源版本信息
    /// 生成RemoteResourceVersion.b文件 （文件内容为：版本号|是否存在可下载资源）
    /// </summary>
    public class BuildRemoteResourceVersionTask:IBuildTask
    {
        public void Run(IResourceBuilder builder)
        {
            ResourceBuildConfig resourceBuildConfig = builder.GetData<ResourceBuildConfig>(ResourceBuilder.ResourceBuildConfig);
            Dictionary<string, HashSet<string>> optionalAssets = builder.GetData<Dictionary<string, HashSet<string>>>(ResourceBuilder.OptionalAssets);
            string content = resourceBuildConfig.ResourceVersion;
            content += $"|{optionalAssets != null && optionalAssets.Count > 0}";
            string remoteResourceVersionPath = Path.Combine(resourceBuildConfig.OutputPath,ResourceConfig.RemoteResourceVersion);
            SerializerUtility.SerializeObject(remoteResourceVersionPath, content);
            FileUtility.WriteAllBytes(resourceBuildConfig.RemoteResourceVersionLog,content);
            Logger.Info($"MandatoryVersion.b:   {content}");
        }
    }
}
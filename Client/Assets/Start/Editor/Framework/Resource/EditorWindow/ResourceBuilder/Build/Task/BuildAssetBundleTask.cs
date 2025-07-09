using System.IO;
using UnityEditor;
using UnityEngine;

namespace Start.Editor
{
    /// <summary>
    /// 构建AssetBundle
    /// </summary>
    public class BuildAssetBundleTask:IBuildTask
    {
        public void Run(IResourceBuilder builder)
        {
            ResourceBuildConfig resourceBuildConfig = builder.GetData<ResourceBuildConfig>(ResourceBuilder.ResourceBuildConfig);
            AssetBundleBuild[] buildList = builder.GetData<AssetBundleBuild[]>(ResourceBuilder.AssetBundleBuilds);
            string workingPath = resourceBuildConfig.WorkingPath;
            if (resourceBuildConfig.BuildModeType == BuildModeType.DryRunBuild)
            {
                if (!Directory.Exists(workingPath))
                {
                    Directory.CreateDirectory(workingPath);
                }
            }
            else
            {
                if (Directory.Exists(workingPath))
                {
                    FileUtility.DeleteDirectory(workingPath);
                }
                Directory.CreateDirectory(workingPath);
            }
            
            BuildAssetBundleOptions buildAssetBundleOptions = resourceBuildConfig.GetBuildAssetBundleOptions();
            BuildTarget buildTarget = resourceBuildConfig.BuildTarget;
            AssetBundleManifest assetBundleManifest = BuildPipeline.BuildAssetBundles(workingPath, buildList, buildAssetBundleOptions, buildTarget);
            builder.SetData(ResourceBuilder.AssetBundleManifest,assetBundleManifest);
        }
    }
}
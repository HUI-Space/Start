using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Start.Editor
{
    /// <summary>
    /// 准备强制下载资源 (应该是有问题的待处理)
    /// </summary>
    public class BuildRemoteMandatoryManifestTask:IBuildTask
    {
        public void Run(IResourceBuilder builder)
        {
            ResourceBuildConfig resourceBuildConfig = builder.GetData<ResourceBuildConfig>(ResourceBuilder.ResourceBuildConfig);
            AssetBundleManifest assetBundleManifest = builder.GetData<AssetBundleManifest>(ResourceBuilder.AssetBundleManifest);
            Dictionary<string, MainResource> mainResourceData = builder.GetData<Dictionary<string, MainResource>>(ResourceBuilder.MainResourceData);
            Dictionary<string, HashSet<string>> moduleAssets = builder.GetData<Dictionary<string, HashSet<string>>>(ResourceBuilder.OptionalAssets);
            
            Manifest mandatoryManifest = new Manifest();
            
            HashSet<string> allModuleAssets = new HashSet<string>();
            foreach (var item in moduleAssets)
            {
                allModuleAssets.UnionWith(item.Value);
            }
            
            HashSet<string> allMandatoryAssetBundles = new HashSet<string>();
            foreach (KeyValuePair<string, MainResource> item in mainResourceData)
            {
                MainResource mainResource = item.Value;
                if (allModuleAssets.Contains(mainResource.Resource.Path))
                {
                    continue;
                }
                
                //创建代码加载的资源信息
                string assetBundleName = ResourceBuilderHelper.GetAssetBundleName(mainResource,resourceBuildConfig.NameType);
                AssetInfo assetInfo = new AssetInfo();
                assetInfo.Address = ResourceBuilderHelper.GetAddressName(mainResource);
                assetInfo.Path = mainResource.Resource.Path;
                assetInfo.Tags = mainResource.Tags.Split('_');
                assetInfo.Resource = assetBundleName;
                
                //添加到默认热更中
                mandatoryManifest.Assets.Add(assetInfo);
                
                //获取该资源的AssetBundle以及自身的依赖，并添加到默认包中
                string[] dependencies = assetBundleManifest.GetAllDependencies(assetBundleName);
                allMandatoryAssetBundles.Add(assetBundleName);
                allMandatoryAssetBundles.UnionWith(dependencies);
            }

            foreach (string mandatoryAssetBundle in allMandatoryAssetBundles)
            {
                //AssetBundle信息
                string assetBundleName = Path.Combine(resourceBuildConfig.WorkingPath,mandatoryAssetBundle);
                string[] dependencies = assetBundleManifest.GetAllDependencies(mandatoryAssetBundle);
                
                ResourceInfo resourceInfo = new ResourceInfo();
                resourceInfo.Name = mandatoryAssetBundle;
                resourceInfo.MD5 = HashUtility.FileMD5(assetBundleName);
                resourceInfo.Size = FileUtility.GetFileSize(assetBundleName);
                resourceInfo.CRC = ResourceBuilderHelper.GetCRCForAssetBundle(assetBundleName);
                resourceInfo.ResourceType = EResourceType.Mandatory;
                resourceInfo.Depends = dependencies;
                
                //添加到默认热更中
                mandatoryManifest.Resources.Add(resourceInfo);
                
                //拷贝热更资源到热更目录
                string source = Path.Combine(resourceBuildConfig.WorkingPath, resourceInfo.Name);
                string target = Path.Combine(resourceBuildConfig.MandatoryPath, resourceInfo.Name);
                FileUtility.CopyFile(source, target);
                Logger.Info("--------------------->Copy AssetBundle<---------------------");
                Logger.Info($"Source:				  {source}");
                Logger.Info($"Target:				  {target}");
            }
            
            //生成热更的日志文件到热更目录
            string content = UnityUtility.ToJson(mandatoryManifest);
            FileUtility.WriteAllBytes(resourceBuildConfig.MandatoryResourceManifest,content);
            
            //生成热更的日志文件到热更目录
            string mandatoryPath = Path.Combine(resourceBuildConfig.MandatoryPath, ResourceConfig.RemoteMandatoryManifest);
            SerializerUtility.SerializeObject(mandatoryPath, mandatoryManifest);
        }
    }
}
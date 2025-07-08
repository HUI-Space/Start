using System.Collections.Generic;
using UnityEngine;
using System.IO;
using Newtonsoft.Json;

namespace Start.Editor
{
    /// <summary>
    /// 生成各个模块的资源包信息
    /// 适用于某个模块需要单独下载使用
    /// </summary>
    public class BuildRemoteOptionalManifestTask:IBuildTask
    {
        public void Run(IResourceBuilder builder)
        {
            ResourceBuildConfig resourceBuildConfig = builder.GetData<ResourceBuildConfig>(ResourceBuilder.ResourceBuildConfig);
            AssetBundleManifest assetBundleManifest = builder.GetData<AssetBundleManifest>(ResourceBuilder.AssetBundleManifest);
            Dictionary<string, MainResource> mainResourceData = builder.GetData<Dictionary<string, MainResource>>(ResourceBuilder.MainResourceData);
            Dictionary<string, HashSet<string>> optionalAssets = builder.GetData<Dictionary<string, HashSet<string>>>(ResourceBuilder.OptionalAssets);

            //输出日志使用
            Dictionary<string , Manifest> optionalResourceManifests = new Dictionary<string, Manifest>();
            
            //创建对应版本的文件夹
            FileUtility.CreateDirectory(resourceBuildConfig.MandatoryPath);
            
            foreach (var item in optionalAssets)
            {
                Manifest optionalManifest = new Manifest();
                HashSet<string> optionalAssetBundles = new HashSet<string>();
                foreach (string optionalAsset in item.Value)
                {
                    if (!mainResourceData.TryGetValue(optionalAsset, out MainResource mainResource))
                    {
                        Logger.Error($"没有找到模块资源包信息,资源名称：{optionalAsset}");
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
                    optionalManifest.Assets.Add(assetInfo);
                    //获取该资源的AssetBundle以及自身的依赖，并添加到默认包中
                    string[] dependencies = assetBundleManifest.GetAllDependencies(assetBundleName);
                    optionalAssetBundles.Add(assetBundleName);
                    optionalAssetBundles.UnionWith(dependencies);
                }

                foreach (string optionalAssetBundle in optionalAssetBundles)
                {
                    //AssetBundle信息
                    string assetBundleName = Path.Combine(resourceBuildConfig.WorkingPath,optionalAssetBundle);
                    string[] dependencies = assetBundleManifest.GetAllDependencies(optionalAssetBundle);
                    
                    ResourceInfo resourceInfo = new ResourceInfo();
                    resourceInfo.Name = optionalAssetBundle;
                    resourceInfo.MD5 = HashUtility.FileMD5(assetBundleName);
                    resourceInfo.Size = FileUtility.GetFileSize(assetBundleName);
                    resourceInfo.CRC = ResourceBuilderHelper.GetCRCForAssetBundle(assetBundleName);
                    resourceInfo.ResourceType = EResourceType.Optional;
                    resourceInfo.Depends = dependencies;
                
                    //添加到默认热更中
                    optionalManifest.Resources.Add(resourceInfo);
                
                    //拷贝热更资源到热更目录
                    string source = Path.Combine(resourceBuildConfig.WorkingPath, resourceInfo.Name);
                    string target = Path.Combine(resourceBuildConfig.MandatoryPath, resourceInfo.Name);
                    FileUtility.CopyFile(source, target);
                    Logger.Info("--------------------->Copy AssetBundle<---------------------");
                    Logger.Info($"Source:				  {source}");
                    Logger.Info($"Target:				  {target}");
                }
                
                optionalResourceManifests.Add(item.Key, optionalManifest);
            }
            
            string optionalManifestPath = Path.Combine(resourceBuildConfig.MandatoryPath,ResourceConfig.RemoteOptionalManifest);
            SerializerUtility.SerializeObject(optionalManifestPath, optionalResourceManifests);
            
            //输出模块资源包信息
            string content = JsonConvert.SerializeObject(optionalResourceManifests,Formatting.Indented);
            string optionalResourceManifestPath = Path.Combine(resourceBuildConfig.ReportPath, resourceBuildConfig.OptionalResourceManifest);
            FileUtility.WriteAllBytes(optionalResourceManifestPath,content);
        }
    }
}
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Start.Editor
{
    /// <summary>
    /// 准备内置资源
    /// </summary>
    public class BuildLocalBuiltInManifestTask : IBuildTask
    {
        public void Run(IResourceBuilder builder)
        {
            //1.获取基础资源
            
            HashSet<string> builtInAssets = builder.GetData<HashSet<string>>(ResourceBuilder.BuiltInAssets);
            
            ResourceBuildConfig resourceBuildConfig = builder.GetData<ResourceBuildConfig>(ResourceBuilder.ResourceBuildConfig);
            AssetBundleManifest assetBundleManifest = builder.GetData<AssetBundleManifest>(ResourceBuilder.AssetBundleManifest);
            Dictionary<string, MainResource> mainResourceData = builder.GetData<Dictionary<string, MainResource>>(ResourceBuilder.MainResourceData);
            
            Manifest builtInManifest = new Manifest();
            HashSet<string> builtInAssetBundles = new HashSet<string>();
            
            //2.找到基础资源
            foreach (string asset in builtInAssets)
            {
                if (!mainResourceData.TryGetValue(asset, out MainResource mainResource))
                {
                    Logger.Error($"没有找到模块资源包信息,资源名称：{asset}");
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
                builtInManifest.Assets.Add(assetInfo);
                //获取该资源的AssetBundle以及自身的依赖，并添加到默认包中
                string[] dependencies = assetBundleManifest.GetAllDependencies(assetBundleName);
                builtInAssetBundles.Add(assetBundleName);
                builtInAssetBundles.UnionWith(dependencies);
            }

            
            string builtInPath = ResourceConfig.GetResourcePath(EResourceType.BuiltIn);
            //创建基础资源文件夹
            FileUtility.CreateDirectory(builtInPath);
            //删除对应版本的builtInPath文件夹
            FileUtility.DeleteDirectory(resourceBuildConfig.BuiltInPath);
            //创建对应版本的builtInPath文件夹
            FileUtility.CreateDirectory(resourceBuildConfig.BuiltInPath);
            
            //生成builtin.s整包
            string sourceFile = Path.Combine(resourceBuildConfig.BuiltInPath,ResourceConfig.LocalBuiltInResource);

            FileStream file = new FileStream(sourceFile, FileMode.Create);
            uint offset = 0;
            //是否加密
            /*var head = new byte[] { 0xAA, 0xBB, 0x10, 0x12 };
            file.Write(head, 0, head.Length);
            uint offset = (uint)head.Length;*/
            foreach (string item in builtInAssetBundles)
            {
                string assetBundleName = Path.Combine(resourceBuildConfig.WorkingPath,item);
                string[] dependencies = assetBundleManifest.GetAllDependencies(item);
                ResourceInfo resourceInfo = new ResourceInfo();
                resourceInfo.Name = item;
                resourceInfo.MD5 = HashUtility.FileMD5(assetBundleName);
                resourceInfo.Size = FileUtility.GetFileSize(assetBundleName);
                resourceInfo.Offset = offset;
                resourceInfo.CRC = ResourceBuilderHelper.GetCRCForAssetBundle(assetBundleName);
                resourceInfo.ResourceType = EResourceType.BuiltIn;
                resourceInfo.Depends = dependencies;
                
                builtInManifest.Resources.Add(resourceInfo);
                
                string source = Path.Combine(resourceBuildConfig.WorkingPath, item);
                byte[] sourceBytes = FileUtility.ReadAllBytes(source);
                file.Write(sourceBytes, 0, sourceBytes.Length);
                offset += (uint)sourceBytes.Length;
            }
            file.Close();
            
            //拷贝builtin.s到文件夹下 ：Application.streamingAssetsPath/AssetBundle
            string targetFile = Path.Combine(builtInPath,ResourceConfig.LocalBuiltInResource);
            FileUtility.CopyFile(sourceFile, targetFile);
            
            //创建配置文件
            string builtInManifestPath = Path.Combine(resourceBuildConfig.BuiltInPath, ResourceConfig.LocalBuiltInManifest);
            SerializerUtility.SerializeObject(builtInManifestPath, builtInManifest);
            
            //拷贝配置文件 到文件夹下 ：Application.streamingAssetsPath/AssetBundle
            FileUtility.CopyFile(builtInManifestPath, Path.Combine(builtInPath,ResourceConfig.LocalBuiltInManifest));
            
            //创建日志文件
            string content = UnityUtility.ToJson(builtInManifest);
            string builtInResourceManifestPath = Path.Combine(resourceBuildConfig.ReportPath, resourceBuildConfig.BuiltInResourceManifest);
            FileUtility.WriteAllBytes(builtInResourceManifestPath,content);
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace Start.Editor
{
    /// <summary>
    /// 准备资源包构建信息 TODO (待优化未递归所有资源)
    /// </summary>
    public class PrepareAssetBundleBuildTask:IBuildTask
    {
        
        public void Run(IResourceBuilder builder)
        {
            ResourceBuildConfig resourceBuildConfig = builder.GetData<ResourceBuildConfig>(ResourceBuilder.ResourceBuildConfig);
            
            List<ResourceGroup> groups = builder.GetData<ResourceGroupConfig>(ResourceBuilder.ResourceGroupConfig).Groups;
            
            //所有有效资源
            Dictionary<string, Resource> allResource = new Dictionary<string, Resource>();
            
            //所有主资源
            Dictionary<string, MainResource> allMainResource = new Dictionary<string, MainResource>();
            
            // 1.首先收集主资源
            foreach (ResourceGroup group in groups)
            {
                foreach (ResourceGroupItem groupItem in group.GroupItems)
                {
                    if (string.IsNullOrEmpty(groupItem.Path))
                    {
                        continue;
                    }
                    
                    string[] assets = Utility.FindAssets(ESearchType.All, groupItem.Path);
                    foreach (string asset in assets)
                    {
                        Resource resource = new Resource
                        (
                            asset,
                            groupItem.Path
                        );
                        MainResource mainResource = new MainResource
                        (
                            resource,
                            group.Name,
                            groupItem.Path,
                            groupItem.CollectorType, 
                            groupItem.PackType, 
                            groupItem.AddressType, 
                            group.Tags
                        );
                        allMainResource.Add(asset,mainResource);
                        allResource.Add(asset,resource);
                    }
                }
            }
            
            // 2.遍历主资源，收集所有效资源
            foreach (KeyValuePair<string, MainResource> item in allMainResource)
            {
                string mainResourcePath = item.Key;
                MainResource mainResource = item.Value;
                List<string> allDependencies = Utility.GetAllDependencies(mainResourcePath);
                foreach (string path in allDependencies)
                {
                    if (!allResource.TryGetValue(path,out Resource resource))
                    {
                        resource = new Resource(path,mainResource.ResourceGroupName);
                        allResource.Add(path,resource);
                    }
                }
            }
            
            // 3.将主资源分组
            // key 为资源的路径
            Dictionary<string,MainResource> separately = new Dictionary<string, MainResource>();
            // key 为 ResourceGroupItem 的 ResourcePath
            Dictionary<string,List<MainResource>> groupItemMain = new Dictionary<string, List<MainResource>>();
            // key 为 ResourceGroup 的 Name
            Dictionary<string,List<MainResource>> groupMain = new Dictionary<string, List<MainResource>>();
            foreach (var item in allMainResource)
            {
                if (item.Value.PackType == PackType.PackSeparately)
                {
                    separately.Add(item.Key,item.Value);
                }
                else if (item.Value.PackType == PackType.PackGroupItem)
                {
                    if (!groupItemMain.ContainsKey(item.Value.ResourceGroupItemPath))
                    {
                        groupItemMain[item.Value.ResourceGroupItemPath] = new List<MainResource>();
                    }
                    groupItemMain[item.Value.ResourceGroupItemPath].Add(item.Value);
                }
                else if (item.Value.PackType == PackType.PackGroup)
                {
                    if (!groupMain.ContainsKey(item.Value.ResourceGroupName))
                    {
                        groupMain[item.Value.ResourceGroupName] = new List<MainResource>();
                    }
                    groupMain[item.Value.ResourceGroupName].Add(item.Value);
                }
            }
            
            //4.收集共享资源 //TODO 有待优化
            HashSet<string> shareResource = new HashSet<string>();
            foreach (KeyValuePair<string, Resource> item in allResource)
            {
                if (!allMainResource.ContainsKey(item.Key))
                {
                    shareResource.Add(item.Key);
                }
            }
            
            //5.创建 AssetBundleBuild
            List<AssetBundleBuild> buildList = new List<AssetBundleBuild>();
            foreach (KeyValuePair<string, List<MainResource>> item in groupMain)
            {
                AssetBundleBuild build = new AssetBundleBuild();
                build.assetBundleName = ResourceBuilderHelper.GetAssetBundleName(item.Key,resourceBuildConfig.NameType);
                build.assetBundleVariant = string.Empty;
                HashSet<string> asset = new HashSet<string>();
                foreach (MainResource main in item.Value)
                {
                    asset.Add(main.Resource.Path);
                    foreach (string dependency in main.Resource.AllDependencies)
                    {
                        if (!shareResource.Contains(dependency) && !allMainResource.ContainsKey(dependency))
                        {
                            asset.Add(dependency);
                        }
                    }
                }
                build.assetNames = asset.ToArray();
                buildList.Add(build);
            }
            
            foreach (KeyValuePair<string, List<MainResource>> item in groupItemMain)
            {
                AssetBundleBuild build = new AssetBundleBuild();
                build.assetBundleName = ResourceBuilderHelper.GetAssetBundleName(item.Key,resourceBuildConfig.NameType);
                build.assetBundleVariant = string.Empty;
                HashSet<string> asset = new HashSet<string>();
                foreach (MainResource main in item.Value)
                {
                    asset.Add(main.Resource.Path);
                    foreach (string dependency in main.Resource.AllDependencies)
                    {
                        if (!shareResource.Contains(dependency) && !allMainResource.ContainsKey(dependency))
                        {
                            asset.Add(dependency);
                        }
                    }
                }
                build.assetNames = asset.ToArray();
                buildList.Add(build);
            }
            
            foreach (KeyValuePair<string, MainResource> item in separately)
            {
                AssetBundleBuild build = new AssetBundleBuild();
                build.assetBundleName = ResourceBuilderHelper.GetAssetBundleName(item.Key,resourceBuildConfig.NameType);
                build.assetBundleVariant = string.Empty;
                HashSet<string> asset = new HashSet<string>();
                asset.Add(item.Value.Resource.Path);
                foreach (string dependency in item.Value.Resource.AllDependencies)
                {
                    if (!shareResource.Contains(dependency) && !allMainResource.ContainsKey(dependency))
                    {
                        asset.Add(dependency);
                    }
                }
                build.assetNames = asset.ToArray();
                buildList.Add(build);
            }
            
            foreach (string path in shareResource)
            {
                AssetBundleBuild build = new AssetBundleBuild();
                build.assetBundleName = ResourceBuilderHelper.GetShareAssetBundleName(path,resourceBuildConfig.NameType);
                build.assetBundleVariant = string.Empty;
                build.assetNames = new[] { path };
                buildList.Add(build);
            }
            builder.SetData(ResourceBuilder.AssetBundleBuilds,buildList.ToArray());
            builder.SetData(ResourceBuilder.MainResourceData,allMainResource);
            builder.SetData(ResourceBuilder.ResourceData,allResource);
            builder.SetData(ResourceBuilder.ShareResourceData,shareResource);
            Print();
            
            void Print()
            {
                // 输出主资源
                
                foreach (KeyValuePair<string,MainResource> item in allMainResource)
                {
                    string mainResourcePath = item.Key;
                    MainResource mainResource = item.Value;
                
                    Logger.Info($"--------------------->{nameof(MainResource)}<---------------------");
                    Logger.Info($"{nameof(MainResource.CollectorType)}:        {mainResource.CollectorType}" );
                    Logger.Info($"{nameof(MainResource.AddressType)}:          {mainResource.AddressType}" );
                    Logger.Info($"{nameof(MainResource.PackType)}:             {mainResource.PackType}" );
                    Logger.Info($"{nameof(Resource.Path)}:                 {mainResourcePath}");
                    Logger.Info($"{nameof(MainResource.Tags)}:                 {mainResource.Tags}"); 
                    Logger.Info($"Dependencies Count:   {mainResource.Resource.AllDependencies.Count}");   
                    foreach (string path in mainResource.Resource.AllDependencies)
                    {
                        Logger.Info($"Dependencies:         {path}");   
                    }

                    Logger.Info(string.Empty);
                }
                
                // 输出所有有效资源日志
                foreach (KeyValuePair<string, Resource> item in allResource)
                {
                    Resource resource = item.Value;
                    Logger.Info($"--------------------->{nameof(Resource)}<---------------------");
                    Logger.Info($"{nameof(Resource.Path)}:                 {resource.Path}");
                    Logger.Info($"{nameof(Resource.GUID)}:                 {resource.GUID}");
                    Logger.Info($"{nameof(Resource.Type.Name)}:                 {resource.Type.Name}");
                    Logger.Info($"{nameof(Resource.Extension)}:            {resource.Extension}");
                    Logger.Info(string.Empty);
                }

                // 输出所有AssetBundle信息
                foreach (AssetBundleBuild build in buildList)
                {
                    Logger.Info($"--------------------->{nameof(AssetBundleBuild)}<---------------------");
                    Logger.Info($"{nameof(AssetBundleBuild.assetBundleName)}:      {build.assetBundleName}");
                    Logger.Info($"{nameof(AssetBundleBuild.assetBundleVariant)}:   {string.Empty}");
                    foreach (string asset in build.assetNames)
                    {
                        Logger.Info($"{nameof(AssetBundleBuild.assetNames)}:           {asset}");
                    }
                    Logger.Info(string.Empty);
                }
            }
        }
    }
}
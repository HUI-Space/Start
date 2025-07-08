using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Start.Editor
{
    public class ResourceBuilder:IResourceBuilder
    {
        public const string ResourceBuildConfig = nameof(ResourceBuildConfig);
        public const string ResourceGroupConfig = nameof(ResourceGroupConfig);
        public const string AssetBundleBuilds = nameof(AssetBundleBuilds);
        public const string AssetBundleManifest = nameof(AssetBundleManifest);
        public const string ResourceData = nameof(ResourceData);
        public const string MainResourceData = nameof(MainResourceData);
        public const string ShareResourceData = nameof(ShareResourceData);
        public const string BuiltInAssets = nameof(BuiltInAssets);
        public const string OptionalAssets = nameof(OptionalAssets);
        
        private int _totalSeconds;
        private ReportLog _reportLog;
        private Stopwatch _buildWatch;
        private Dictionary<string, IGenericData> _genericsData = new Dictionary<string, IGenericData>();

        public void Build(EResourceModeType resourceModeType)
        {
            if (resourceModeType == EResourceModeType.Standalone)
            {
                BuildStandalone();
            }
            else if (resourceModeType == EResourceModeType.Updatable)
            {
                BuildUpdatable();
            }
        }
        
        /// <summary>
        /// 构建资源（单机模式）
        /// </summary>
        private void BuildStandalone()
        {
            List<IBuildTask> buildTasks = new List<IBuildTask>()
            {
                new PrepareConfigTask(),
                new PrepareAssetBundleBuildTask(),
                new PrepareBuiltInResourceTask(),
                new BuildAssetBundleTask(),
                new BuildLocalBuiltInManifestTask(),
                new BuildLocalGameVersionTask()
            };
            Run(buildTasks);
        }
        
        
        /// <summary>
        /// 构建资源（热更新模式）
        /// </summary>
        private void BuildUpdatable()
        {
            List<IBuildTask> buildTasks = new List<IBuildTask>()
            {
                new PrepareConfigTask(),
                new PrepareAssetBundleBuildTask(),
                new PrepareBuiltInResourceTask(),
                new PrepareRemoteOptionalResourceTask(),
                new BuildAssetBundleTask(),
                new BuildLocalBuiltInManifestTask(),
                new BuildRemoteOptionalManifestTask(),
                new BuildRemoteMandatoryManifestTask(),
                new BuildRemoteResourceVersionTask(),
                new BuildLocalGameVersionTask(),
            };

            Run(buildTasks);
        }

        public void BuildUpdatablePatch()
        {
            List<IBuildTask> buildTasks = new List<IBuildTask>()
            {
                new PrepareConfigTask(),
                new PrepareAssetBundleBuildTask(),
                new PrepareRemoteOptionalResourceTask(),
                
                new BuildAssetBundleTask(),
                
                new BuildRemoteOptionalManifestTask(),
                new BuildRemoteMandatoryManifestTask(),
                new BuildRemoteResourceVersionTask(),
            };

            Run(buildTasks);
        }
        
        private void Run(List<IBuildTask> buildTasks)
        {
            _totalSeconds = 0;
            _buildWatch = new Stopwatch();
            _reportLog = ReportLog.Create();
            Logger.AddLogHelper(_reportLog);
            for (int i = 0; i < buildTasks.Count; i++)
            {
                IBuildTask buildTask = buildTasks[i];
                string name = buildTask.GetType().Name;
                Logger.Info($"--------------------------------------------->{name} Start<--------------------------------------------");
                _buildWatch.Start();
                buildTask.Run(this);
                _buildWatch.Stop();
                
                int seconds = GetBuildSeconds();
                _totalSeconds += seconds;
                _buildWatch.Reset();
                Logger.Info($"{name} It takes {seconds} seconds in total");
                Logger.Info($"--------------------------------------------->{name} End <--------------------------------------------");
                Logger.Info(string.Empty);
            }
            Logger.Info($"Total build process time: {_totalSeconds} seconds");
            
            ResourceBuildConfig resourceBuildConfig = GetData<ResourceBuildConfig>(ResourceBuildConfig);
            _reportLog.Save(resourceBuildConfig.BuildLog,resourceBuildConfig.ReportPath);
            
            UnityEditor.EditorUtility.DisplayDialog("ResourceBuilder", "Successful!","ok");
            Logger.RemoveLogHelper(_reportLog);
        }
        
        public T GetData<T>(string key)
        {
            if ( key == null)
                throw new ArgumentNullException(nameof (key));
            if (!_genericsData.TryGetValue(key,out IGenericData data))
            {
                throw new Exception($"这个Key不存在. Key : {key}");
            }
            
            return data.GetData1<T>();
        }
        
        public void SetData<T>(string key,T data)
        {
            GenericData<T> genericData = GenericData<T>.Create(data);
            if ( key == null)
                throw new ArgumentNullException(nameof (key));
            if (_genericsData.ContainsKey(key))
            {
                throw new Exception($"这个Key已经存在了. Key : {key}");
            }
            _genericsData.Add(key,genericData);
        }
        
        private int GetBuildSeconds()
        {
            float seconds = _buildWatch.ElapsedMilliseconds / 1000f;
            return (int)seconds;
        }
    }
}
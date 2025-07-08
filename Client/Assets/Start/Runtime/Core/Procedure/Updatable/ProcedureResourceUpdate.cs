using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Common;


namespace Start
{
    /// <summary>
    /// 7.更新资源
    /// </summary>
    public class ProcedureResourceUpdate : ProcedureBase
    {
        private bool _downloadComplete;
        private int _needDownloadCount;
        private int _downloadCompleteCount;
        
        protected override Task OnEnter()
        {
            RuntimeEvent.AddListener((int)EMessageId.ConfirmUpdateResource, OnConfirmUpdateResource);
            DownloadManager.Instance.DownloadEventHandler += DownloadEventHandler;
            Dictionary<string, ResourceInfo> updateResourceInfos = 
                Fsm.GetData<Dictionary<string, ResourceInfo>>(ProcedureConst.UpdateResourceInfos);
            _needDownloadCount = updateResourceInfos.Count;
            long size = 0;
            foreach (ResourceInfo resourceInfo in updateResourceInfos.Values)
            {
                size += resourceInfo.Size;
            }
            IGenericData genericData = GenericData<long, long>.Create(updateResourceInfos.Count,size);
            RuntimeEvent.SendMessage((int)EMessageId.DisplayUpdateResourceInfo, genericData);
            return base.OnEnter();
        }

        protected override Task OnExit()
        {
            RuntimeEvent.RemoveListener((int)EMessageId.ConfirmUpdateResource, OnConfirmUpdateResource);
            DownloadManager.Instance.DownloadEventHandler -= DownloadEventHandler;
            Fsm.RemoveData(ProcedureConst.UpdateResourceInfos);
            return base.OnExit();
        }

        private void OnConfirmUpdateResource(IGenericData genericData)
        {
            _downloadComplete = false;
            Logger.Info("开始更新资源!");
            Dictionary<string, ResourceInfo> updateResourceInfos = 
                Fsm.GetData<Dictionary<string, ResourceInfo>>(ProcedureConst.UpdateResourceInfos);
            foreach (var item in updateResourceInfos)
            {
                string downloadPath = Path.Combine(ResourceConfig.ResourcePath, item.Key).RegularPath();
                string downloadUri = Path.Combine(GameConfig.RemoteDownloadResourcePath,item.Key).RegularPath();
                DownloadManager.Instance.AddDownload(downloadPath, downloadUri, "Resource", 0, null);
            }
            if (updateResourceInfos.Count == 0)
            {
                DownloadComplete();
            }
        }
        
        private void DownloadEventHandler(DownloadEvent downloadEvent)
        {
            if (_downloadComplete)
            {
                return;
            }
            if (downloadEvent.StatusType == EDownloadStatusType.Done)
            {
                Logger.Info($"DownloadEventHandler Done: {downloadEvent.DownloadUri} ");
                _downloadCompleteCount += 1;
            }
            
            if (_downloadCompleteCount == _needDownloadCount)
            {
                Logger.Info("热更下载完成");
                DownloadComplete();
            }
        }

        private void DownloadComplete()
        {
            _downloadComplete = true;
            UpdateLocalManifest();
            UpdateLocalResourceVersion();
            ChangeState<ProcedureLaunch>();
        }

        private void UpdateLocalManifest()
        {
            Manifest localManifest = Fsm.GetData<Manifest>(ProcedureConst.LocalManifest);
            Manifest remoteMandatoryManifest = Fsm.GetData<Manifest>(ProcedureConst.RemoteMandatoryManifest);
            Dictionary<string, Manifest> remoteOptionalResourceManifests =
                Fsm.GetData<Dictionary<string, Manifest>>(ProcedureConst.LocalOptionalManifests);
            
            Dictionary<string, AssetInfo> assetInfos = new Dictionary<string, AssetInfo>();
            Dictionary<string, ResourceInfo> resourceInfos = new Dictionary<string, ResourceInfo>();
            
            foreach (ResourceInfo resourceInfo in localManifest.Resources)
            {
                if (resourceInfos.TryGetValue(resourceInfo.Name,out ResourceInfo resource))
                {
                    if (resource.MD5.Equals(resourceInfo.MD5))
                    {
                        Logger.Error($"已存在资源{resourceInfo.Name}");
                    }
                }
                else
                {
                    resourceInfos.Add(resourceInfo.Name,resourceInfo);
                }
            }
            
            AddAssetInfo(localManifest, ref assetInfos);
            AddAssetInfo(remoteMandatoryManifest, ref assetInfos);
            AddResourceInfo(remoteMandatoryManifest, ref resourceInfos);

            if (remoteOptionalResourceManifests != null)
            {
                foreach (Manifest manifest in remoteOptionalResourceManifests.Values)
                {
                    AddAssetInfo(manifest, ref assetInfos);
                    AddResourceInfo(manifest, ref resourceInfos);
                }
            }
            
            Fsm.RemoveData(ProcedureConst.LocalManifest);
            Fsm.RemoveData(ProcedureConst.RemoteMandatoryManifest);
            Fsm.RemoveData(ProcedureConst.LocalOptionalManifests);
            
            localManifest.Assets.Clear();
            localManifest.Resources.Clear();
            localManifest.Assets.AddRange(assetInfos.Values);
            localManifest.Resources.AddRange(resourceInfos.Values);
            FileUtility.CreateFileDirectory(ResourceConfig.LocalManifestPath);
            SerializerUtility.SerializeObject(ResourceConfig.LocalManifestPath,localManifest);
            Logger.Info($"更新本地manifest成功");
        }

        private void UpdateLocalResourceVersion()
        {
            Logger.Info($"更新本地资源版本:{GameConfig.RemoteResourceVersion}");
            FileUtility.WriteAllBytes(ResourceConfig.LocalResourceVersionPath,GameConfig.RemoteResourceVersion);
        }
        
        private void AddAssetInfo(Manifest manifest, ref Dictionary<string, AssetInfo> assetInfos)
        {
            foreach (AssetInfo assetInfo in manifest.Assets)
            {
                if (!assetInfos.ContainsKey(assetInfo.Path))
                {
                    assetInfos.Add(assetInfo.Path,assetInfo);
                }
            }
        }

        private void AddResourceInfo(Manifest manifest, ref Dictionary<string, ResourceInfo> resourceInfos)
        {
            foreach (ResourceInfo resourceInfo in manifest.Resources)
            {
                if (resourceInfos.TryGetValue(resourceInfo.Name,out ResourceInfo resource))
                {
                    if (!resource.MD5.Equals(resourceInfo.MD5))
                    {
                        resourceInfos[resource.Name] = resourceInfo;
                    }
                }
                else
                {
                    resourceInfos.Add(resourceInfo.Name,resourceInfo);
                }
            }
        }
        
    }
}
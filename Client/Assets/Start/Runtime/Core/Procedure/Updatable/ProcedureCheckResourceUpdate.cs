using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;


namespace Start
{
    /// <summary>
    /// 6.检查需要更新的资源内容
    /// </summary>
    public class ProcedureCheckResourceUpdate : ProcedureBase
    {
        protected override async Task OnEnter()
        {
            RuntimeEvent.AddListener((int)EMessageId.ReGetRemoteMandatoryManifest, OnReGetRemoteMandatoryManifest);
            RuntimeEvent.AddListener((int)EMessageId.ReGetRemoteOptionalManifest, OnReGetRemoteOptionalManifest);
            await GetRemoteMandatoryManifest();
        }

        protected override Task OnExit()
        {
            RuntimeEvent.RemoveListener((int)EMessageId.ReGetRemoteMandatoryManifest, OnReGetRemoteMandatoryManifest);
            RuntimeEvent.RemoveListener((int)EMessageId.ReGetRemoteOptionalManifest, OnReGetRemoteOptionalManifest);
            return base.OnExit();
        }

        private async void OnReGetRemoteMandatoryManifest(IGenericData genericData)
        {
            await GetRemoteMandatoryManifest();
        }

        private async void OnReGetRemoteOptionalManifest(IGenericData genericData)
        {
            await GetRemoteOptionalManifest();
        }

        /// <summary>
        /// 获取远程强制更新资源的清单
        /// </summary>
        /// <returns></returns>
        private async Task GetRemoteMandatoryManifest()
        {
            HttpResponse response = await HttpManager.Instance.Get(GameConfig.RemoteMandatoryManifestPath);
            string error = response.Error;
            byte[] result = response.Result;
            bool isSuccess = response.IsSuccess;
            long responseCode = response.ResponseCode;
            RecyclableObjectPool.Recycle(response);
            if (isSuccess)
            {
                Manifest remoteMandatoryManifest = SerializerUtility.DeserializeObject<Manifest>(result);
                Fsm.SetData(ProcedureConst.RemoteMandatoryManifest, remoteMandatoryManifest);
                if (File.Exists(ResourceConfig.LocalOptionalResourceInfoPath) && GameConfig.ExistRemoteOptionalResource)
                {
                    await GetRemoteOptionalManifest();
                }
                else
                {
                    CheckResourceUpdate();
                }
            }
            else
            {
                Logger.Error("获取远程资源清单失败:{0}", error);
                IGenericData genericData = GenericData<long, string>.Create(responseCode, error);
                RuntimeEvent.SendMessage((int)EMessageId.GetRemoteMandatoryManifestFailure, genericData);
                RecyclableObjectPool.Recycle(genericData);
            }
        }

        /// <summary>
        /// 获取远程可选更新资源的清单
        /// </summary>
        /// <returns></returns>
        private async Task GetRemoteOptionalManifest()
        {
            HttpResponse response = await HttpManager.Instance.Get(GameConfig.RemoteOptionalManifestPath);
            string error = response.Error;
            byte[] result = response.Result;
            bool isSuccess = response.IsSuccess;
            long responseCode = response.ResponseCode;
            RecyclableObjectPool.Recycle(response);
            if (isSuccess)
            {
                Dictionary<string, Manifest> remoteOptionalResourceManifests =
                    SerializerUtility.DeserializeObject<Dictionary<string, Manifest>>(result);
                string localOptionalInfo = File.ReadAllText(ResourceConfig.LocalOptionalResourceInfoPath);
                string[] localOptionalInfos = localOptionalInfo.Split('_');
                Dictionary<string, Manifest> localOptionalManifests = new Dictionary<string, Manifest>();
                foreach (string optionalResourceInfo in localOptionalInfos)
                {
                    if (remoteOptionalResourceManifests.TryGetValue(optionalResourceInfo,out Manifest manifest))
                    {
                        localOptionalManifests.Add(optionalResourceInfo,manifest);
                    }
                }
                Fsm.SetData(ProcedureConst.LocalOptionalManifests, localOptionalManifests);
                
                CheckResourceUpdate();
            }
            else
            {
                Logger.Error("获取远程资源清单失败:{0}", error);
                IGenericData genericData = GenericData<long, string>.Create(responseCode, error);
                RuntimeEvent.SendMessage((int)EMessageId.GetRemoteOptionalManifestFailure, genericData);
                RecyclableObjectPool.Recycle(genericData);
            }
        }

        private void CheckResourceUpdate()
        {
            Manifest localManifest = Fsm.GetData<Manifest>(ProcedureConst.LocalManifest);
            Manifest remoteMandatoryManifest = Fsm.GetData<Manifest>(ProcedureConst.RemoteMandatoryManifest);
            Dictionary<string, Manifest> remoteOptionalResourceManifests =
                Fsm.GetData<Dictionary<string, Manifest>>(ProcedureConst.LocalOptionalManifests);

            //本地所有资源信息
            Dictionary<string, ResourceInfo> localResourceInfos = new Dictionary<string, ResourceInfo>();
            //远程需要更新的资源信息
            Dictionary<string, ResourceInfo> remoteResourceInfos = new Dictionary<string, ResourceInfo>();
            //需要更新的资源列表
            Dictionary<string, ResourceInfo> updateResourceInfos = new Dictionary<string, ResourceInfo>();

            foreach (ResourceInfo resourceInfo in localManifest.Resources)
            {
                localResourceInfos[resourceInfo.Name] = resourceInfo;
            }

            foreach (var remoteResourceInfo in remoteMandatoryManifest.Resources)
            {
                remoteResourceInfos.Add(remoteResourceInfo.Name, remoteResourceInfo);
            }

            if (remoteOptionalResourceManifests != null)
            {
                foreach (var item in remoteOptionalResourceManifests.Values)
                {
                    foreach (var remoteResourceInfo in item.Resources)
                    {
                        if (!remoteResourceInfos.TryGetValue(remoteResourceInfo.Name,out ResourceInfo resourceInfo))
                        {
                            remoteResourceInfos.Add(remoteResourceInfo.Name, remoteResourceInfo);
                        }
                        else
                        {
                            if (!resourceInfo.MD5.Equals(remoteResourceInfo.MD5))
                            {
                                Logger.Error($"资源名称相同,但MD5不同,资源名称：{remoteResourceInfo.Name}" );
                            }
                        }
                    }
                }
            }

            foreach (var item in remoteResourceInfos)
            {
                if (localResourceInfos.TryGetValue(item.Key, out ResourceInfo resourceInfo))
                {
                    if (!item.Value.MD5.Equals(resourceInfo.MD5))
                    {
                        updateResourceInfos.Add(item.Key, item.Value);
                    }
                    continue;
                }
                updateResourceInfos.Add(item.Key, item.Value);
            }

            Fsm.SetData(ProcedureConst.UpdateResourceInfos, updateResourceInfos);
            ChangeState<ProcedureResourceUpdate>();
        }
    }
}
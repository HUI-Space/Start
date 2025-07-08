using System.IO;
using System.Threading.Tasks;

namespace Start
{
    public class ProcedureStandaloneResourceInitialize : ProcedureBase
    {
        protected override async Task OnEnter()
        {
            RuntimeEvent.AddListener((int)EMessageId.ReGetBuiltInManifest, OnReGetBuiltInManifest);
            if (!File.Exists(ResourceConfig.LocalManifestPath))
            {
                await GetBuiltInManifest();
            }
            else
            {
                ChangeState<ProcedureLaunch>();
            }
        }
        
        /// <summary>
        /// 获取内嵌资源
        /// </summary>
        /// <returns></returns>
        private async Task GetBuiltInManifest()
        {
            HttpResponse response = await HttpManager.Instance.Get(ResourceConfig.LocalBuiltInManifestPath);
            string error = response.Error;
            byte[] result = response.Result;
            bool isSuccess = response.IsSuccess;
            long responseCode = response.ResponseCode;
            ReferencePool.Release(response);
            if (isSuccess)
            {
                Manifest manifest = SerializerUtility.DeserializeObject<Manifest>(result);
                FileUtility.CreateFileDirectory(ResourceConfig.LocalManifestPath);
                SerializerUtility.SerializeObject(ResourceConfig.LocalManifestPath, manifest);
                Logger.Info("获取内嵌资源配置信息成功");
                Logger.Info("创建LocalManifest.b");
                //开始游戏
                ChangeState<ProcedureLaunch>();
            }
            else
            {
                Logger.Error("获取嵌入资源配置信息错误:" + response.Error);
                IGenericData genericData = GenericData<long, string>.Create(responseCode, error);
                RuntimeEvent.SendMessage((int)EMessageId.GetBuiltInManifestFailure, genericData);
                ReferencePool.Release(genericData);
            }
        }
        
        /// <summary>
        /// 重新获取内嵌资源
        /// </summary>
        /// <returns></returns>
        private async void OnReGetBuiltInManifest(IGenericData data)
        {
            await GetBuiltInManifest();
        }
    }
}
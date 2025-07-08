using System.IO;
using System.Threading.Tasks;


namespace Start
{
    /// <summary>
    /// 3.初始化嵌入资源
    /// 这个步骤通常第一次运行游戏的时候运行
    /// 主要的作用是将嵌入到包内的资源信息写入到统一的文件内
    /// 这样以后就可以直接从统一的文件内读取资源信息，而不用每次都去解析AssetBundle包内的文件
    /// </summary>
    public class ProcedureResourceInitialize : ProcedureBase
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
                ChangeState<ProcedureCheckResourceVersion>();
            }
        }

        protected override Task OnExit()
        {
            RuntimeEvent.RemoveListener((int)EMessageId.ReGetBuiltInManifest, OnReGetBuiltInManifest);
            return base.OnExit();
        }

        /// <summary>
        /// 重新获取内嵌资源
        /// </summary>
        /// <returns></returns>
        private async void OnReGetBuiltInManifest(IGenericData data)
        {
            await GetBuiltInManifest();
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
                ChangeState<ProcedureCheckResourceVersion>();
            }
            else
            {
                Logger.Error("获取嵌入资源配置信息错误:" + response.Error);
                IGenericData genericData = GenericData<long, string>.Create(responseCode, error);
                RuntimeEvent.SendMessage((int)EMessageId.GetBuiltInManifestFailure, genericData);
                ReferencePool.Release(genericData);
            }
        }
    }
}
using System.IO;
using System.Threading.Tasks;


namespace Start
{
    /// <summary>
    /// 4.检测资源
    /// 判断是否开启热更
    /// 获取CDN上的资源版本文件中的版本号
    /// 判断本地是否存在资源版本文件
    /// 判断本地版本号是否等于远程版本号
    /// 创建本地资源版本文件
    /// </summary>
    public class ProcedureCheckResourceVersion : ProcedureBase
    {
        protected override async Task OnEnter()
        {
            RuntimeEvent.AddListener((int)EMessageId.ReGetRemoteResourceVersion, OnReGetRemoteResourceVersion);
            //判断是否开启热更
            if (GameConfig.GameClient.EnableHotUpdate)
            {
                await GetRemoteResourceVersion();
            }
            else
            {
                //未开启热更 开启游戏
                ChangeState<ProcedureLaunch>();
            }
        }

        protected override Task OnExit()
        {
            RuntimeEvent.RemoveListener((int)EMessageId.ReGetRemoteResourceVersion, OnReGetRemoteResourceVersion);
            return base.OnExit();
        }

        /// <summary>
        /// 重新获取版本号
        /// </summary>
        /// <param name="data"></param>
        private async void OnReGetRemoteResourceVersion(IGenericData data)
        {
            await GetRemoteResourceVersion();
        }
        
        /// <summary>
        /// 获取远程版本号
        /// </summary>
        private async Task GetRemoteResourceVersion()
        {
            //1.获取CDN上的资源版本文件中的版本号
            HttpResponse response = await HttpManager.Instance.Get(GameConfig.RemoteResourceVersionPath);
            string error = response.Error;
            byte[] result = response.Result;
            bool isSuccess = response.IsSuccess;
            long responseCode = response.ResponseCode;
            RecyclableObjectPool.Recycle(response);
            if (isSuccess)
            {
                CheckResourceVersion(ResourceConfig.LocalResourceVersionPath, result);
            }
            else
            {
                Logger.Fatal("获取远程版本号错误:" + response.Error);
                IGenericData genericData = GenericData<long, string>.Create(responseCode, error);
                RuntimeEvent.SendMessage((int)EMessageId.GetRemoteResourceVersionFailure, genericData);
                RecyclableObjectPool.Recycle(genericData);
            }
        }

        /// <summary>
        /// 检查资源版本
        /// </summary>
        /// <param name="LocalResourceVersionPath">本地资源版本号路径</param>
        /// <param name="data">远程资源版本信息</param>
        /// <returns></returns>
        private void CheckResourceVersion(string LocalResourceVersionPath,byte[] data)
        {
            string remoteResourceVersion = SerializerUtility.DeserializeObject<string>(data);
            string[] strings = remoteResourceVersion.Split('|');
            GameConfig.RemoteResourceVersion = strings[0];
            GameConfig.ExistRemoteOptionalResource = strings[1] == "True";
            if (File.Exists(LocalResourceVersionPath))
            {
                string localResourceVersion = FileUtility.ReadAllText(LocalResourceVersionPath);
                if (localResourceVersion.Equals(remoteResourceVersion))
                {
                    ChangeState<ProcedureLaunch>();
                    return;
                }
            }
            ChangeState<ProcedureResourceVerify>();;
        }
    }
}
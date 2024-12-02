using System.Text;
using System.Threading.Tasks;
using Start.Framework;

namespace Start.Runtime
{
    /// <summary>
    /// 2.检测游戏版本号
    /// 检查本地游戏版本号和远程游戏版本号是否相同
    /// 不相同则需要去下载新的安装包
    /// 版本相同则切换状态到 ProcedureInitResource
    /// </summary>
    public class ProcedureCheckGameVersion : ProcedureBase
    {
        protected override async Task OnEnter()
        {
            RuntimeMessage.AddListener((int)EMessageId.ReGetLocalVersion, OnReGetLocalVersion);
            RuntimeMessage.AddListener((int)EMessageId.ReGetRemoteGameClient, OnReGetRemoteGameClient);
            await GetLocalVersion();
        }
        
        protected override Task OnExit()
        {
            RuntimeMessage.RemoveListener((int)EMessageId.ReGetLocalVersion, OnReGetLocalVersion);
            RuntimeMessage.RemoveListener((int)EMessageId.ReGetRemoteGameClient, OnReGetRemoteGameClient);
            return base.OnExit();
        }
        
        /// <summary>
        /// 获取本地版本号
        /// </summary>
        /// <returns></returns>
        private async Task GetLocalVersion()
        {
            HttpResponse response = await HttpManager.Instance.Get(ResourceConfig.LocalGameVersionPath);
            string error = response.Error;
            byte[] result = response.Result;
            bool isSuccess = response.IsSuccess;
            long responseCode = response.ResponseCode;
            ReferencePool.Release(response);
            if (isSuccess)
            {
                GameConfig.LocalGameVersion = Encoding.UTF8.GetString(result);
                Log.Info("本地版本号:" + GameConfig.LocalGameVersion);
                await GetRemoteGameClient();
            }
            else
            {
                Log.Error("获取本地版本号错误:" + error);
                IGenericData genericData = GenericData<long, string>.Create(responseCode, error);
                RuntimeMessage.SendMessage((int)EMessageId.GetLocalVersionFailure, genericData);
                ReferencePool.Release(genericData);
            }
        }

        /// <summary>
        /// 获取远程 GameClient
        /// </summary>
        /// <returns></returns>
        private async Task GetRemoteGameClient()
        {
            Log.Info($"获取远程 GameClient Path:{GameConfig.RemoteGameClientPath}");
            HttpResponse response = await HttpManager.Instance.Get(GameConfig.RemoteGameClientPath);
            string error = response.Error;
            byte[] result = response.Result;
            bool isSuccess = response.IsSuccess;
            long responseCode = response.ResponseCode;
            ReferencePool.Release(response);
            if (isSuccess)
            {
                GameClient gameClient = UnityUtility.FromJson<GameClient>(Encoding.UTF8.GetString(result));
                GameConfig.GameClient = gameClient;
                Log.Info("远程登录地址:" + GameConfig.GameClient.LoginUrl);
                Log.Info("远程下载地址:" + GameConfig.GameClient.DownloadUrl);
                Log.Info("远程游戏版本号:" + GameConfig.GameClient.GameVersion);
                Log.Info("远程资源根目录:" + GameConfig.GameClient.ResourceRoot);
                Log.Info("远程资源是否开启热更:" + GameConfig.GameClient.EnableHotUpdate);
                CheckGameVersion();
            }
            else
            {
                Log.Error("获取远程 GameClient 错误:" + error);
                IGenericData genericData = GenericData<long, string>.Create(responseCode, error);
                RuntimeMessage.SendMessage((int)EMessageId.GetRemoteGameClientFailure, genericData);
                ReferencePool.Release(genericData);
            }
        }
        
        private async void CheckGameVersion()
        {
            if (GameConfig.GameClient !=null && !string.IsNullOrEmpty(GameConfig.GameClient.GameVersion) && !string.IsNullOrEmpty(GameConfig.LocalGameVersion))
            {
                if (GameConfig.GameClient.GameVersion.Equals(GameConfig.LocalGameVersion))
                {
                    await ChangeState<ProcedureResourceInitialize>();
                }
                else
                {
                    //本地版本号和远程版本号不一致
                    //此时基本上需要重新下载新的安装包，重新安装
                    Log.Info("本地版本号和远程版本号不一致，需要重新下载安装包");
                    Log.Info("下载地址：" + GameConfig.GameClient.DownloadUrl);
                    IGenericData genericData = GenericData<string>.Create(GameConfig.GameClient.DownloadUrl);
                    RuntimeMessage.SendMessage((int)EMessageId.GameVersionUpdated, genericData);
                    ReferencePool.Release(genericData);
                }
            }
        }
        
        private async void OnReGetLocalVersion(IGenericData data)
        {
            await GetLocalVersion();
        }
        
        private async void OnReGetRemoteGameClient(IGenericData data)
        {
            await GetRemoteGameClient();
        }
    }
}
using System;
using System.IO;
using System.Threading.Tasks;

namespace Start
{
    /// <summary>
    /// 5.校验资源
    /// 读取本地依赖配置
    /// 读取本地内嵌资源
    /// 校验本地文件
    /// </summary>
    public class ProcedureResourceVerify : ProcedureBase
    {
        protected override async Task OnEnter()
        {
            RuntimeEvent.AddListener((int)EMessageId.ReGetBuiltInResource, OnReGetBuiltInResource);
            //判断本地依赖配置是否存在
            if (!File.Exists(ResourceConfig.LocalManifestPath))
            {
                Logger.Error("获取本地依赖信息失败");
                //本地依赖配置不存在，则重新创建本地依赖资源
                ChangeState<ProcedureResourceInitialize>();
            }
            else
            {
                //读取本地依赖配置
                Manifest localManifest = SerializerUtility.DeserializeObject<Manifest>(ResourceConfig.LocalManifestPath);
                Fsm.SetData(ProcedureConst.LocalManifest, localManifest);
                await GetBuiltInResource();
            }
        }

        protected override Task OnExit()
        {
            RuntimeEvent.RemoveListener((int)EMessageId.ReGetBuiltInResource, OnReGetBuiltInResource);
            return base.OnExit();
        }


        /// <summary>
        /// 校验本地资源信息
        /// </summary>
        /// <param name="localManifest">本地依赖信息</param>
        /// <param name="builtIn">嵌入资源</param>
        private async Task VerifyLocalResource(Manifest localManifest,byte[] builtIn)
        {
            bool needInitialize = false;
            foreach (ResourceInfo resourceInfo in localManifest.Resources)
            {
                if (resourceInfo.ResourceType == EResourceType.BuiltIn)
                {
                    byte[] targetArray = new byte[resourceInfo.Size];
                    Array.Copy(builtIn, (long)resourceInfo.Offset, targetArray, 0, resourceInfo.Size);
                    if (!HashUtility.BytesMD5(targetArray).Equals(resourceInfo.MD5))
                    {
                        Logger.Error("内置资源与依赖配置错误! 资源文件MD5校验失败:{0}", resourceInfo.Name);//通常是修改本地文件
                        needInitialize = true;
                        break;
                    }
                }
                else
                {
                    string resourcePath = Path.Combine(ResourceConfig.ResourcePath,resourceInfo.Name);
                    if (!File.Exists(resourcePath) || !HashUtility.FileMD5(resourcePath).Equals(resourceInfo.MD5))
                    {
                        Logger.Error("资源文件MD5校验失败:{0}", resourceInfo.Name);
                        break;
                    }
                }
                await Task.Delay(1);
            }
            if (needInitialize)
            {
                ChangeState<ProcedureResourceInitialize>();
                return;
            }
            ChangeState<ProcedureCheckResourceUpdate>();
        }
        
        /// <summary>
        /// 重新读取内嵌资源
        /// </summary>
        /// <param name="data"></param>
        private async void OnReGetBuiltInResource(IGenericData data)
        {
            await GetBuiltInResource();
        }
        
        /// <summary>
        /// 获取内嵌资源源文件
        /// </summary>
        /// <returns></returns>
        private async Task GetBuiltInResource()
        {
            HttpResponse response = await HttpManager.Instance.Get(ResourceConfig.LocalBuiltInResourcePath);
            string error = response.Error;
            byte[] result = response.Result;
            bool isSuccess = response.IsSuccess;
            long responseCode = response.ResponseCode;
            RecyclableObjectPool.Recycle(response);
            if (isSuccess)
            {
                Manifest localManifest = Fsm.GetData<Manifest>(ProcedureConst.LocalManifest);
                await VerifyLocalResource(localManifest, result);
            }
            else
            {
                Logger.Error("获取内嵌资源失败:{0}", response.Error);
                IGenericData genericData = GenericData<long, string>.Create(responseCode, error);
                RuntimeEvent.SendMessage((int)EMessageId.GetBuiltInResourceFailure, genericData);
                RecyclableObjectPool.Recycle(genericData);
            }
        }
    }
}
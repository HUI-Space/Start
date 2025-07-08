using System.Threading.Tasks;


namespace Start
{
    /// <summary>
    /// 1.流程开始
    /// </summary>
    public class ProcedureMain : ProcedureBase
    {
        protected override Task OnEnter()
        {
            //使用AssetBundle
            if (GameConfig.EnableAssetbundle)
            {
                if (GameConfig.ResourceModeType == EResourceModeType.Standalone)
                {
                    ChangeState<ProcedureStandaloneResourceInitialize>();
                    return base.OnEnter();
                }
                if (GameConfig.ResourceModeType == EResourceModeType.Updatable)
                {
                    ChangeState<ProcedureCheckGameVersion>();
                    return base.OnEnter();
                }
            }
            ChangeState<ProcedureLaunch>();
            return base.OnEnter();
        }
    }
}
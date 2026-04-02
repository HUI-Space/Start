using System.Threading.Tasks;

namespace Start
{
    public class PlayingState : AsyncFsmState<BattleManager>
    {
        protected override async Task OnEnter()
        {
            EventManager.Instance.AddListener((int)EEventType.FrameSync,(int)EMessageId.FrameSyncEnd, OnFrameSyncEnd);
            //1.关闭加载界面
            await UIActions.HideLoading();
            //2.开始战斗
            BattleManager.Instance.StartEngine();
        }
        
        private void OnFrameSyncEnd(IGenericData data)
        {
            ChangeState<EndState>();
        }
        
        protected override Task OnExit()
        {
            EventManager.Instance.RemoveListener((int)EEventType.FrameSync,(int)EMessageId.FrameSyncEnd, OnFrameSyncEnd);
            return Task.CompletedTask;
        }
    }
}
namespace Start
{
    [MatchState(EMatchState.End)]
    public class MatchEndState : MatchStateBase
    {
        public override void OnEnter(MatchEntity matchEntity)
        {
            base.OnEnter(matchEntity);
            EventManager.Instance.SendMessage((int)EEventType.FrameSync,(int)EMessageId.FrameSyncEnd,null);
        }
        
        public override void OnUpdate(MatchEntity matchEntity)
        {
            base.OnUpdate(matchEntity);
        }
        
        public override void OnExit(MatchEntity matchEntity)
        {
            base.OnExit(matchEntity);
        }
    }
}
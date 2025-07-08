namespace Start
{
    public abstract class ExampleSyncFsmState : SyncFsmState<ExampleSyncFsm>
    {
        protected override void OnEnter()
        {
            UIActions.FsmPanel_SyncFsmLog($"{GetType().FullName} OnEnter");
        }

        protected override void OnExit()
        {
            UIActions.FsmPanel_SyncFsmLog($"{GetType().FullName} OnExit");
        }
    }
    
    
    public class ExampleSyncFsmState_1 : ExampleSyncFsmState
    {
        
    }
    
    public class ExampleSyncFsmState_2 : ExampleSyncFsmState
    {
        
    }
    
    public class ExampleSyncFsmState_3 : ExampleSyncFsmState
    {
        
    }
    
    public class ExampleSyncFsmState_4 : ExampleSyncFsmState
    {
        
    }
}
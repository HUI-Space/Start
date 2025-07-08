using System.Threading.Tasks;
using UnityEngine;

namespace Start
{
    
    public class ExampleAsyncFsmState : AsyncFsmState<ExampleAsyncFsm>
    {
        protected override async Task OnEnter()
        {
            UIActions.FsmPanel_AsyncFsmLog($"{GetType().FullName} OnEnter start");
            var random = Random.Range(0,5) ;
            UIActions.FsmPanel_AsyncFsmLog($"{GetType().FullName} OnEnter start delay {random} 秒");
            await Task.Delay(random * 1000);
            UIActions.FsmPanel_AsyncFsmLog($"{GetType().FullName} OnEnter end delay {random} 秒");
            UIActions.FsmPanel_AsyncFsmLog($"{GetType().FullName} OnEnter end");
        }


        protected override Task OnExit()
        {
            UIActions.FsmPanel_AsyncFsmLog($"{GetType().FullName} OnExit");
            return base.OnExit();
        }
    }
    
    public class ExampleAsyncFsmState_1 : ExampleAsyncFsmState
    {
        
    }
    
    public class ExampleAsyncFsmState_2 : ExampleAsyncFsmState
    {
        
    }
    
    public class ExampleAsyncFsmState_3 : ExampleAsyncFsmState
    {
        
    }
    
    public class ExampleAsyncFsmState_4 : ExampleAsyncFsmState
    {
        
    }
}
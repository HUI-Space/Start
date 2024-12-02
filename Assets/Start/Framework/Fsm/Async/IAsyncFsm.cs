using System.Threading.Tasks;

namespace Start.Framework
{
    public interface IAsyncFsm<T>:IFsm<T> where T : class
    {
        
        /// <summary>
        /// 当前状态
        /// </summary>
        AsyncFsmState<T> CurrentState{ get; }
        
        /// <summary>
        /// 开始状态
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        Task Start<TState>() where TState : AsyncFsmState<T>;
        
        /// <summary>
        /// 是否拥有状态
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <returns></returns>
        bool HasState<TState>() where TState : AsyncFsmState<T>;
        
        /// <summary>
        /// 获取状态
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <returns></returns>
        TState GetState<TState>() where TState : AsyncFsmState<T>;
        
        /// <summary>
        /// 获取有限状态机的所有状态
        /// </summary>
        /// <returns></returns>
        AsyncFsmState<T>[] GetAllStates();
    }
}
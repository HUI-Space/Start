namespace Start.Framework
{
    public interface ISyncFsm<T>:IFsm<T> where T : class
    {
        /// <summary>
        /// 当前状态
        /// </summary>
        SyncFsmState<T> CurrentState{ get; }
        
        /// <summary>
        /// 开始状态
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        void Start<TState>() where TState : SyncFsmState<T>;
        
        /// <summary>
        /// 是否拥有状态
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <returns></returns>
        bool HasState<TState>() where TState : SyncFsmState<T>;
        
        /// <summary>
        /// 获取状态
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <returns></returns>
        TState GetState<TState>() where TState : SyncFsmState<T>;
        
        /// <summary>
        /// 获取有限状态机的所有状态
        /// </summary>
        /// <returns></returns>
        SyncFsmState<T>[] GetAllStates();
    }
}
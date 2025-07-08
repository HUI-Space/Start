using System;
using System.Threading.Tasks;

namespace Start
{
    public interface IAsyncFsm<T> : IFsm<T> where T : class
    {
        /// <summary>
        /// 当前状态
        /// </summary>
        AsyncFsmState<T> CurrentState { get; }
        
        /// <summary>
        /// 开始状态
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        Task Start<TState>() where TState : AsyncFsmState<T>;

        /// <summary>
        /// 开始状态
        /// </summary>
        /// <param name="stateType">状态类型</param>
        /// <returns></returns>
        Task Start(Type stateType);

        /// <summary>
        /// 切换状态
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <returns></returns>
        void ChangeState<TState>() where TState : AsyncFsmState<T>;

        /// <summary>
        /// 切换状态
        /// </summary>
        void ChangeState(Type stateType);

        /// <summary>
        /// 是否拥有状态
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <returns></returns>
        bool HasState<TState>() where TState : AsyncFsmState<T>;

        /// <summary>
        /// 是否拥有状态
        /// </summary>
        /// <param name="stateType">状态类型</param>
        /// <returns></returns>
        bool HasState(Type stateType);

        /// <summary>
        /// 获取状态
        /// </summary>
        /// <typeparam name="TState"></typeparam>
        /// <returns></returns>
        TState GetState<TState>() where TState : AsyncFsmState<T>;

        /// <summary>
        /// 获取状态
        /// </summary>
        /// <param name="stateType">状态类型</param>
        /// <returns></returns>
        AsyncFsmState<T> GetState(Type stateType);

        /// <summary>
        /// 获取有限状态机的所有状态
        /// </summary>
        /// <returns></returns>
        AsyncFsmState<T>[] GetAllStates();
    }
}
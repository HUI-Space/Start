using System;

namespace Start
{
    /// <summary>
    /// 无参数优先级委托
    /// 支持按优先级顺序执行无参数的Action委托
    /// 使用对象池管理，减少GC分配
    /// </summary>
    public class PriorityDelegate : PriorityDelegateBase<Action>
    {
        /// <summary>
        /// 从对象池创建优先级委托实例
        /// </summary>
        /// <returns>优先级委托实例</returns>
        public static PriorityDelegate Create()
        {
            PriorityDelegate priorityDelegate = RecyclablePool.Acquire<PriorityDelegate>();
            return priorityDelegate;
        }

        /// <summary>
        /// 按优先级顺序调用所有注册的委托
        /// </summary>
        public void Invoke()
        {
            foreach (Action action in GetInvokeList())
            {
                try
                {
                    action.Invoke();
                }
                catch (Exception e)
                {
                    string targetName = action.Target?.GetType().Name ?? "Static";
                    throw new Exception($"委托回调执行失败 => {targetName}.{action.Method.Name}", e);
                }
            }
        }
    }

    /// <summary>
    /// 单参数优先级委托
    /// 支持按优先级顺序执行单参数的Action委托
    /// 使用对象池管理，减少GC分配
    /// </summary>
    /// <typeparam name="T">委托参数类型</typeparam>
    public class PriorityDelegate<T> : PriorityDelegateBase<Action<T>>
    {
        /// <summary>
        /// 从对象池创建优先级委托实例
        /// </summary>
        /// <returns>优先级委托实例</returns>
        public static PriorityDelegate<T> Create()
        {
            PriorityDelegate<T> priorityDelegate = RecyclablePool.Acquire<PriorityDelegate<T>>();
            return priorityDelegate;
        }

        /// <summary>
        /// 按优先级顺序调用所有注册的委托
        /// </summary>
        /// <param name="param">传递给委托的参数</param>
        public void Invoke(T param)
        {
            foreach (Action<T> action in GetInvokeList())
            {
                try
                {
                    action.Invoke(param);
                }
                catch (Exception e)
                {
                    string targetName = action.Target?.GetType().Name ?? "Static";
                    throw new Exception($"委托回调执行失败 => {targetName}.{action.Method.Name}", e);
                }
            }
        }
    }

    /// <summary>
    /// 双参数优先级委托
    /// 支持按优先级顺序执行双参数的Action委托
    /// 使用对象池管理，减少GC分配
    /// </summary>
    /// <typeparam name="T1">第一个参数类型</typeparam>
    /// <typeparam name="T2">第二个参数类型</typeparam>
    public class PriorityDelegate<T1, T2> : PriorityDelegateBase<Action<T1, T2>>
    {
        /// <summary>
        /// 从对象池创建优先级委托实例
        /// </summary>
        /// <returns>优先级委托实例</returns>
        public static PriorityDelegate<T1, T2> Create()
        {
            PriorityDelegate<T1, T2> priorityDelegate = RecyclablePool.Acquire<PriorityDelegate<T1, T2>>();
            return priorityDelegate;
        }

        /// <summary>
        /// 按优先级顺序调用所有注册的委托
        /// </summary>
        /// <param name="param1">第一个参数</param>
        /// <param name="param2">第二个参数</param>
        public void Invoke(T1 param1, T2 param2)
        {
            foreach (Action<T1, T2> action in GetInvokeList())
            {
                try
                {
                    action.Invoke(param1, param2);
                }
                catch (Exception e)
                {
                    string targetName = action.Target?.GetType().Name ?? "Static";
                    throw new Exception($"委托回调执行失败 => {targetName}.{action.Method.Name}", e);
                }
            }
        }
    }
}
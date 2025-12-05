using System;

namespace Start
{
    public class PriorityDelegate : PriorityDelegateBase<Action>
    {
        public static PriorityDelegate Create()
        {
            PriorityDelegate priorityDelegate = RecyclableObjectPool.Acquire<PriorityDelegate>();
            return priorityDelegate;
        }

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
                    throw new Exception($"委托回调执行失败 => {action.Target.GetType().Name}.{action.Method.Name} : {e}");
                }
            }
        }
    }

    public class PriorityDelegate<T> : PriorityDelegateBase<Action<T>>
    {
        public static PriorityDelegate<T> Create()
        {
            PriorityDelegate<T> priorityDelegate = RecyclableObjectPool.Acquire<PriorityDelegate<T>>();
            return priorityDelegate;
        }

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
                    throw new Exception($"委托回调执行失败 => {action.Target.GetType().Name}.{action.Method.Name} : {e}");
                }
            }
        }
    }

    public class PriorityDelegate<T1, T2> : PriorityDelegateBase<Action<T1, T2>>
    {
        public static PriorityDelegate<T1, T2> Create()
        {
            PriorityDelegate<T1, T2> priorityDelegate = RecyclableObjectPool.Acquire<PriorityDelegate<T1, T2>>();
            return priorityDelegate;
        }

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
                    throw new Exception($"委托回调执行失败 => {action.Target.GetType().Name}.{action.Method.Name} : {e}");
                }
            }
        }
    }
}
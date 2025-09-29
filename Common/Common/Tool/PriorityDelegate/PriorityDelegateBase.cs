using System;
using System.Collections.Generic;

namespace Start
{
    public abstract class PriorityDelegateBase<TDelegate> : IReference where TDelegate : MulticastDelegate
    {
        public bool CanBeReleased => _count == 0;
        private int _count = 0;
        private readonly List<int> _priority = new List<int>();
        private readonly List<TDelegate> _actions = new List<TDelegate>();
        private readonly List<TDelegate> _invokeList = new List<TDelegate>();

        /// <summary>
        /// 注册委托回调
        /// 优先级数字越小表示优先级越高
        /// 严禁注册重复委托回调
        /// </summary>
        /// <param name="callback">委托回调</param>
        /// <param name="priority">优先级</param>
        public void AddListener(TDelegate callback, int priority)
        {
            if (callback == null)
            {
                throw new Exception("Callback is null");
            }
            else if (_actions.Contains(callback))
            {
                throw new Exception("Callback is Contains");
            }

            int insertIndex = 0;
            while (insertIndex < _count)
            {
                if (_priority[insertIndex] > priority)
                {
                    break;
                }

                insertIndex++;
            }

            _actions.Insert(insertIndex, callback);
            _priority.Insert(insertIndex, priority);
            _count++;
        }

        /// <summary>
        /// 注销委托回调
        /// </summary>
        /// <param name="rCallback">委托回调</param>
        public void RemoveListener(TDelegate rCallback)
        {
            for (int i = 0; i < _count; i++)
            {
                if (_actions[i].Equals(rCallback))
                {
                    _actions.RemoveAt(i);
                    _priority.RemoveAt(i);
                    _count--;
                    break;
                }
            }
        }

        protected IEnumerable<TDelegate> GetInvokeList()
        {
            _invokeList.Clear();
            _invokeList.AddRange(_actions);
            return _invokeList;
        }

        /// <summary>
        /// 移除所有委托回调
        /// </summary>
        private void RemoveAll()
        {
            _actions.Clear();
            _priority.Clear();
            _count = 0;
        }

        ~PriorityDelegateBase()
        {
            RemoveAll();
        }

        public void Clear()
        {
            RemoveAll();
        }
    }
}
using System;
using System.Collections.Generic;

namespace Start
{
    /// <summary>
    /// 优先级委托基类
    /// 提供按优先级顺序管理和调用委托的基础功能
    /// 支持委托的注册、注销和按优先级顺序执行
    /// </summary>
    /// <typeparam name="TDelegate">委托类型，必须继承自MulticastDelegate</typeparam>
    public abstract class PriorityDelegateBase<TDelegate> : IRecycle where TDelegate : MulticastDelegate
    {
        private struct PriorityItem
        {
            public readonly int Priority;
            public readonly TDelegate Action;
            public PriorityItem(int priority, TDelegate action)
            {
                Priority = priority;
                Action = action;
            }
        }

        private readonly List<PriorityItem> _items = new List<PriorityItem>();
        private readonly HashSet<TDelegate> _actionSet = new HashSet<TDelegate>();
        private readonly List<TDelegate> _invokeList = new List<TDelegate>();

        /// <summary>
        /// 委托回调数量
        /// </summary>
        public int Count => _items.Count;

        /// <summary>
        /// 是否可以被释放回对象池
        /// 当委托数量为0时返回true
        /// </summary>
        public bool CanBeReleased => _items.Count == 0;

        /// <summary>
        /// 注册委托回调
        /// 优先级数字越小表示优先级越高
        /// 严禁注册重复委托回调
        /// </summary>
        /// <param name="callback">委托回调</param>
        /// <param name="priority">优先级，数值越小优先级越高</param>
        public void AddListener(TDelegate callback, int priority)
        {
            if (callback == null)
            {
                throw new ArgumentNullException(nameof(callback));
            }

            if (!_actionSet.Add(callback))
            {
                throw new InvalidOperationException("Callback already registered");
            }

            int insertIndex = FindInsertIndex(priority);
            _items.Insert(insertIndex, new PriorityItem(priority, callback));
        }

        /// <summary>
        /// 注销委托回调
        /// </summary>
        /// <param name="rCallback">要注销的委托回调</param>
        public bool RemoveListener(TDelegate rCallback)
        {
            if (!_actionSet.Remove(rCallback))
            {
                return false;
            }

            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].Action.Equals(rCallback))
                {
                    _items.RemoveAt(i);
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 获取委托调用列表的副本
        /// 用于安全地遍历和执行所有注册的委托
        /// </summary>
        /// <returns>按优先级排序的委托列表</returns>
        protected IEnumerable<TDelegate> GetInvokeList()
        {
            _invokeList.Clear();
            for (int i = 0; i < _items.Count; i++)
            {
                _invokeList.Add(_items[i].Action);
            }
            return _invokeList;
        }

        /// <summary>
        /// 查找插入位置，按优先级从高到低排序（低优先级值在前）
        /// </summary>
        private int FindInsertIndex(int priority)
        {
            int low = 0;
            int high = _items.Count;

            while (low < high)
            {
                int mid = (low + high) / 2;
                if (_items[mid].Priority <= priority)
                {
                    high = mid;
                }
                else
                {
                    low = mid + 1;
                }
            }

            return low;
        }

        /// <summary>
        /// 移除所有委托回调
        /// </summary>
        private void ClearAll()
        {
            _items.Clear();
            _actionSet.Clear();
        }

        /// <summary>
        /// 回收对象，清空所有委托
        /// 实现IRecycle接口
        /// </summary>
        public void Recycle()
        {
            ClearAll();
        }
    }
}
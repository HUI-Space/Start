using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Start
{
    /// <summary>
    /// 扩展
    /// </summary>
    public static class Extensions
    {
        /// <summary>
        /// 从 Stack<T> 中移除指定的对象
        /// </summary>
        /// <typeparam name="T">栈中元素的类型</typeparam>
        /// <param name="stack">要操作的栈实例</param>
        /// <param name="item">要移除的对象</param>
        /// <returns>是否成功移除（对象不存在则返回 false）</returns>
        /// <exception cref="ArgumentNullException">栈实例为 null 时抛出</exception>
        public static bool TryRemove<T>(this Stack<T> stack, T item)
        {
            // 校验入参，避免空引用异常
            if (stack == null)
                throw new ArgumentNullException(nameof(stack), "栈实例不能为 null");

            // 栈为空时直接返回 false
            if (stack.Count == 0)
                return false;

            // 临时列表存储不需要移除的元素
            var tempList = new List<T>();
            bool isRemoved = false;

            // 遍历栈：弹出所有元素，筛选出不需要移除的
            while (stack.Count > 0)
            {
                T currentItem = stack.Pop();

                // 找到目标对象时标记为已移除，不加入临时列表
                if (!isRemoved && EqualityComparer<T>.Default.Equals(currentItem, item))
                {
                    isRemoved = true;
                    continue;
                }

                // 非目标对象加入临时列表
                tempList.Add(currentItem);
            }

            // 将临时列表中的元素重新压回栈（注意反转，恢复原顺序）
            // 因为栈是后进先出，临时列表是逆序的，需要反向遍历
            for (int i = tempList.Count - 1; i >= 0; i--)
            {
                stack.Push(tempList[i]);
            }

            return isRemoved;
        }

        /// <summary>
        /// 重载：移除栈中所有匹配的对象（支持重复元素）
        /// </summary>
        /// <typeparam name="T">栈中元素的类型</typeparam>
        /// <param name="stack">要操作的栈实例</param>
        /// <param name="item">要移除的对象</param>
        /// <returns>成功移除的元素数量</returns>
        /// <exception cref="ArgumentNullException">栈实例为 null 时抛出</exception>
        public static int TryRemoveAll<T>(this Stack<T> stack, T item)
        {
            if (stack == null)
                throw new ArgumentNullException(nameof(stack), "栈实例不能为 null");

            if (stack.Count == 0)
                return 0;

            var tempList = new List<T>();
            int removeCount = 0;

            while (stack.Count > 0)
            {
                T currentItem = stack.Pop();

                if (EqualityComparer<T>.Default.Equals(currentItem, item))
                {
                    removeCount++;
                    continue;
                }

                tempList.Add(currentItem);
            }

            // 恢复栈的顺序
            for (int i = tempList.Count - 1; i >= 0; i--)
            {
                stack.Push(tempList[i]);
            }

            return removeCount;
        }

        public static bool TryRemove<T>(this ConcurrentStack<T> stack, T item)
        {
            var tempList = new List<T>();
            while (stack.TryPop(out var temp))
            {
                if (temp.Equals(item))
                {
                    // 把弹出的非目标元素塞回去
                    foreach (var obj in tempList)
                    {
                        stack.Push(obj);
                    }

                    return true;
                }

                tempList.Add(temp);
            }

            // 全部塞回去（未找到目标）
            foreach (var obj in tempList)
            {
                stack.Push(obj);
            }

            return false;
        }
    }
}
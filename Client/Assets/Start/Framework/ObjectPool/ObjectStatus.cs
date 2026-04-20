using System;

namespace Start
{
    /// <summary>
    /// 对象池对象状态容器
    /// </summary>
    /// <typeparam name="T">对象类型</typeparam>
    public class ObjectStatus<T> : IRecycle where T : class, IObject, new()
    {
        /// <summary>
        /// 对象是否被加锁（锁定后不可释放）
        /// </summary>
        public bool Locked { get; set; }

        /// <summary>
        /// 对象优先级（数值越低，越优先保留）
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 对象上次使用时间（UTC时间）
        /// </summary>
        public DateTime LastUseTime { get; set; }
        
        /// <summary>
        /// 关联的对象实例
        /// </summary>
        public T Target { get; private set; }
        
        /// <summary>
        /// 自定义释放检查标记（可重写实现个性化释放规则）
        /// </summary>
        public virtual bool CustomCanReleaseFlag => true;
        
        /// <summary>
        /// 创建对象状态实例
        /// </summary>
        /// <param name="target">关联的对象</param>
        /// <param name="locked">是否锁定</param>
        /// <param name="priority">优先级</param>
        /// <returns>对象状态实例</returns>
        public static ObjectStatus<T> Create(T target, bool locked = false, int priority = 0)
        {
            ObjectStatus<T> status = RecyclablePool.Acquire<ObjectStatus<T>>();
            status.Target = target;
            status.Locked = locked;
            status.Priority = priority;
            status.LastUseTime = DateTime.UtcNow;
            return status;
        }
        
        /// <summary>
        /// 回收状态对象（重置状态）
        /// </summary>
        public void Recycle()
        {
            Locked = false;
            Priority = 0;
            LastUseTime = default;
            Target = null;
        }
    }
}
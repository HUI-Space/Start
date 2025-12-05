using System;

namespace Start
{
    public abstract class ObjectBase<T> : IReusable where T : class
    {
        /// <summary>
        /// 对象名称。
        /// </summary>
        public string Name { get; private set; }
        
        /// <summary>
        /// 对象。
        /// </summary>
        public T Target { get; private set; }

        /// <summary>
        /// 对象是否被加锁。
        /// </summary>
        public bool Locked { get; set; }

        /// <summary>
        /// 对象的优先级。
        /// </summary>
        public int Priority { get; set; }

        /// <summary>
        /// 对象上次使用时间。
        /// </summary>
        public DateTime LastUseTime { get; set; }
        
        /// <summary>
        /// 获取自定义释放检查标记。
        /// </summary>
        public virtual bool CustomCanReleaseFlag => true;
        
        
        public void Initialize(T target, string name = null, bool locked = false, int priority = 0)
        {
            Target = target ?? throw new Exception($"Target '{name}' 为空.");
            Name = name ?? string.Empty;
            Locked = locked;
            Priority = priority;
            LastUseTime = DateTime.UtcNow;
        }
        
        /// <summary>
        /// 获取对象时的事件。
        /// </summary>
        public virtual void OnSpawn()
        {
        }

        /// <summary>
        /// 回收对象时的事件。
        /// </summary>
        public virtual void OnUnSpawn()
        {
        }

        public void Initialize(object target, string name = null, bool locked = false, int priority = 0)
        {
            
        }

        public virtual void DeInitialize(bool isShutdown)
        {
        }

        // 其他成员（如 Clear/OnSpawn 等）保持不变
        public virtual void Reset()
        {
            Name = null;
            Target = null;  // 泛型默认值
            Locked = false;
            Priority = 0;
            LastUseTime = default;
        }
    }
}
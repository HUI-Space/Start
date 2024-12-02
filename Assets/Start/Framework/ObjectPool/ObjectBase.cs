using System;

namespace Start.Framework
{
    public abstract class ObjectBase: IReference
    {
        /// <summary>
        /// 对象名称。
        /// </summary>
        public string Name { get; private set; }
        
        /// <summary>
        /// 对象。
        /// </summary>
        public object Target { get; private set; }
        
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

        public void Initialize(object target,string name = default, bool locked = default , int priority = default)
        {
            Target = target ?? throw new Exception($"Target '{name}' is invalid.");
            Name = name ?? string.Empty;
            Locked = locked;
            Priority = priority;
            LastUseTime = DateTime.UtcNow;
        }
        
        /// <summary>
        /// 获取自定义释放检查标记。
        /// </summary>
        public virtual bool CustomCanReleaseFlag
        {
            get
            {
                return true;
            }
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
        
        public abstract void DeInitialize(bool isShutdown);
        
        public void Clear()
        {
            Name = default;
            Target = default;
            Locked = default;
            Priority = default;
            LastUseTime = default;
        }
    }
    
}
namespace Start
{
    /// <summary>
    /// 对象池对象基础接口
    /// </summary>
    public interface IObject
    {
        /// <summary>
        /// 从对象池获取对象时触发
        /// </summary>
        public virtual void OnSpawn()
        {
        }

        /// <summary>
        /// 回收对象到对象池时触发
        /// </summary>
        public virtual void OnUnSpawn()
        {
        }
        
        /// <summary>
        /// 对象被永久释放出对象池时触发（不再复用）
        /// </summary>
        public virtual void OnRelease()
        {
        }
    }
}
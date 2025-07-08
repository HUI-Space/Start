namespace Start
{
    public abstract class FsmBase
    {
        /// <summary>
        /// 获取有限状态机是否被销毁。
        /// </summary>
        public virtual bool IsDestroyed { get; private set; }
        
        /// <summary>
        /// 有限状态机轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">当前已流逝时间，以秒为单位。</param>
        internal abstract void Update(float elapseSeconds, float realElapseSeconds);

        /// <summary>
        /// 关闭并清理有限状态机。
        /// </summary>
        internal abstract void DeInitialize();
    }
}
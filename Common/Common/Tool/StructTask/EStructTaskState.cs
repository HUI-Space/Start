namespace Start
{
    /// <summary>
    /// 任务状态枚举
    /// </summary>
    public enum EStructTaskState : byte
    {
        /// <summary>
        /// 等待中
        /// </summary>
        Pending = 0,
        
        /// <summary>
        /// 成功
        /// </summary>
        Succeeded = 1,
        
        /// <summary>
        /// 失败
        /// </summary>
        Faulted = 2,
        
        /// <summary>
        /// 取消
        /// </summary>
        Canceled = 3,
    }
}

namespace Start
{
    /// <summary>
    /// 轻量级可等待任务配置
    /// </summary>
    public static class StructTaskOptions
    {
        /// <summary>
        /// 最大并发任务数（可外部配置）
        /// </summary>
        public static int MaxTasks { get; set; } = 1024;
    }
}

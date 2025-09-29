namespace Start
{
    public abstract class TaskBase : IReference
    {
        /// <summary>
        /// 任务的序列编号
        /// </summary>
        public int SerialId { get; private set; }

        /// <summary>
        /// 任务的优先级
        /// </summary>
        public int Priority { get; private set; }

        /// <summary>
        /// 任务的标签
        /// </summary>
        public string Tag { get; private set; }

        /// <summary>
        /// 任务的用户自定义数据
        /// </summary>
        public object UserData { get; private set; }

        /// <summary>
        /// 获取或设置任务是否完成。
        /// </summary>
        public bool Done { get; set; }
        
        /// <summary>
        /// 获取任务描述。
        /// </summary>
        public virtual string Description => null;

        protected void Initialize(int serialId, int priority, string tag, object userData)
        {
            SerialId = serialId;
            Priority = priority;
            Tag = tag;
            UserData = userData;
            Done = false;
        }

        public virtual void Clear()
        {
            SerialId = default;
            Tag = default;
            Priority = default;
            UserData = default;
            Done = default;
        }
    }
}
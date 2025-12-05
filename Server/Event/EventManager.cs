namespace Start.Server
{
    /// <summary>
    /// 事件管理器，继承自ManagerBase并实现IUpdateManger接口
    /// 负责游戏内事件的管理和调度
    /// </summary>
    public partial class EventManager : ManagerBase<EventManager>, IUpdateManger
    {
        /// <summary>
        /// 优先级属性，用于确定事件处理的顺序
        /// </summary>
        public override int Priority => 1;
    
        /// <summary>
        /// 用于存储消息类型的字典，键为消息类型，值为另一个字典，该字典的键为消息ID，值为委托
        /// </summary>
        private Dictionary<int,Dictionary<int,PriorityDelegate<IGenericData>>> _messageDic;
    
        /// <summary>
        /// 消息队列，用于存储待处理的事件
        /// </summary>
        private Queue<Event> _messageQueue;
        
        /// <summary>
        /// 初始化方法，用于在事件管理器初始化时分配必要的资源
        /// </summary>
        /// <returns>初始化任务</returns>
        public override Task Initialize()
        {
            _messageDic = new Dictionary<int, Dictionary<int, PriorityDelegate<IGenericData>>>();
            _messageQueue = new Queue<Event>();
            return base.Initialize();
        }
    
        /// <summary>
        /// 反初始化方法，用于在事件管理器关闭时释放资源
        /// </summary>
        /// <returns>反初始化任务</returns>
        public override Task DeInitialize()
        {
            foreach (Dictionary<int,PriorityDelegate<IGenericData>> dic in _messageDic.Values)
            {
                foreach (PriorityDelegate<IGenericData> priorityDelegate in dic.Values)
                {
                    RecyclableObjectPool.Recycle(priorityDelegate);
                }
            }
            lock (_messageQueue)
            {
                foreach (var Message in _messageQueue)
                {
                    RecyclableObjectPool.Recycle(Message);
                }
                _messageQueue.Clear();
                _messageQueue = default;
            }
            return base.DeInitialize();
        }
    
        /// <summary>
        /// 更新方法，每帧调用一次，用于处理消息队列中的事件
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间</param>
        /// <param name="realElapseSeconds">真实流逝时间</param>
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            lock (_messageQueue)
            {
                if (_messageQueue.Count > 0)
                {
                    Event e = _messageQueue.Dequeue();
                    SendMessage(e.MessageType, e.MessageId, e.Data);
                    RecyclableObjectPool.Recycle(e);
                }
            }
        }
    
        /// <summary>
        /// 添加事件监听器
        /// </summary>
        /// <param name="messageType">消息类型</param>
        /// <param name="messageId">消息ID</param>
        /// <param name="callback">回调函数</param>
        /// <param name="priority">优先级，默认为0</param>
        public void AddListener(int messageType, int messageId, Action<IGenericData> callback, int priority = 0)
        {
            PriorityDelegate<IGenericData> callBack = GetPriorityDelegate(messageType,messageId);
            callBack.AddListener(callback,priority);
        }
        
        /// <summary>
        /// 移除事件监听器
        /// </summary>
        /// <param name="messageType">消息类型</param>
        /// <param name="messageId">消息ID</param>
        /// <param name="callback">回调函数</param>
        public void RemoveListener(int messageType, int messageId, Action<IGenericData> callback)
        {
            PriorityDelegate<IGenericData> callBack = GetPriorityDelegate(messageType,messageId);
            if (callBack != null)
            {
                callBack.RemoveListener(callback);
                if (callBack.CanBeReleased)
                {
                    if (_messageDic.TryGetValue(messageType, out Dictionary<int, PriorityDelegate<IGenericData>> callBacks))
                    {
                        callBacks.Remove(messageId);
                    }
                    RecyclableObjectPool.Recycle(callBack);
                }
            }
        }
    
        /// <summary>
        /// 发送消息给所有监听该消息类型的监听器
        /// </summary>
        /// <param name="messageType">消息类型</param>
        /// <param name="messageId">消息ID</param>
        /// <param name="data">消息数据</param>
        public void SendMessage(int messageType, int messageId, IGenericData data)
        {
            PriorityDelegate<IGenericData> callBack = GetPriorityDelegate(messageType,messageId);
            callBack?.Invoke(data);
        }
        
        /// <summary>
        /// 延迟发送消息，将消息添加到消息队列中，等待Update方法处理
        /// </summary>
        /// <param name="messageType">消息类型</param>
        /// <param name="messageId">消息ID</param>
        /// <param name="data">消息数据</param>
        public void DelaySendMessage(int messageType, int messageId, IGenericData data)
        {
            lock (_messageQueue)
            {
                _messageQueue.Enqueue(Event.Create(messageType,messageId,data));
            }
        }
    
        /// <summary>
        /// 获取指定消息类型的委托
        /// </summary>
        /// <param name="messageType">消息类型</param>
        /// <param name="messageId">消息ID</param>
        /// <returns>委托对象</returns>
        private PriorityDelegate<IGenericData> GetPriorityDelegate(int messageType, int messageId)
        {
            if (!_messageDic.TryGetValue(messageType, out Dictionary<int, PriorityDelegate<IGenericData>> callBacks))
            {
                callBacks = new Dictionary<int, PriorityDelegate<IGenericData>>();
                _messageDic[messageType] = callBacks;
            }
            if (!callBacks.TryGetValue(messageId, out PriorityDelegate<IGenericData> callBack))
            {
                callBack = PriorityDelegate<IGenericData>.Create();
                callBacks[messageId] = callBack;
            }
            return callBack;
        }
    }
    
    public partial class EventManager
    {
        private class Event : IReusable
        {
            public int MessageType { get; private set; }
            public int MessageId { get; private set; }
            public IGenericData Data { get; private set; }
    
            public static Event Create(int MessageType, int messageId, IGenericData data)
            {
                Event e = RecyclableObjectPool.Acquire<Event>();
                e.MessageType = MessageType;
                e.MessageId = messageId;
                e.Data = data;
                return e;
            }
    
            public void Reset()
            {
                MessageId = default;
                Data = default;
            }
        }
    }
}

    
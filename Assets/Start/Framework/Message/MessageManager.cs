using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Start.Framework
{
    /// <summary>
    /// 消息管理器
    /// </summary>
    [Manager]
    public partial class MessageManager : ManagerBase<MessageManager>, IUpdateManger
    {
        public override int Priority => 4;

        private Dictionary<int,Dictionary<int,PriorityDelegate<IGenericData>>> _messageDic;
        private Queue<Message> _messageQueue;
        
        
        public override Task Initialize()
        {
            _messageDic = new Dictionary<int, Dictionary<int, PriorityDelegate<IGenericData>>>();
            _messageQueue = new Queue<Message>();
            return base.Initialize();
        }

        public override Task DeInitialize()
        {
            foreach (Dictionary<int,PriorityDelegate<IGenericData>> dic in _messageDic.Values)
            {
                foreach (PriorityDelegate<IGenericData> priorityDelegate in dic.Values)
                {
                    ReferencePool.Release(priorityDelegate);
                }
            }
            lock (_messageQueue)
            {
                foreach (var Message in _messageQueue)
                {
                    ReferencePool.Release(Message);
                }
                _messageQueue.Clear();
                _messageQueue = default;
            }
            return base.DeInitialize();
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            lock (_messageQueue)
            {
                if (_messageQueue.Count > 0)
                {
                    Message message = _messageQueue.Dequeue();
                    SendMessage(message.MessageType, message.MessageId, message.Data);
                    ReferencePool.Release(message);
                }
            }
        }

        public void AddListener(int messageType, int messageId, Action<IGenericData> callback, int priority = 0)
        {
            PriorityDelegate<IGenericData> callBack = GetPriorityDelegate(messageType,messageId);
            callBack.AddListener(callback,priority);
        }
        
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
                    ReferencePool.Release(callBack);
                }
            }
        }

        public void SendMessage(int messageType, int messageId, IGenericData data)
        {
            PriorityDelegate<IGenericData> callBack = GetPriorityDelegate(messageType,messageId);
            callBack?.Invoke(data);
        }
        
        public void DelaySendMessage(int messageType, int messageId, IGenericData data)
        {
            lock (_messageQueue)
            {
                _messageQueue.Enqueue(Message.Create(messageType,messageId,data));
            }
        }

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
}
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Start.Framework
{
    public abstract class ChannelBase : IChannel
    {
        public abstract EChannelType EChannelType { get; }
        protected abstract void Received(byte[] bytes, long offset, long count);
        
        public uint Id => _client.Id;

        public bool IsConnected => _client.IsConnected;
        
        public bool IsCacheCall => _callTaskCache.Count > 0;
        
        public event Action<IChannel> OnConnected;
        
        public event Action<IChannel> OnConnecting;
        
        public event Action<IChannel> OnDisconnected;
        
        public event Action<IChannel> OnDisconnecting;
        
        public event Action<IChannel, SocketError> OnError;
        
        public event Action<IChannel, uint, uint, byte[]> OnReadMessageCallByServer;
        
        public event Action<IChannel, HashSet<uint>> OnReceiveError;
        
        public event Action<IChannel> OnCacheCall;

        protected readonly Queue<byte[]> PacketQueue = new Queue<byte[]>();
        private readonly HashSet<uint> _receiveErrorSet = new HashSet<uint>();
        private readonly List<CallTask> _callTaskCache = new List<CallTask>();
        private readonly Dictionary<ulong, CallTask> _callTasks = new Dictionary<ulong, CallTask>();
        
        private readonly Dictionary<int, Action<uint, object>> _callbackTool = new Dictionary<int, Action<uint, object>>();
        private readonly Dictionary<uint, PriorityDelegate<uint, object>> _callbacks = new Dictionary<uint, PriorityDelegate<uint, object>>();
        
        private readonly IClient _client;
        protected readonly IChannelHelper ChannelHelper;
        
        private uint _serialNumber;
        
        protected ChannelBase(IClient client, IChannelHelper channelHelper)
        {
            ChannelHelper = channelHelper;
            _client = client;
            _client.OnConnected += Connected;
            _client.OnConnecting += Connecting;
            _client.OnDisconnected += Disconnected;
            _client.OnDisconnecting += Disconnecting;
            _client.OnReceived += Received;
            _client.OnError += Error;
        }
        
        public bool Connect()
        {
            try
            {
                return _client.ConnectAsync();
            }
            catch (Exception e)
            {
                Log.Error(e.Message);
                return false;
            }
        }

        public bool Reconnect()
        {
            return _client.ReconnectAsync();
        }

        public virtual void Update(float elapseSeconds, float realElapseSeconds)
        {
            while (PacketQueue.Count > 0)
            {
                var result = PacketQueue.Dequeue();
                ChannelHelper.DecodePacket(result, out var errCode, out var serialNumber, out var messageId, out var message);
                if (RemoveCall(serialNumber, out var callTask))
                {
                    callTask.SetResponse(errCode, message);
                }
                else
                {
                    OnReadMessageCallByServer?.Invoke(this, messageId, errCode, message);
                }
            }
        }
        
        public async Task<(uint, ToC)> Call<ToC, ToS>(ToS toS, bool isCache = true) where ToC : class where ToS : class
        {
            (uint code, ToC toC) result ;
            try
            {
                var messageId = ChannelHelper.GetMessageId(typeof(ToS));
                var message = ChannelHelper.Serialize(toS);
                CallTask callTask = CallTask.Create(_serialNumber++, messageId, message);
                SendAsync(callTask, isCache);
                await callTask.Task;
                result = (callTask.ErrCode, ChannelHelper.Deserialize<ToC>(callTask.Response));
                ReferencePool.Release(callTask);
            }
            catch (Exception e)
            {
                result = (ChannelHelper.ErrCode, default);
                Log.Error(e.Message);
            }
            return result;
        }

        public bool Disconnect()
        {
            var result = _client.Disconnect();
            ClearCacheCall();
            ClearCalls();
            return result;
        }
        
        #region 注册注销回调

        /// <summary>
        /// 注册回调
        /// </summary>
        /// <param name="handle">回调（错误码，错误信息）</param>
        /// <param name="priority">优先级</param>
        /// <typeparam name="T"></typeparam>
        public void Register<T>(Action<uint, T> handle, int priority) where T : class
        {
            Action<uint, object> callback = (code, msg) => { handle(code, msg as T); };
            _callbackTool[handle.GetHashCode()] = callback;
            uint messageId = ChannelHelper.GetMessageId(typeof(T));

            if (messageId == 0)
            {
                return;
            }
            
            if (!_callbacks.TryGetValue(messageId, out var list))
            {
                list = PriorityDelegate<uint, object>.Create();
                _callbacks.Add(messageId, list);
            }

            list.AddListener(callback, priority);
        }

        /// <summary>
        /// 注销回调
        /// </summary>
        /// <param name="handle">回调（错误码，错误信息）</param>
        /// <typeparam name="T"></typeparam>
        public void UnRegister<T>(Action<uint, T> handle) where T : class
        {
            uint messageId = ChannelHelper.GetMessageId(typeof(T));
            if (messageId == 0)
            {
                return;
            }
            if (_callbackTool.TryGetValue(handle.GetHashCode(), out var callback))
            {
                if (_callbacks.TryGetValue(messageId, out var list))
                {
                    list.RemoveListener(callback);
                    if (list.CanBeReleased)
                    {
                        _callbacks.Remove(messageId);
                        ReferencePool.Release(list);
                    }
                }
            }
        }

        #endregion
        
        #region CallTask
        
        public void SendCacheCall()
        {
            for (var i = _callTaskCache.Count - 1; i >= 0; i--)
            {
                var seq = (uint)i + 1;
                var callTask = _callTaskCache[i];
                callTask.SetSerialNumber(seq);
                _callTaskCache.RemoveAt(i);
                SendAsync(callTask, true);
            }
        }
        
        public void ClearCacheCall()
        {
            foreach (var callTask in _callTaskCache)
            {
                callTask.SetResponse(ChannelHelper.ErrCode, default);
            }

            _callTaskCache.Clear();
        }

        private void SendAsync(CallTask callTask, bool isCache)
        {
            if (isCache)
            {
                if (_client.IsConnected)
                {
                    var success = _client.SendAsync(ChannelHelper.EncodePacket(callTask.MessageId, callTask.Request,
                        callTask.SerialNumber));
                    if (success == false)
                        // -------------发送失败缓存消息
                        CacheCall(callTask);
                    else
                        // -------------成功发送等待消息返回
                        AddCall(callTask);
                }
                else
                {
                    // -------------未连接缓存消息
                    CacheCall(callTask);
                }
            }
            else
            {
                if (_client.IsConnected == false)
                {
                    callTask.SetResponse(ChannelHelper.ErrCode, default);
                }
                else
                {
                    var success = _client.SendAsync(ChannelHelper.EncodePacket(callTask.MessageId, callTask.Request,
                        callTask.SerialNumber));
                    if (success == false)
                    {
                        callTask.SetResponse(ChannelHelper.ErrCode, default);
                    }
                    else
                    {
                        AddCall(callTask);
                    }
                }
            }
        }

        /// <summary>
        /// 缓存call，等待重新发送
        /// </summary>
        /// <param name="callTask"></param>
        private void CacheCall(CallTask callTask)
        {
            _callTaskCache.Add(callTask);
            _callTaskCache.Sort();
            OnCacheCall?.Invoke(this);
        }

        /// <summary>
        /// 添加call，等待消息返回
        /// </summary>
        /// <param name="callTask"></param>
        private void AddCall(CallTask callTask)
        {
            lock (_callTasks)
            {
                _callTasks[callTask.SerialNumber] = callTask;
            }
        }

        private bool RemoveCall(uint serialNumber, out CallTask callTask)
        {
            lock (_callTasks)
            {
                if (!_callTasks.TryGetValue(serialNumber, out callTask)) return false;

                _callTasks.Remove(serialNumber);
                return true;
            }
        }

        private void ClearCalls()
        {
            lock (_callTasks)
            {
                foreach (var callTask in _callTasks.Values)
                {
                    callTask.SetResponse(ChannelHelper.ErrCode, default);
                }

                _callTasks.Clear();
            }
        }

        #endregion
        
        #region 事件
        
        private void Connected()
        {
            _serialNumber = 1;
            _callTasks.Clear();
            OnConnected?.Invoke(this);
        }

        private void Connecting()
        {
            OnConnecting?.Invoke(this);
        }

        private void Disconnecting()
        {
            OnDisconnecting?.Invoke(this);
        }

        private void Disconnected()
        {
            OnDisconnected?.Invoke(this);

            if (_callTasks.Count > 0)
            {
                _receiveErrorSet.Clear();

                foreach (var callTask in _callTasks.Values) _receiveErrorSet.Add(callTask.MessageId);

                ClearCalls();
                OnReceiveError?.Invoke(this, _receiveErrorSet);
            }
        }
        
        private void Error(SocketError obj)
        {
            OnError?.Invoke(this, obj);
        }
        
        #endregion
    }
}
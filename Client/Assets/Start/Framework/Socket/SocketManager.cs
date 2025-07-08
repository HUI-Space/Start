using System;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Start
{
    public partial class SocketManager : ManagerBase<SocketManager>, IUpdateManger
    {
        public override int Priority => 16;
        
        private readonly LinkedList<IChannel> _linkedList = new LinkedList<IChannel>();
        
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            LinkedListNode<IChannel> current = _linkedList.First;
            while (current != null)
            {
                current.Value.Update(elapseSeconds, realElapseSeconds);
                current = current.Next;
            }
        }
        
        public IChannel GetChannel(uint Id)
        {
            foreach (IChannel channel in _linkedList)
            {
                if (channel.Id == Id)
                {
                    return channel;
                }
            }
            return default;
        }
        
        public void Reconnect(uint Id)
        {
            IChannel channel = GetChannel(Id);
            channel.Reconnect();
        }
        
        public void Disconnect(uint Id)
        {
            IChannel channel = GetChannel(Id);
            channel.Reconnect();
            _linkedList.Remove(channel);
        }
    }

    #region API ---发送网络消息

    public partial class SocketManager
    {
        public async Task<(uint, ToC)> Call<ToC, ToS>(uint channelId, ToS toS, bool isCache = true) where ToC : class where ToS : class
        {
            IChannel channel = GetChannel(channelId);
            if (channel == null)
            {
                return default;
            }
            (uint errCode, ToC message) result = await channel.Call<ToC, ToS>(toS, isCache);
            // -----------------回调
            if (_callbacks.TryGetValue(channelId, out Dictionary<uint, PriorityDelegate<uint, object>> callbacks))
            {
                if (callbacks.TryGetValue(channel.ChannelHelper.GetMessageId(typeof(ToC)), out var actionList))
                {
                    actionList.Invoke(result.errCode, result.message);
                }
            }
            return result;
        }

        public async void Send<ToS>(uint channelId, ToS toS, bool isCache = true) where ToS : class
        {
            await Call<object, ToS>(channelId, toS, isCache);
        }
    }

    #endregion
    
    #region API ---TCP Channel

    public partial class SocketManager
    {
        public void TcpConnect(uint id, IPEndPoint endPoint,ITcpChannelHelper tcpChannelHelper)
        {
            IChannel channel = GetChannel(id);
            if (channel == null)
            {
                channel = new TcpChannel(id, endPoint, tcpChannelHelper);
                _linkedList.AddLast(channel);
            }
            channel.Connect();
        }
    }
    

    #endregion
    
    #region API ---KCP Channel

    public partial class SocketManager
    {
        public void KcpConnect(uint id, IPEndPoint endPoint, IChannelHelper channelHelper, IKcpHelper kcpHelper)
        {
            IChannel channel = GetChannel(id);
            if (channel == null)
            {
                channel = new KcpChannel(id, endPoint, channelHelper,kcpHelper);
                _linkedList.AddLast(channel);
            }
            channel.Connect();
        }
        
    }

    #endregion
    
    #region API ---UDP Channel

    public partial class SocketManager
    {
        public void UdpConnect(uint id, IPEndPoint endPoint,IChannelHelper channelHelper)
        {
            IChannel channel = GetChannel(id);
            if (channel == null)
            {
                channel = new UdpChannel(id, endPoint, channelHelper);
                _linkedList.AddLast(channel);
            }
            channel.Connect();
        }
    }

    #endregion
    
    #region API ---注册注销回调

    public partial class SocketManager
    {
        
        private readonly Dictionary<uint,Dictionary<int, Action<uint, object>>> _callbackTool 
            = new Dictionary<uint,Dictionary<int, Action<uint, object>>>();
        
        private readonly Dictionary<uint,Dictionary<uint, PriorityDelegate<uint, object>>> _callbacks
            = new Dictionary<uint,Dictionary<uint, PriorityDelegate<uint, object>>>();

        public void Register<T>(uint channelId,uint messageId, Action<uint, T> handle, int priority) where T : class
        {
            if (!_callbackTool.TryGetValue(channelId, out Dictionary<int, Action<uint, object>> callbackTool))
            {
                callbackTool = new Dictionary<int, Action<uint, object>>();
                _callbackTool[channelId] = callbackTool;
            }
            Action<uint, object> callback = (code, msg) => { handle(code, msg as T); };
            callbackTool[handle.GetHashCode()] = callback;
            
            if (!_callbacks.TryGetValue(channelId, out Dictionary<uint, PriorityDelegate<uint, object>> callbacks))
            {
                callbacks = new Dictionary<uint, PriorityDelegate<uint, object>>();
                _callbacks[channelId] = callbacks;
            }
            if (!callbacks.TryGetValue(messageId, out var priorityDelegate))
            {
                priorityDelegate = PriorityDelegate<uint, object>.Create();
                callbacks.Add(messageId, priorityDelegate);
            }
            priorityDelegate.AddListener(callback, priority);
        }
        
        public void UnRegister<T>(uint channelId,uint messageId, Action<uint, T> handle) where T : class
        {
            if (_callbackTool.TryGetValue(channelId, out Dictionary<int, Action<uint, object>> callbackTool))
            {
                if (callbackTool.TryGetValue(handle.GetHashCode(), out Action<uint, object> callback))
                {
                    if (_callbacks.TryGetValue(channelId, out Dictionary<uint, PriorityDelegate<uint, object>> callbacks))
                    {
                        if (callbacks.TryGetValue(messageId, out PriorityDelegate<uint, object> priorityDelegate))
                        {
                            priorityDelegate.RemoveListener(callback);
                            if (priorityDelegate.CanBeReleased)
                            {
                                callbacks.Remove(messageId);
                                callbackTool.Remove(handle.GetHashCode());
                                ReferencePool.Release(priorityDelegate);
                                if (callbacks.Count == 0)
                                {
                                    _callbacks.Remove(channelId);
                                }
                                if (callbackTool.Count == 0)
                                {
                                    _callbackTool.Remove(channelId);
                                }
                            }
                        }
                    }
                }
            }
        }
        
    }
    
    #endregion
}
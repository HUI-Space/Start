using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace Start.Framework
{
    [Manager]
    public partial class SocketManager : ManagerBase<SocketManager>, IUpdateManger
    {
        public override int Priority => 16;
        
        private readonly LinkedList<IChannel> _updateChannels = new LinkedList<IChannel>();
        private readonly Dictionary<uint, IChannel> _channels = new Dictionary<uint, IChannel>();
        
        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            LinkedListNode<IChannel> current = _updateChannels.First;
            while (current != null)
            {
                current.Value.Update(elapseSeconds, realElapseSeconds);
                current = current.Next;
            }
        }
        
        public IChannel GetChannel(uint id)
        {
            if (_channels.TryGetValue(id, out var channel)) return channel;
            return default;
        }
        
        public void Reconnect(uint id)
        {
            var channel = GetChannel(id);
            channel.Reconnect();
        }
        
        public void Disconnect(uint id)
        {
            if (_channels.TryGetValue(id, out var channel))
            {
                channel.Disconnect();
                _channels.Remove(id);
                _updateChannels.Remove(channel);
            }
        }
    }

    #region API ---发送网络消息

    public partial class SocketManager
    {
        public async Task<(uint, ToC)> Call<ToC, ToS>(ToS toS, uint channelId, bool isCache = true) where ToC : class where ToS : class
        {
            var channel = GetChannel(channelId);
            (uint errCode, ToC message) result = await channel.Call<ToC, ToS>(toS, isCache);
            return result;
        }

        public async void Send<ToS>(ToS toS, uint channelId, bool isCache = true) where ToS : class
        {
            await Call<object, ToS>(toS, channelId, isCache);
        }
    }

    #endregion

    #region API ---Event

    public partial class SocketManager
    {
        private void Connected(IChannel channel)
        {
        }

        private void Connecting(IChannel channel)
        {
            
        }

        private void Disconnected(IChannel channel)
        {
            
        }

        private void Disconnecting(IChannel channel)
        {
        }

        private void Error(IChannel channel, SocketError arg2)
        {
        }

        private void ReadMessageCallByServer(IChannel channel, uint arg2, uint arg3, byte[] arg4)
        {
            
        }

        private void ReceiveError(IChannel channel, HashSet<uint> arg2)
        {
            
        }

        private void CacheCall(IChannel channel)
        {
            
        }
    }

    #endregion
    
    #region API ---TCP Channel

    public partial class SocketManager
    {
        public void TcpConnect(uint id, IPEndPoint endPoint,ITcpChannelHelper tcpChannelHelper)
        {
            var channel = GetChannel(id) ?? CreateTcpChannel(id, endPoint,tcpChannelHelper);
            channel.Connect();
        }
        
        private IChannel CreateTcpChannel(uint id, IPEndPoint endPoint ,ITcpChannelHelper tcpChannelHelper)
        {
            IChannel channel = new TcpChannel(id, endPoint, tcpChannelHelper);
            
            channel.OnConnected += Connected;
            channel.OnConnecting += Connecting;
            channel.OnDisconnected += Disconnected;
            channel.OnDisconnecting += Disconnecting;
            channel.OnReadMessageCallByServer += ReadMessageCallByServer;
            channel.OnReceiveError += ReceiveError;
            channel.OnCacheCall += CacheCall;
            channel.OnError += Error;
            
            _channels.Add(id, channel);
            _updateChannels.AddLast(channel);
            
            return channel;
        }
    }
    

    #endregion
    
    #region API ---KCP Channel

    public partial class SocketManager
    {
        public void TcpConnect(uint id, IPEndPoint endPoint, IChannelHelper channelHelper, IKcpHelper kcpHelper)
        {
            var channel = GetChannel(id) ?? CreateKcpChannel(id, endPoint,channelHelper,kcpHelper);
            channel.Connect();
        }
        
        private IChannel CreateKcpChannel(uint id, IPEndPoint endPoint, IChannelHelper channelHelper , IKcpHelper kcpHelper)
        {
            IChannel channel = new KcpChannel(id, endPoint, channelHelper,kcpHelper);
            
            channel.OnConnected += Connected;
            channel.OnConnecting += Connecting;
            channel.OnDisconnected += Disconnected;
            channel.OnDisconnecting += Disconnecting;
            channel.OnReadMessageCallByServer += ReadMessageCallByServer;
            channel.OnReceiveError += ReceiveError;
            channel.OnCacheCall += CacheCall;
            channel.OnError += Error;
            
            _channels.Add(id, channel);
            _updateChannels.AddLast(channel);
            
            return channel;
        }
    }

    #endregion
    
    #region API ---UDP Channel

    public partial class SocketManager
    {
        public void UdpConnect(uint id, IPEndPoint endPoint,IChannelHelper channelHelper)
        {
            var channel = GetChannel(id) ?? CreateUdpChannel(id, endPoint,channelHelper);
            channel.Connect();
        }
        
        private IChannel CreateUdpChannel(uint id, IPEndPoint endPoint ,IChannelHelper channelHelper)
        {
            IChannel channel = new UdpChannel(id, endPoint, channelHelper);
            
            channel.OnConnected += Connected;
            channel.OnConnecting += Connecting;
            channel.OnDisconnected += Disconnected;
            channel.OnDisconnecting += Disconnecting;
            channel.OnReadMessageCallByServer += ReadMessageCallByServer;
            channel.OnReceiveError += ReceiveError;
            channel.OnCacheCall += CacheCall;
            channel.OnError += Error;
            
            _channels.Add(id, channel);
            _updateChannels.AddLast(channel);
            
            return channel;
        }
    }

    #endregion
    
    #region API ---注册注销回调

    public partial class SocketManager
    {
        /// <summary>
        /// 注册回调
        /// </summary>
        /// <param name="channelId">通道id</param>
        /// <param name="handle">回调（错误码，错误信息）</param>
        /// <param name="priority">优先级</param>
        /// <typeparam name="T"></typeparam>
        public void Register<T>(uint channelId, Action<uint, T> handle, int priority) where T : class
        {
            IChannel channel = GetChannel(channelId);
            channel.Register(handle, priority);
        }

        /// <summary>
        /// 注销回调
        /// </summary>
        /// <param name="channelId">通道id</param>
        /// <param name="handle">回调（错误码，错误信息）</param>
        /// <typeparam name="T"></typeparam>
        public void UnRegister<T>(uint channelId, Action<uint, T> handle) where T : class
        {
            IChannel channel = GetChannel(channelId);
            channel.UnRegister(handle);
        }
    }
    
    #endregion
}
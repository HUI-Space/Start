using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Start
{
    public interface IChannel
    {
        /// <summary>
        /// Id
        /// </summary>
        uint Id { get; }

        /// <summary>
        /// 连接
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// 是否有消息被缓存
        /// </summary>
        bool IsCacheCall { get; }
        /// <summary>
        /// 渠道类型
        /// </summary>
        EChannelType EChannelType { get; }
        
        /// <summary>
        /// 渠道辅助器
        /// </summary>
        IChannelHelper ChannelHelper { get; }

        /// <summary>
        /// 连接事件
        /// </summary>
        event Action<IChannel> OnConnected;

        /// <summary>
        /// 连接中事件
        /// </summary>
        event Action<IChannel> OnConnecting;

        /// <summary>
        /// 断开连接事件
        /// </summary>
        event Action<IChannel> OnDisconnected;

        /// <summary>
        /// 断开连接中事件
        /// </summary>
        event Action<IChannel> OnDisconnecting;

        /// <summary>
        /// 错误事件
        /// </summary>
        event Action<IChannel, SocketError> OnError;

        /// <summary>
        /// 读取服务器主动下的消息(通道，消息Id，错误码，消息)
        /// </summary>
        event Action<IChannel, uint, uint, byte[]> OnReadMessageCallByServer;

        /// <summary>
        /// 断线时仍有消息未返回 接收消息失败
        /// </summary>
        event Action<IChannel, HashSet<uint>> OnReceiveError;

        /// <summary>
        /// 当缓存消息时
        /// </summary>
        event Action<IChannel> OnCacheCall;

        /// <summary>
        /// 连接
        /// </summary>
        /// <returns></returns>
        bool Connect();

        /// <summary>
        /// 重连
        /// </summary>
        /// <returns></returns>
        bool Reconnect();

        /// <summary>
        /// 所有游戏框架模块轮询。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        void Update(float elapseSeconds, float realElapseSeconds);
        
        /// <summary>
        /// 发送消息等待回调
        /// </summary>
        /// <param name="toS">发送给服务器的消息</param>
        /// <param name="isCache">是否缓存</param>
        /// <typeparam name="ToC">服务器发送给客户端</typeparam>
        /// <typeparam name="ToS">客户端发送给服务器</typeparam>
        /// <returns></returns>
        Task<(uint, ToC)> Call<ToC, ToS>(ToS toS, bool isCache = true) where ToC : class where ToS : class;

        /// <summary>
        /// 发送缓存
        /// </summary>
        void SendCacheCall();

        /// <summary>
        /// 断开连接
        /// </summary>
        /// <returns></returns>
        bool Disconnect();
    }
}
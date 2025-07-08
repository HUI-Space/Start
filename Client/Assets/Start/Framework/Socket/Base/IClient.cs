using System;
using System.Net;
using System.Net.Sockets;

namespace Start
{
    public interface IClient : IDisposable
    {
        /// <summary>
        /// 网络频道Id
        /// </summary>
        uint Id { get; }

        /// <summary>
        /// 网络端点 (IP地址 和 端口号)
        /// </summary>
        IPEndPoint EndPoint { get; }

        /// <summary>
        /// 是否释放非托管资源
        /// </summary>
        bool IsDisposed { get; }

        /// <summary>
        /// 获取是否已连接。
        /// </summary>
        bool IsConnected { get; }

        /// <summary>
        /// 客户端发送的字节数
        /// </summary>
        long BytesSent { get; }

        /// <summary>
        /// 客户端发送的待处理字节数
        /// </summary>
        long BytesPending { get; }

        /// <summary>
        /// 客户端发送的字节数
        /// </summary>
        long BytesSending { get; }

        /// <summary>
        /// 客户端接收到的字节数
        /// </summary>
        long BytesReceived { get; }

        /// <summary>
        /// 接收缓冲区最小限制
        /// </summary>
        int OptionReceiveBufferLimit { get; }

        /// <summary>
        /// 接收缓冲区最大限制
        /// </summary>
        int OptionReceiveBufferSize { get; }

        /// <summary>
        /// 发送缓冲区最小限制
        /// </summary>
        int OptionSendBufferLimit { get; }

        /// <summary>
        /// 发送缓冲区最大限制
        /// </summary>
        int OptionSendBufferSize { get; }

        /// <summary>
        /// 连接中
        /// </summary>
        event Action OnConnecting;

        /// <summary>
        /// 连接成功
        /// </summary>
        event Action OnConnected;

        /// <summary>
        /// 接受到buffer
        /// </summary>
        event Action<byte[], long, long> OnReceived;

        /// <summary>
        /// 断开连接中
        /// </summary>
        event Action OnDisconnecting;

        /// <summary>
        /// 断开连接
        /// </summary>
        event Action OnDisconnected;

        /// <summary>
        /// Socket Error
        /// </summary>
        event Action<SocketError> OnError;

        /// <summary>
        /// 连接到远程主机。
        /// </summary>
        bool ConnectAsync();

        /// <summary>
        /// 向远程主机发送消息。
        /// </summary>
        /// <param name="buffer">消息</param>
        bool SendAsync(byte[] buffer);

        /// <summary>
        /// 重连
        /// </summary>
        /// <returns></returns>
        bool ReconnectAsync();

        /// <summary>
        /// 关闭网络频道。
        /// </summary>
        bool Disconnect();
    }
}
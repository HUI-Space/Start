using System;

namespace Start
{
    public interface IChannelHelper
    {
        /// <summary>
        /// 错误码
        /// </summary>
        uint ErrCode { get; }

        /// <summary>
        /// 获取消息Id
        /// </summary>
        /// <param name="type">消息类型</param>
        /// <returns></returns>
        uint GetMessageId(Type type);

        /// <summary>
        /// 序列化
        /// </summary>
        /// <param name="t">对象</param>
        /// <typeparam name="T">类型</typeparam>
        /// <returns></returns>
        byte[] Serialize<T>(T t);

        /// <summary>
        /// 反序列化
        /// </summary>
        /// <param name="data">字节</param>
        /// <typeparam name="T">对象类型</typeparam>
        /// <returns></returns>
        T Deserialize<T>(byte[] data);
        
        /// <summary>
        /// 编码
        /// </summary>
        /// <param name="messageId">消息Id</param>
        /// <param name="message">消息</param>
        /// <param name="serialNumber">序列号</param>
        /// <returns></returns>
        byte[] EncodePacket(uint messageId, byte[] message, uint serialNumber);

        /// <summary>
        /// 解码
        /// </summary>
        /// <param name="bytes">数据</param>
        /// <param name="errCode">错误码</param>
        /// <param name="serialNumber">序列号</param>
        /// <param name="messageId">消息Id</param>
        /// <param name="message">消息</param>
        void DecodePacket(byte[] bytes, out uint errCode, out uint serialNumber, out uint messageId, out byte[] message);
    }
}
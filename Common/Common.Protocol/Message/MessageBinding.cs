using System;
using System.Collections.Generic;

namespace Start
{
    /// <summary>
    /// 协议ID
    /// </summary>
    public enum EMessageID
    {
        None = 0,
        Login_C2S,
        Login_S2C,
        Ping_C2S,
        Ping_S2C,
        RoomMatch_C2S,
        RoomMatch_S2C,
        RoomMatchSuccess_S2C,
        RoomReady_C2S,
        RoomStart_S2C,
        RoomFrame_C2S,
        RoomFrame_S2C,
        RoomUpdateTime_S2C,
        RoomCheckHash_S2C,
        RoomCheckHashFail_S2C,
        RoomReconnect_S2C,
    }
    public static class MessageBinding
    {
        private static readonly Dictionary<EMessageID, Type> _messageToTypeDic = new Dictionary<EMessageID, Type>();
        private static readonly Dictionary<Type, EMessageID> _typeToMessageDic = new Dictionary<Type, EMessageID>();

        static MessageBinding()
        {
            _messageToTypeDic.Add(EMessageID.Login_C2S, typeof(Login_C2S));
            _messageToTypeDic.Add(EMessageID.Login_S2C, typeof(Login_S2C));
            _messageToTypeDic.Add(EMessageID.Ping_C2S, typeof(Ping_C2S));
            _messageToTypeDic.Add(EMessageID.Ping_S2C, typeof(Ping_S2C));
            _messageToTypeDic.Add(EMessageID.RoomMatch_C2S, typeof(RoomMatch_C2S));
            _messageToTypeDic.Add(EMessageID.RoomMatch_S2C, typeof(RoomMatch_S2C));
            _messageToTypeDic.Add(EMessageID.RoomMatchSuccess_S2C, typeof(RoomMatchSuccess_S2C));
            _messageToTypeDic.Add(EMessageID.RoomReady_C2S, typeof(RoomReady_C2S));
            _messageToTypeDic.Add(EMessageID.RoomStart_S2C, typeof(RoomStart_S2C));
            _messageToTypeDic.Add(EMessageID.RoomFrame_C2S, typeof(RoomFrame_C2S));
            _messageToTypeDic.Add(EMessageID.RoomFrame_S2C, typeof(RoomFrame_S2C));
            _messageToTypeDic.Add(EMessageID.RoomUpdateTime_S2C, typeof(RoomUpdateTime_S2C));
            _messageToTypeDic.Add(EMessageID.RoomCheckHash_S2C, typeof(RoomCheckHash_S2C));
            _messageToTypeDic.Add(EMessageID.RoomCheckHashFail_S2C, typeof(RoomCheckHashFail_S2C));
            _messageToTypeDic.Add(EMessageID.RoomReconnect_S2C, typeof(Reconnect_S2C));
            
            _typeToMessageDic.Add(typeof(Login_C2S), EMessageID.Login_C2S);
            _typeToMessageDic.Add(typeof(Login_S2C), EMessageID.Login_S2C);
            _typeToMessageDic.Add(typeof(Ping_C2S), EMessageID.Ping_C2S);
            _typeToMessageDic.Add(typeof(Ping_S2C), EMessageID.Ping_S2C);
            _typeToMessageDic.Add(typeof(RoomMatch_C2S), EMessageID.RoomMatch_C2S);
            _typeToMessageDic.Add(typeof(RoomMatch_S2C), EMessageID.RoomMatch_S2C);
            _typeToMessageDic.Add(typeof(RoomMatchSuccess_S2C), EMessageID.RoomMatchSuccess_S2C);
            _typeToMessageDic.Add(typeof(RoomReady_C2S), EMessageID.RoomReady_C2S);
            _typeToMessageDic.Add(typeof(RoomStart_S2C), EMessageID.RoomStart_S2C);
            _typeToMessageDic.Add(typeof(RoomFrame_C2S), EMessageID.RoomFrame_C2S);
            _typeToMessageDic.Add(typeof(RoomFrame_S2C), EMessageID.RoomFrame_S2C);
            _typeToMessageDic.Add(typeof(RoomUpdateTime_S2C), EMessageID.RoomUpdateTime_S2C);
            _typeToMessageDic.Add(typeof(RoomCheckHash_S2C), EMessageID.RoomCheckHash_S2C);
            _typeToMessageDic.Add(typeof(RoomCheckHashFail_S2C), EMessageID.RoomCheckHashFail_S2C);
            _typeToMessageDic.Add(typeof(Reconnect_S2C), EMessageID.RoomReconnect_S2C);
        }
        
        /// <summary>
        /// 根据MessageID获取类型
        /// </summary>
        public static Type GetTypeByMessageID(EMessageID messageId)
        {
            return _messageToTypeDic.GetValueOrDefault(messageId);
        }

        /// <summary>
        /// 根据Type获取MessageID
        /// </summary>
        public static EMessageID GetMessageIDByType(Type type)
        {
            EMessageID id;
            _typeToMessageDic.TryGetValue(type, out id);
            return id;
        }
    }
}
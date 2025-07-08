using System;
using System.Collections.Generic;

namespace Start
{
    public enum EMessageID
    {
        HeartBeat = 1,
    }
    
    public static class MessageBinding
    {
        private static Dictionary<EMessageID, Type> messageToTypeDic = new Dictionary<EMessageID, Type>();
        private static Dictionary<Type, EMessageID> typeToMessageDic = new Dictionary<Type, EMessageID>();

        static MessageBinding()
        {
            typeToMessageDic.Add(typeof(HeartBeat_ToS), EMessageID.HeartBeat);
            
            messageToTypeDic.Add(EMessageID.HeartBeat, typeof(HeartBeat_ToC));
        }
        
        /// <summary>
        /// 根据MessageID获取类型
        /// </summary>
        public static Type GetTypeByMessageID(EMessageID tcp)
        {
            Type type = null;
            messageToTypeDic.TryGetValue(tcp, out type);
            if (type == null)
            {
            }
            return type;
        }

        /// <summary>
        /// 根据Type获取MessageID
        /// </summary>
        public static EMessageID GetMessageIDByType(Type type)
        {
            EMessageID id;
            typeToMessageDic.TryGetValue(type, out id);
            return id;
        }
    }

    [Serializable]
    public class HeartBeat_ToS
    {
        public int Id;
    }
    
    [Serializable]
    public class HeartBeat_ToC
    {
        public int Id;
    }
}
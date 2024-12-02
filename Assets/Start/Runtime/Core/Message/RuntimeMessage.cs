using System;
using Start.Framework;

namespace Start.Runtime
{
    public static class RuntimeMessage
    {
        public static void AddListener(int messageId, Action<IGenericData> callback, int priority = 0)
        {
            MessageManager.Instance.AddListener((int)EMessageType.Runtime, messageId, callback,priority);
        }

        public static void RemoveListener(int messageId, Action<IGenericData> callback)
        {
            MessageManager.Instance.RemoveListener((int)EMessageType.Runtime, messageId, callback);
        }

        public static void SendMessage(int messageId, IGenericData data)
        {
            MessageManager.Instance.SendMessage((int)EMessageType.Runtime, messageId, data);
        }
        
        public static void DelaySendMessage(int messageId, IGenericData data)
        {
            MessageManager.Instance.DelaySendMessage((int)EMessageType.Runtime, messageId, data);
        }
    }
}
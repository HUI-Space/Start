using System;

namespace Start
{
    public static class RuntimeEvent
    {
        public static void AddListener(int messageId, Action<IGenericData> callback, int priority = 0)
        {
            EventManager.Instance.AddListener((int)EEventType.Runtime, messageId, callback,priority);
        }

        public static void RemoveListener(int messageId, Action<IGenericData> callback)
        {
            EventManager.Instance.RemoveListener((int)EEventType.Runtime, messageId, callback);
        }

        public static void SendMessage(int messageId, IGenericData data)
        {
            EventManager.Instance.SendMessage((int)EEventType.Runtime, messageId, data);
        }
        
        public static void DelaySendMessage(int messageId, IGenericData data)
        {
            EventManager.Instance.DelaySendMessage((int)EEventType.Runtime, messageId, data);
        }
    }
}
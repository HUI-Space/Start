namespace Start
{
    public static partial class UIActionTypes
    {
        public static string EventPanel_AddListener = nameof(EventPanel_AddListener);
        public static string EventPanel_RemoveListener = nameof(EventPanel_RemoveListener);
        public static string EventPanel_SendMessage = nameof(EventPanel_SendMessage);
        public static string EventPanel_DelaySendMessage = nameof(EventPanel_DelaySendMessage);
        
    }
    public static partial class UIActions
    {
        
        public static void EventPanel_AddListener(int messageType,int messageId)
        {
            UIAction.Create(nameof(EventPanel), UIActionTypes.EventPanel_AddListener)
                .SetData(messageType, messageId)
                .Dispatch();
        }
        
        public static void EventPanel_RemoveListener(int messageType,int messageId)
        {
            UIAction.Create(nameof(EventPanel), UIActionTypes.EventPanel_RemoveListener)
                .SetData(messageType, messageId)
                .Dispatch();
        }
        
        public static void EventPanel_SendMessage(int messageType,int messageId,bool isDelay,string message)
        {
            UIAction.Create(nameof(EventPanel), UIActionTypes.EventPanel_SendMessage)
                .SetData(messageType, messageId, isDelay, message)
                .Dispatch();
        }
        
        public static void EventPanel_DelaySendMessage(int messageType,int messageId,bool isDelay,string message)
        {
            UIAction.Create(nameof(EventPanel), UIActionTypes.EventPanel_DelaySendMessage)
                .SetData(messageType, messageId, isDelay, message)
                .Dispatch();
        }
    }
}
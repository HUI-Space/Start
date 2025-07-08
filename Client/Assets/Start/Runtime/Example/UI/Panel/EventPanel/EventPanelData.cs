using System.Collections.Generic;

namespace Start
{
    public class EventPanelData : UIData
    {
        public override string UIName => nameof(EventPanel);

        public readonly Dictionary<int,HashSet<int>> AddListenerMessageInfo = new Dictionary<int, HashSet<int>>();
        
        public int MessageType { get; private set; }
        public int MessageId { get; private set; }
        public bool IsDelay { get; private set; }
        public string Message { get; private set; }
        
        public override void Initialize()
        {
            base.Initialize();
            Register(UIActionTypes.EventPanel_AddListener, AddListener);
            Register(UIActionTypes.EventPanel_RemoveListener, RemoveListener);
            Register(UIActionTypes.EventPanel_SendMessage, SendMessage);
            Register(UIActionTypes.EventPanel_DelaySendMessage, DelaySendMessage);
        }

        private void AddListener(UIAction uiAction)
        {
            int messageType = uiAction.GetData1<int>();
            int messageId = uiAction.GetData2<int>();
            if (AddListenerMessageInfo.TryGetValue(messageType, out var value))
            {
                if (value.Add(messageId))
                {
                    EventManager.Instance.AddListener(messageType,messageId,EventCallback);
                }
            }
            else
            {
                AddListenerMessageInfo.Add(messageType, new HashSet<int>() {messageId});
                EventManager.Instance.AddListener(messageType,messageId,EventCallback);
            }
            IsDirty = true;
        }
        
        private void RemoveListener(UIAction uiAction)
        {
            int messageType = uiAction.GetData1<int>();
            int messageId = uiAction.GetData2<int>();
            if (AddListenerMessageInfo.TryGetValue(messageType, out var value))
            {
                if (value.Remove(messageId))
                {
                    EventManager.Instance.RemoveListener(messageType,messageId,EventCallback);
                }
            }
            if (value.Count == 0)
            {
                AddListenerMessageInfo.Remove(messageType);
            }
            IsDirty = true;
        }
        
        private void SendMessage(UIAction uiAction)
        {
            MessageType = default;
            MessageId = default;
            IsDelay = default;
            Message = default;
            IsDirty = true;
            int messageType = uiAction.GetData1<int>();
            int messageId = uiAction.GetData2<int>();
            bool isDelay = uiAction.GetData3<bool>();
            string message = uiAction.GetData4<string>();
            GenericData<int,int,bool,string> genericData = GenericData<int,int,bool,string>.Create(messageType,messageId,isDelay,message);
            EventManager.Instance.SendMessage(messageType,messageId,genericData);
        }
        
        private void DelaySendMessage(UIAction uiAction)
        {
            MessageType = default;
            MessageId = default;
            IsDelay = default;
            Message = default;
            IsDirty = true;
            int messageType = uiAction.GetData1<int>();
            int messageId = uiAction.GetData2<int>();
            bool isDelay = uiAction.GetData3<bool>();
            string message = uiAction.GetData4<string>();
            GenericData<int,int,bool,string> genericData = GenericData<int,int,bool,string>.Create(messageType,messageId,isDelay,message);
            EventManager.Instance.DelaySendMessage(messageType,messageId,genericData);
        }
        
        private void EventCallback(IGenericData genericData)
        {
            MessageType = genericData.GetData1<int>();
            MessageId = genericData.GetData2<int>();
            IsDelay = genericData.GetData3<bool>();
            Message = genericData.GetData4<string>();
            IsDirty = true;
        }
    }
}
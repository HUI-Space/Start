using System.Collections.Generic;
using System.Text;
using UnityEngine.UI;

namespace Start
{
    public class EventPanel : UIPanel<EventPanelData>
    {
        public Text TitleText;
        public Text MessageInfoTitleText;
        public Text MessageInfoText;
        public Text MessageTitleText;
        public Text MessageText;
        
        public InputField MessageType;
        public InputField MessageId;
        
        public InputField MessageData;
        public InputField ListenerMessageType;
        public InputField ListenerMessageId;

        public Text AddListenerButtonText;
        public Text RemoveListenerButtonText;
        public Text SendMessageButtonText;
        public Text DelaySendMessageButtonText;
        
        public Button AddListenerButton;
        public Button RemoveListenerButton;
        
        public Button SendMessageButton;
        public Button DelaySendMessageButton;
        
        public Button CloseButton;
        public Button RuleButton;
        
        private readonly Dictionary<int,HashSet<int>> _addListenerInfo = new Dictionary<int, HashSet<int>>();
        private StringBuilder _stringBuilder = new StringBuilder();
        
        public override void Initialize()
        {
            base.Initialize();
            AddListenerButton.onClick.AddListener(OnAddListenerButtonClick);
            RemoveListenerButton.onClick.AddListener(OnRemoveListenerButtonClick);
            SendMessageButton.onClick.AddListener(OnSendMessageButtonClick);
            DelaySendMessageButton.onClick.AddListener(OnDelaySendMessageButtonClick);
            CloseButton.onClick.AddListener(OnCloseButtonClick);
            RuleButton.onClick.AddListener(OnRuleButtonClick);
            TitleText.text = LocalizationManager.Instance.GetString(600);
            MessageType.placeholder.GetComponent<Text>().text = LocalizationManager.Instance.GetString(603);
            MessageId.placeholder.GetComponent<Text>().text = LocalizationManager.Instance.GetString(604);
            ListenerMessageType.placeholder.GetComponent<Text>().text = LocalizationManager.Instance.GetString(605);
            ListenerMessageId.placeholder.GetComponent<Text>().text = LocalizationManager.Instance.GetString(606);
            MessageData.placeholder.GetComponent<Text>().text = LocalizationManager.Instance.GetString(607);
            AddListenerButtonText.text = LocalizationManager.Instance.GetString(608);
            RemoveListenerButtonText.text = LocalizationManager.Instance.GetString(609);
            SendMessageButtonText.text = LocalizationManager.Instance.GetString(610);
            DelaySendMessageButtonText.text = LocalizationManager.Instance.GetString(611);
            MessageInfoTitleText.text = LocalizationManager.Instance.GetString(612);
            MessageTitleText.text = LocalizationManager.Instance.GetString(613);
        }

        protected override void Render(EventPanelData uiData)
        {
            base.Render(uiData);
            MessageText.text = $"MessageType:{uiData.MessageType}\nMessageId:{uiData.MessageId}\nDelay:{uiData.IsDelay}\nMessage:{uiData.Message}";
            _stringBuilder.Clear();
            foreach (var item in uiData.AddListenerMessageInfo)
            {
                _stringBuilder.AppendFormat($"MessageType:{item.Key}");
                _stringBuilder.AppendLine();
                _stringBuilder.Append("MessageId:");
                foreach (var data in item.Value)
                {
                    _stringBuilder.Append($" {data}");
                }
                _stringBuilder.AppendLine();
            }
            MessageInfoText.text = _stringBuilder.ToString();
        }

        public override void DeInitialize()
        {
            base.DeInitialize();
            AddListenerButton.onClick.RemoveListener(OnAddListenerButtonClick);
            RemoveListenerButton.onClick.RemoveListener(OnRemoveListenerButtonClick);
            SendMessageButton.onClick.RemoveListener(OnSendMessageButtonClick);
            DelaySendMessageButton.onClick.RemoveListener(OnDelaySendMessageButtonClick);
            CloseButton.onClick.RemoveListener(OnCloseButtonClick);
            RuleButton.onClick.RemoveListener(OnRuleButtonClick);
        }
        
        private void OnAddListenerButtonClick()
        {
            if (int.TryParse(MessageType.text,out int messageType) && int.TryParse(MessageId.text,out int messageId))
            {
                UIActions.EventPanel_AddListener(messageType,messageId);
            }
        }
        
        private void OnRemoveListenerButtonClick()
        {
            if (int.TryParse(MessageType.text,out int messageType) && int.TryParse(MessageId.text,out int messageId))
            {
                UIActions.EventPanel_RemoveListener(messageType,messageId);
            }
        }

        private void OnSendMessageButtonClick()
        {
            if (int.TryParse(ListenerMessageType.text,out int messageType) && int.TryParse(ListenerMessageId.text,out int messageId))
            {
                UIActions.EventPanel_SendMessage(messageType,messageId,false,MessageData.text);
            }
        }
        
        private void OnDelaySendMessageButtonClick()
        {
            if (int.TryParse(ListenerMessageType.text,out int messageType) && int.TryParse(ListenerMessageId.text,out int messageId))
            {
                UIActions.EventPanel_DelaySendMessage(messageType,messageId,true,MessageData.text);
            }
        }
        
        private void OnCloseButtonClick()
        {
            Close();
        }
        
        private void OnRuleButtonClick()
        {
            UIActions.OpenRulePopup(601, 602);
        }
    }
}
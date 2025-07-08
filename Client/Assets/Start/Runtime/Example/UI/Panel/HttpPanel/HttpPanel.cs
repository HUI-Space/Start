using System.Text;
using UnityEngine.UI;

namespace Start
{
    public class HttpPanel : UIPanel<HttpPanelData>
    {
        public Text TitleText;
        public Text GetTitleText;
        public Text PostTitleText;
        public Text PostMessageTitleText;
        public Text GetButtonText;
        public Text PostButtonText;
        public Text GetInputFieldText;
        public Text PostInputFieldText;
        public Text PostMessageInputFieldText;
        public Text LogText;
        
        public InputField GetInputField;
        public InputField PostInputField;
        public InputField PostMessageInputField;
        
        public Button GetButton;
        public Button PostButton;
        public Button CloseButton;
        public Button QuestionButton;


        public override void Initialize()
        {
            base.Initialize();
            TitleText.text = LocalizationManager.Instance.GetString(1000);
            GetTitleText.text = LocalizationManager.Instance.GetString(1001);
            PostTitleText.text = LocalizationManager.Instance.GetString(1002);
            PostMessageTitleText.text = LocalizationManager.Instance.GetString(1003);
            GetButtonText.text = LocalizationManager.Instance.GetString(1004);
            PostButtonText.text = LocalizationManager.Instance.GetString(1005);
            GetInputFieldText.text = LocalizationManager.Instance.GetString(1006);
            PostInputFieldText.text = LocalizationManager.Instance.GetString(1007);
            PostMessageInputFieldText.text = LocalizationManager.Instance.GetString(1008);
            
            LogText.text = string.Empty;
            GetButton.onClick.AddListener(OnGetButtonClick);
            PostButton.onClick.AddListener(OnPostButtonClick);
            CloseButton.onClick.AddListener(OnCloseButtonClick);
            QuestionButton.onClick.AddListener(OnQuestionButtonClick);
        }
        

        public override void DeInitialize()
        {
            base.DeInitialize();
            GetButton.onClick.RemoveListener(OnGetButtonClick);
            PostButton.onClick.RemoveListener(OnPostButtonClick);
            CloseButton.onClick.RemoveListener(OnCloseButtonClick);
            QuestionButton.onClick.RemoveListener(OnQuestionButtonClick);
        }
        
        private async void OnGetButtonClick()
        {
            HttpResponse httpResponse = await HttpManager.Instance.Get(GetInputField.text);
            LogText.text = httpResponse.Result != null ? Encoding.UTF8.GetString(httpResponse.Result) : string.Empty;
        }
        
        private async void OnPostButtonClick()
        {
            if (string.IsNullOrEmpty(PostInputField.text) && string.IsNullOrEmpty(PostMessageInputField.text))
            {
                return;
            }
            HttpResponse httpResponse = await HttpManager.Instance.Post(PostInputField.text, Encoding.UTF8.GetBytes(PostMessageInputField.text));
            LogText.text = httpResponse.Result != null ? httpResponse.Result.ToString() : string.Empty;
        }
        
        private void OnCloseButtonClick()
        {
            Close();
        }
        
        private void OnQuestionButtonClick()
        {
            UIActions.OpenRulePopup(1009, 1010);
        }
    }
}
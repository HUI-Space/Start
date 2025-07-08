using UnityEngine.UI;

namespace Start
{
    public class LogPanelItem : UIElement<ELogType>
    {
        public Text TitleText;
        public Text ButtonText;
        public Text InputFieldText;
        public InputField InputField;
        public Button Button;

        public override void Initialize()
        {
            base.Initialize();
            Button.onClick.AddListener(OnButtonClick);
            ButtonText.text = LocalizationManager.Instance.GetString(1101);
            InputFieldText.text = LocalizationManager.Instance.GetString(1101);
        }
        
        protected override void Render(ELogType data)
        {
            TitleText.text = data.ToString();
        }

        public override void DeInitialize()
        {
            base.DeInitialize();
            Button.onClick.RemoveListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            if (!string.IsNullOrEmpty(InputField.text))
            {
                Logger.Log(Value, InputField.text);
                UIActions.LogPanel_Log($"{Value.ToString()}:{InputField.text}");
            }
        }
    }
}
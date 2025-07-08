using System.Globalization;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Start
{
    public class SettingPanelItem : UIElement<SettingPanelItem.SettingType>
    {
        public enum SettingType
        {
            Bool,
            Int,
            Float,
            String,
        }
        
        public Text GetTitleText;
        public Text GetInputFieldText;
        public Text ResultTitleText;
        public Text ResultText;
        public Text GetButtonText;
        public Text SetTitleText;
        public Text SetInputFieldText;
        public Text SetButtonText;
        
        public InputField GetInputField;
        public InputField SetInputField;
        public Button GetButton;
        public Button SetButton;
        
        public override void Initialize()
        {
            base.Initialize();
            GetInputFieldText.text = LocalizationManager.Instance.GetString(1201);
            SetInputFieldText.text = LocalizationManager.Instance.GetString(1201);
            ResultTitleText.text = LocalizationManager.Instance.GetString(1202);
            GetButtonText.text = LocalizationManager.Instance.GetString(1203);
            SetButtonText.text = LocalizationManager.Instance.GetString(1204);
            SetTitleText.text = LocalizationManager.Instance.GetString(1205);

            GetButton.onClick.AddListener(OnGetButtonClick);
            SetButton.onClick.AddListener(OnSetButtonClick);
        }

        protected override void Render(SettingType data)
        {
            GetTitleText.text = LocalizationManager.Instance.GetString(1213) + data.ToString();
            SetTitleText.text = LocalizationManager.Instance.GetString(1214) + data.ToString();
        }

        public override void DeInitialize()
        {
            base.DeInitialize();
            GetButton.onClick.RemoveListener(OnGetButtonClick);
            SetButton.onClick.RemoveListener(OnSetButtonClick);
        }
        
        private void OnGetButtonClick()
        {
            if (!string.IsNullOrEmpty(GetInputField.text))
            {
                switch (Value)
                {
                    case SettingType.Bool:
                        ResultText.text = SettingManager.Instance.GetBool(GetInputField.text, false).ToString();
                        break;
                    case SettingType.Int:
                        ResultText.text = SettingManager.Instance.GetInt(GetInputField.text, 0).ToString();
                        break;
                    case SettingType.Float:
                        ResultText.text = SettingManager.Instance.GetFloat(GetInputField.text, 0).ToString(CultureInfo.InvariantCulture);
                        break;
                    case SettingType.String:
                        ResultText.text = SettingManager.Instance.GetString(GetInputField.text, string.Empty);
                        break;
                }
            }
            else
            {
                ResultText.text = string.Empty;
            }
        }
        
        private void OnSetButtonClick()
        {
            if (!string.IsNullOrEmpty(SetInputField.text))
            {
                switch (Value)
                {
                    case SettingType.Bool:
                        if (bool.TryParse(SetInputField.text,out bool boolResult))
                        {
                            SettingManager.Instance.SetBool(GetInputField.text, boolResult);
                        }
                        break;
                    case SettingType.Int:
                        if (int.TryParse(SetInputField.text,out int intResult))
                        {
                            SettingManager.Instance.SetInt(GetInputField.text, intResult);
                        }
                        break;
                    case SettingType.Float:
                        if (float.TryParse(SetInputField.text,out float floatResult))
                        {
                            SettingManager.Instance.SetFloat(GetInputField.text, floatResult);
                        }
                        break;
                    case SettingType.String:
                        SettingManager.Instance.SetString(GetInputField.text, SetInputField.text);
                        break;
                }
            }
            ResultText.text = string.Empty;
        }
    }
}
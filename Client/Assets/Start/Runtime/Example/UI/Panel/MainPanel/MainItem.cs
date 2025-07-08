
using UnityEngine.UI;

namespace Start
{
    public class MainItem : UIElement<ExampleConfigItem>
    {
        public Button Button;
        public Text Text;
        private string _uiName;

        public override void Initialize()
        {
            Button.onClick.AddListener(OnButtonClick);
        }

        public override void DeInitialize()
        {
            Button.onClick.RemoveListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            if (!string.IsNullOrEmpty(_uiName))
            {
                UIActions.OpenUI(_uiName);
            }
        }

        protected override void Render(ExampleConfigItem data)
        {
            _uiName = data.UIName;
            Text.text = LocalizationManager.Instance.GetString(data.LocalizationId);
        }
    }
}
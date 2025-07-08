using UnityEngine.UI;

namespace Start
{
    public class RulePopup : UIPopup<RulePopupData>
    {
        public Text TitleText;
        public Text RuleText;
        
        public Button CloseButton;
        public override void Initialize()
        {
            base.Initialize();
            CloseButton.onClick.AddListener(OnCloseButtonClick);
        }

        protected override void Render(RulePopupData uiData)
        {
            base.Render(uiData);
            TitleText.text = LocalizationManager.Instance.GetString(uiData.TitleId);
            RuleText.text = LocalizationManager.Instance.GetString(uiData.RuleId);
        }

        public override void DeInitialize()
        {
            base.DeInitialize();
            CloseButton.onClick.RemoveListener(OnCloseButtonClick);
        }

        private void OnCloseButtonClick()
        {
            Close();
        }
    }
}
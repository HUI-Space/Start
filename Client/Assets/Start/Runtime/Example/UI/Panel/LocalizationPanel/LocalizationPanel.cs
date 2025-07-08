using System.Collections.Generic;
using UnityEngine.UI;

namespace Start
{
    public class LocalizationPanel : UIPanel<LocalizationPanelData>
    {
        public Toggle ChineseToggle;
        public Toggle EnglishToggle;
        public Button CloseButton;
        public Text Title;
        
        private readonly List<string> _optionsList = new List<string>();
        public override void Initialize()
        {
            base.Initialize();
            ChineseToggle.isOn = LocalizationManager.Instance.LanguageId == (int)ELocalization.Chinese;
            EnglishToggle.isOn = LocalizationManager.Instance.LanguageId == (int)ELocalization.English;
            ChineseToggle.onValueChanged.AddListener(OnChineseToggleChanged);
            EnglishToggle.onValueChanged.AddListener(OnEnglishToggleChanged);
            CloseButton.onClick.AddListener(OnCloseButtonClick);
        }
        
        private void OnChineseToggleChanged(bool isOn)
        {
            if (isOn)
            {
                UIActions.ChangeLocalization(ELocalization.Chinese);
            }
        }
        
        private void OnEnglishToggleChanged(bool isOn)
        {
            if (isOn)
            {
                UIActions.ChangeLocalization(ELocalization.English);
            }
        }

        protected override void Render(LocalizationPanelData uiData)
        {
            base.Render(uiData);
            Title.text = LocalizationManager.Instance.GetString(202);
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
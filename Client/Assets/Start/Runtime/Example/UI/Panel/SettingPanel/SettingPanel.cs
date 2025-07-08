using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Start
{
    public class SettingPanel : UIPanel<SettingPanelData>
    {
        public Text TitleText;
        public Text DeleteKeyTitleText;
        public Text DeleteKeyInputFieldText;
        public Text DeleteKeyButtonText;
        public Text DeleteAllButtonText;
        public Text HasSettingTitleText;
        public Text HasSettingInputFieldText;
        public Text HasSettingResultText;
        public Text HasSettingButtonText;
        
        public Transform Parent;
        public SettingPanelItem SettingPanelItem;

        public InputField DeleteKeyInputField;
        public InputField HasSettingInputField;
        
        public Button HasSettingButton;
        public Button DeleteKeyButton;
        public Button DeleteAllButton;
        public Button CloseButton;
        public Button QuestionButton;

        private LimitList<SettingPanelItem,SettingPanelItem.SettingType> _limitList;
        private readonly List<SettingPanelItem.SettingType> _settingTypes = new List<SettingPanelItem.SettingType>();
        
        public override void Initialize()
        {
            base.Initialize();
            TitleText.text = LocalizationManager.Instance.GetString(1200);
            DeleteKeyTitleText.text = LocalizationManager.Instance.GetString(1206);
            DeleteKeyInputFieldText.text = LocalizationManager.Instance.GetString(1201);
            DeleteKeyButtonText.text = LocalizationManager.Instance.GetString(1207);
            DeleteAllButtonText.text = LocalizationManager.Instance.GetString(1208);
            HasSettingTitleText.text = LocalizationManager.Instance.GetString(1211);
            HasSettingButtonText.text = LocalizationManager.Instance.GetString(1212);
            HasSettingInputFieldText.text = LocalizationManager.Instance.GetString(1201);
            
            _limitList = new LimitList<SettingPanelItem,SettingPanelItem.SettingType>();
            _limitList.Initialize(SettingPanelItem, Parent);
            foreach (SettingPanelItem.SettingType s in Enum.GetValues(typeof(SettingPanelItem.SettingType)))
            {
                _settingTypes.Add(s);
            }
            
            HasSettingButton.onClick.AddListener(OnHasSettingButtonClick);
            DeleteKeyButton.onClick.AddListener(OnDeleteKeyButtonClick);
            DeleteAllButton.onClick.AddListener(OnDeleteAllButtonClick);
            CloseButton.onClick.AddListener(OnCloseButtonClick);
            QuestionButton.onClick.AddListener(OnQuestionButtonClick);
        }

        protected override void Render(SettingPanelData uiData)
        {
            base.Render(uiData);
            _limitList.SetData(_settingTypes);
            
        }

        public override void DeInitialize()
        {
            base.DeInitialize();
            _limitList.DeInitialize();
            _limitList = null;
            
            HasSettingButton.onClick.RemoveListener(OnHasSettingButtonClick);
            DeleteKeyButton.onClick.RemoveListener(OnDeleteKeyButtonClick);
            DeleteAllButton.onClick.RemoveListener(OnDeleteAllButtonClick);
            CloseButton.onClick.RemoveListener(OnCloseButtonClick);
            QuestionButton.onClick.RemoveListener(OnQuestionButtonClick);
        }

        private void OnHasSettingButtonClick()
        {
            if (!string.IsNullOrEmpty(HasSettingInputField.text))
            {
                HasSettingResultText.text = SettingManager.Instance.HasSetting(HasSettingInputField.text).ToString();
            }
            else
            {
                HasSettingResultText.text = string.Empty;
            }
        }
        
        private void OnDeleteKeyButtonClick()
        {
            if (!string.IsNullOrEmpty(DeleteKeyInputField.text))
            {
                SettingManager.Instance.DeleteKey(DeleteKeyInputField.text);
            }
        }
        
        private void OnDeleteAllButtonClick()
        {
            SettingManager.Instance.DeleteAll();
        }
        
        private void OnCloseButtonClick()
        {
            Close();
        }
        
        private void OnQuestionButtonClick()
        {
            UIActions.OpenRulePopup(1209, 1210);
        }
    }
}
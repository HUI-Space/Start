using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Start
{
    public class LogPanel : UIPanel<LogPanelData>
    {
        public Text TitleText;
        public Text LogText;
        
        public Transform Parent;
        public LogPanelItem LogPanelItem;
        private LimitList<LogPanelItem> _limitList;
        private readonly List<ELogType> _logTypes = new List<ELogType>();
        
        public Button CloseButton;
        public Button QuestionButton;
        
        public override void Initialize()
        {
            base.Initialize();
            TitleText.text = LocalizationManager.Instance.GetString(1100);
            foreach (ELogType logType in Enum.GetValues(typeof(ELogType)))
            {
                _logTypes.Add(logType);
            }
            _limitList = new LimitList<LogPanelItem>();
            _limitList.Initialize(LogPanelItem, Parent, null, (item,data) =>
            {
                item.SetData(_logTypes[data]);
            });
            
            CloseButton.onClick.AddListener(OnCloseButtonClick);
            QuestionButton.onClick.AddListener(OnQuestionButtonClick);
        }

        protected override void Render(LogPanelData uiData)
        {
            base.Render(uiData);
            _limitList.SetData(_logTypes.Count);
            LogText.text = uiData.LogBuilder.ToString();
        }
        
        public override void DeInitialize()
        {
            base.DeInitialize();
            _limitList.DeInitialize();
            _limitList = null;
            
            CloseButton.onClick.RemoveListener(OnCloseButtonClick);
            QuestionButton.onClick.RemoveListener(OnQuestionButtonClick);
        }
        
        private void OnCloseButtonClick()
        {
            Close();
        }
        
        private void OnQuestionButtonClick()
        {
            UIActions.OpenRulePopup(1103, 1104);
        }
    }
}
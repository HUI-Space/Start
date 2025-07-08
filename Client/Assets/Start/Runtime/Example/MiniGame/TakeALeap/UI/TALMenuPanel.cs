using UnityEngine;
using UnityEngine.UI;

namespace Start
{
    public class TALMenuPanel : UIPanel<TALMenuPanelData>
    {
        public CanvasGroup MenuCanvasGroup;
        public CanvasGroup SettingCanvasGroup;
        public CanvasGroup RankCanvasGroup;
        
        public Button StartButton;
        public Button SettingButton;
        public Button RankButton;
        public Button SettingCloseButton;
        public Button RankCloseButton;
        
        public Toggle NormalToggle;
        public Toggle HardToggle;
        
        public Toggle AudioToggle;
        
        public TALRankItem RankItem;
        public ScrollBase RankScroll;
        
        public override void Initialize()
        {
            base.Initialize();
            StartButton.onClick.AddListener(OnStartButtonClick);
            SettingButton.onClick.AddListener(OnSettingButtonClick);
            RankButton.onClick.AddListener(OnRankButtonClick);
            SettingCloseButton.onClick.AddListener(OnSettingCloseButtonClick);
            RankCloseButton.onClick.AddListener(OnRankCloseButtonClick);
            NormalToggle.onValueChanged.AddListener(OnNormalToggleClick);
            HardToggle.onValueChanged.AddListener(OnHardToggleClick);
            AudioToggle.onValueChanged.AddListener(OnAudioToggleClick);
            RankScroll.SetElementUI(RankItem, OnRenderCell);
        }

        private void OnRenderCell(UIElement arg1, int arg2)
        {
            if (arg1 is TALRankItem item)
            {
                //item.SetData(TALController.Instance.RankModule.GetRankData());
            }
        }

        protected override void Render(TALMenuPanelData uiData)
        {
            base.Render(uiData);
            MenuCanvasGroup.Switch(true);
            SettingCanvasGroup.Switch(false);
            RankCanvasGroup.Switch(false);
        }

        public override void DeInitialize()
        {
            base.DeInitialize();
            StartButton.onClick.RemoveListener(OnStartButtonClick);
            SettingButton.onClick.RemoveListener(OnSettingButtonClick);
            RankButton.onClick.RemoveListener(OnRankButtonClick);
            SettingCloseButton.onClick.RemoveListener(OnSettingCloseButtonClick);
            RankCloseButton.onClick.RemoveListener(OnRankCloseButtonClick);
            NormalToggle.onValueChanged.RemoveListener(OnNormalToggleClick);
            HardToggle.onValueChanged.RemoveListener(OnHardToggleClick);
            AudioToggle.onValueChanged.RemoveListener(OnAudioToggleClick);
        }
        
        private async void OnStartButtonClick()
        {
            await UIActions.OpenUI(nameof(TALGamePanel));
            TALController.Instance.LogicModule.StartGame();
        }
        
        private void OnSettingButtonClick()
        {
            MenuCanvasGroup.Switch(false);
            SettingCanvasGroup.Switch(true);
            RankCanvasGroup.Switch(false);
        }
        
        private void OnRankButtonClick()
        {
            MenuCanvasGroup.Switch(false);
            SettingCanvasGroup.Switch(false);
            RankCanvasGroup.Switch(true);
        }
        
        private void OnSettingCloseButtonClick()
        {
            MenuCanvasGroup.Switch(true);
            SettingCanvasGroup.Switch(false);
            RankCanvasGroup.Switch(false);
        }
        
        private void OnRankCloseButtonClick()
        {
            MenuCanvasGroup.Switch(true);
            SettingCanvasGroup.Switch(false);
            RankCanvasGroup.Switch(false);
        }
        
        private void OnNormalToggleClick(bool isOn)
        {
            if (isOn)
            {
                TALController.Instance.SettingModule.SetGameMode(true);
            }
        }
        
        private void OnHardToggleClick(bool isOn)
        {
            if (isOn)
            {
                TALController.Instance.SettingModule.SetGameMode(false);
            }
        }
        
        private void OnAudioToggleClick(bool isOn)
        {
            TALController.Instance.SettingModule.SetAudio(isOn);
        }
    }
    
    public class TALMenuPanelData : UIData
    {
        public override string UIName => nameof(TALMenuPanel);
    }
}
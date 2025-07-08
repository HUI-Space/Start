using UnityEngine.UI;

namespace Start
{
    public class AudioPanel : UIPanel<AudioPanelData>
    {
        public Text BackgroundText;
        public Text AudioText;
        
        public Slider BackgroundSlider;
        public Slider AuidoSlider;
        
        public Button CloseButton;
        
        public Button PlayBackgroundButton;
        public Button StopBackgroundButton;
        public Button PlayNextBackgroundButton;
        public Button RandomPlayBackgroundButton;
        
        public Button AudioButton;
        public Button FadeInAudioButton;
        public Button FadeOutAudioButton;
        public Button FadeInAndFadeOutAudioButton;

        public override void Initialize()
        {
            base.Initialize();
            BackgroundSlider.onValueChanged.AddListener(OnBackgroundSliderChanged);
            AuidoSlider.onValueChanged.AddListener(OnAuidoSliderChanged);
            
            CloseButton.onClick.AddListener(OnCloseButtonClick);
            
            PlayBackgroundButton.onClick.AddListener(OnPlayBackgroundButtonClick);
            StopBackgroundButton.onClick.AddListener(OnStopBackgroundButtonClick);
            PlayNextBackgroundButton.onClick.AddListener(OnPlayNextBackgroundButtonClick);
            RandomPlayBackgroundButton.onClick.AddListener(OnRandomPlayBackgroundButtonClick);
            
            AudioButton.onClick.AddListener(OnPlayAudioButtonClick);
            FadeInAudioButton.onClick.AddListener(OnFadeInAudioButtonClick);
            FadeOutAudioButton.onClick.AddListener(OnFadeOutAudioButtonClick);
            FadeInAndFadeOutAudioButton.onClick.AddListener(OnFadeInAndFadeOutAudioButtonClick);
        }

        

        protected override void Render(AudioPanelData uiData)
        {
            base.Render(uiData);
            float backGroundVolume = AudioController.Instance.GetVolume(EAudioType.BackGround);
            float uiVolume = AudioController.Instance.GetVolume(EAudioType.UI);
            BackgroundSlider.value = backGroundVolume;
            BackgroundText.text = backGroundVolume.ToString();
            AuidoSlider.value = uiVolume;
            AudioText.text = uiVolume.ToString();
            
        }

        public override void DeInitialize()
        {
            base.DeInitialize();
            BackgroundSlider.onValueChanged.RemoveListener(OnBackgroundSliderChanged);
            AuidoSlider.onValueChanged.RemoveListener(OnAuidoSliderChanged);
            CloseButton.onClick.RemoveListener(OnCloseButtonClick);
            PlayBackgroundButton.onClick.RemoveListener(OnPlayBackgroundButtonClick);
            StopBackgroundButton.onClick.RemoveListener(OnStopBackgroundButtonClick);
            PlayNextBackgroundButton.onClick.RemoveListener(OnPlayNextBackgroundButtonClick);
            RandomPlayBackgroundButton.onClick.RemoveListener(OnRandomPlayBackgroundButtonClick);
            AudioButton.onClick.RemoveListener(OnPlayAudioButtonClick);
            FadeInAudioButton.onClick.RemoveListener(OnFadeInAudioButtonClick);
            FadeOutAudioButton.onClick.RemoveListener(OnFadeOutAudioButtonClick);
            FadeInAndFadeOutAudioButton.onClick.RemoveListener(OnFadeInAndFadeOutAudioButtonClick);
        }

        private void OnBackgroundSliderChanged(float value)
        {
            if (AudioController.Instance.GetVolume(EAudioType.BackGround) == value)
            {
                return;
            }
            AudioController.Instance.ChangeVolume(EAudioType.BackGround, value);
            BackgroundText.text = value.ToString();
        }
        private void OnAuidoSliderChanged(float value)
        {
            if (AudioController.Instance.GetVolume(EAudioType.UI) == value)
            {
                return;
            }
            AudioController.Instance.ChangeVolume(EAudioType.UI, value);
            AudioText.text = value.ToString();
        }
        
        private void OnCloseButtonClick()
        {
            Close();
        }
        
        private void OnPlayBackgroundButtonClick()
        {
            AudioController.Instance.ResumeAudio(EAudioType.BackGround);
        }

        private void OnStopBackgroundButtonClick()
        {
            AudioController.Instance.PauseAudio(EAudioType.BackGround);
        }

        private void OnPlayNextBackgroundButtonClick()
        {
            AudioController.Instance.ChangeBackGround();
        }

        private void OnRandomPlayBackgroundButtonClick()
        {
            AudioController.Instance.RandomChangeBackGround();
        }

        private void OnPlayAudioButtonClick()
        {
            float volume = AudioController.Instance.GetVolume(EAudioType.UI);
            AudioController.Instance.PlayAudio(EAudioType.UI, AssetConfig.GetAssetPath(EAssetType.Audio, "UI/UI_1.Ogg"), 
                false, volume, 0,0);

        }

        private void OnFadeInAudioButtonClick()
        {
            float volume = AudioController.Instance.GetVolume(EAudioType.UI);
            AudioController.Instance.PlayAudio(EAudioType.UI, AssetConfig.GetAssetPath(EAssetType.Audio, "UI/UI_4.Ogg"),
                false, volume, 1f);

        }

        private void OnFadeOutAudioButtonClick()
        {
            float volume = AudioController.Instance.GetVolume(EAudioType.UI);
            AudioController.Instance.PlayAudio(EAudioType.UI, AssetConfig.GetAssetPath(EAssetType.Audio, "UI/UI_4.Ogg"),
                false, volume, 0,1f);
        }

        private void OnFadeInAndFadeOutAudioButtonClick()
        {
            float volume = AudioController.Instance.GetVolume(EAudioType.UI);
            AudioController.Instance.PlayAudio(EAudioType.UI, AssetConfig.GetAssetPath(EAssetType.Audio, "UI/UI_4.Ogg"),
                false, volume, 1f,1f);
        }
    }
}


using UnityEngine;
using UnityEngine.UI;

namespace Start
{
    [DisallowMultipleComponent]
    public class UIAudio : UIElement
    {
        [Range(0, 1)]
        public float Volume = 1f;
        public string UIAudioPath;
        public bool PlayAudioOnToggleOn = true;
        public bool PlayAudioOnToggleOff = true;
        public EUIInputType InputType { get; private set; }
        
        private Button _button;
        private Toggle _toggle;
        
        public override void Initialize()
        {
            _button = GetComponent<Button>();
            if (_button != null)
            {
                _button.onClick.AddListener(PlayAudio);
                InputType = EUIInputType.Button;
                return;
            }
            _toggle = GetComponent<Toggle>();
            if (_toggle != null)
            {
                _toggle.onValueChanged.AddListener(PlayAudio);
                InputType = EUIInputType.Toggle;
            }
        }

        private void PlayAudio()
        {
            AudioManager.Instance.PlayAudio((int)EAudioType.UI, UIAudioPath, false, Volume);
        }
        
        private void PlayAudio(bool toggleOn)
        {
            if (PlayAudioOnToggleOn && toggleOn)
            {
                AudioManager.Instance.PlayAudio((int)EAudioType.UI, UIAudioPath, false, Volume);
            }
            else if (PlayAudioOnToggleOff && !toggleOn)
            {
                AudioManager.Instance.PlayAudio((int)EAudioType.UI, UIAudioPath, false, Volume);
            }
        }
        
        public override void DeInitialize()
        {
            if (_button != null)
            {
                _button.onClick.RemoveListener(PlayAudio);
            }
            
            if (_toggle != null)
            {
                _toggle.onValueChanged.RemoveListener(PlayAudio);
            }
        }
    }
}
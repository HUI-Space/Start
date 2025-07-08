using UnityEngine;
using UnityEngine.UI;

namespace Start
{

    [RequireComponent(typeof(Text))]
    public class UILocalization : UIElement
    {
        [SerializeField]
        public int LocalizationId;
        
        private Text _text;
        
        public override void Initialize()
        {
            base.Initialize();
            _text = GetComponent<Text>();
            LocalizationManager.Instance.LocalizationChanged += OnLocalizationChange;
            _text.text = LocalizationManager.Instance.GetString(LocalizationId,_text.text);
        }

        private void OnLocalizationChange()
        {
            _text.text = LocalizationManager.Instance.GetString(LocalizationId,_text.text);
        }

        public override void DeInitialize()
        {
            base.DeInitialize();
            LocalizationManager.Instance.LocalizationChanged -= OnLocalizationChange;
        }
    }
}


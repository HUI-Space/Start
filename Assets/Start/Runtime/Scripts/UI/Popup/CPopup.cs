using Start.Runtime;
using UnityEngine.UI;

namespace Start.Script
{
    public class CPopup:UIBase<CPopupData>
    {
        public Button CloseButton;
        public Button GoButton;
        public Button CloseA;
        public override void Initialize()
        {
            base.Initialize();
            CloseButton.onClick.AddListener(OnCloseButtonClick);
            GoButton.onClick.AddListener(OnGoButtonClick);
            CloseA.onClick.AddListener(OnCloseAClick);
        }

        private void OnCloseAClick()
        {
            UIActions.APanel_Close();
        }

        private void OnGoButtonClick()
        {
            UIActions.GoBackAPanel();
        }

        private void OnCloseButtonClick()
        {
            OnClose();
        }

        public override void DeInitialize()
        {
            base.DeInitialize();
            CloseButton.onClick.RemoveListener(OnCloseButtonClick);
        }
    }

    public class CPopupData : UIData
    {
        public override string UIName => nameof(CPopup);
    }
}
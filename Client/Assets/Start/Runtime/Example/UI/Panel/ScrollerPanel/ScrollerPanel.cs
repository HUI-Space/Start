using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Start
{
    public class ScrollerPanel : UIPanel<ScrollerPanelData>
    {
        public CanvasGroup DefaultCanvasGroup;
        public CanvasGroup HorizontalCanvasGroup;
        public CanvasGroup VerticalCanvasGroup;
        public CanvasGroup GridCanvasGroup;
        
        public Button HorizontalButton;
        public Button VerticalButton;
        public Button GridButton;
        
        public Button CloseButton;
        
        public override void Initialize()
        {
            base.Initialize();
            HorizontalButton.onClick.AddListener(OnHorizontalButtonClick);
            VerticalButton.onClick.AddListener(OnVerticalButtonButtonClick);
            GridButton.onClick.AddListener(OnHorizontalGridButtonClick);
            CloseButton.onClick.AddListener(OnCloseButtonClick);
        }

        protected override void Render(ScrollerPanelData uiData)
        {
            base.Render(uiData);
            DefaultCanvasGroup.Switch(uiData.ScrollerType == EScrollerType.None);
            HorizontalCanvasGroup.Switch(uiData.ScrollerType == EScrollerType.Horizontal);
            VerticalCanvasGroup.Switch(uiData.ScrollerType == EScrollerType.Vertical);
            GridCanvasGroup.Switch(uiData.ScrollerType == EScrollerType.Grid);
        }

        public override void DeInitialize()
        {
            base.DeInitialize();
            CloseButton.onClick.RemoveListener(OnCloseButtonClick);
            HorizontalButton.onClick.RemoveListener(OnHorizontalButtonClick);
            VerticalButton.onClick.RemoveListener(OnVerticalButtonButtonClick);
            GridButton.onClick.RemoveListener(OnHorizontalGridButtonClick);
        }

        private void OnCloseButtonClick()
        {
            if (UIWindow.Instance.GetUIData<ScrollerPanelData>(out ScrollerPanelData data))
            {
                if (data.ScrollerType == EScrollerType.None)
                {
                    Close();
                }
                else
                {
                    UIActions.SwitchScrollerType(EScrollerType.None);
                }
            }
        }

        private void OnHorizontalButtonClick()
        {
            UIActions.SwitchScrollerType(EScrollerType.Horizontal);
        }
        private void OnVerticalButtonButtonClick()
        {
            UIActions.SwitchScrollerType(EScrollerType.Vertical);
        }
        private void OnHorizontalGridButtonClick()
        {
            UIActions.SwitchScrollerType(EScrollerType.Grid);
        }
    }
}


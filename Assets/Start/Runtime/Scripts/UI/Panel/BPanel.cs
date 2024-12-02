using System;
using Start.Framework;
using Start.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace Start.Script
{
    public class BPanel:UIBase<BPanelData>
    {
        public Button CloseButton;
        public Button OpenButton;
        
        public override void Initialize()
        {
            base.Initialize();
            CloseButton.onClick.AddListener(OnCloseButtonClick);
            OpenButton.onClick.AddListener(OnOpenButtonClick);
        }

        private void OnOpenButtonClick()
        {
            UIActions.OpenCPopup();
        }

        private void OnCloseButtonClick()
        {
            OnClose();
        }

        public override void DeInitialize()
        {
            base.DeInitialize();
            CloseButton.onClick.RemoveListener(OnCloseButtonClick);
            OpenButton.onClick.RemoveListener(OnOpenButtonClick);
        }
    }

    public class BPanelData:UIData
    {
        public override string UIName =>nameof(BPanel);

        public override void Initialize()
        {
            base.Initialize();
            Register("Open", Open);
        }


        private void Open(UIAction uiAction)
        {
            Debug.Log(uiAction.ActionType);
            IsDirty = true;
        }

        public override void DeInitialize()
        {
            base.DeInitialize();
            UnRegister("Open", Open);
        }
    }
    
}
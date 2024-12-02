using System;
using Start.Framework;
using Start.Runtime;
using UnityEngine;
using UnityEngine.UI;

namespace Start.Script
{
    public class APanel:UIBase<APanelData>
    {
        public Button Button;
        public override void Initialize()
        {
            base.Initialize();
            Button.onClick.AddListener(OnButtonClick);
        }

        private void OnButtonClick()
        {
            UIActions.OpenBPanel();
        }


        protected override void Render(APanelData uiData)
        {
            base.Render(uiData);
        }

        public override void DeInitialize()
        {
            base.DeInitialize();
            Button.onClick.RemoveListener(OnButtonClick);
        }
    }

    public class APanelData:UIData
    {
        public override string UIName =>nameof(APanel);
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
using System.Collections.Generic;
using UnityEngine.UI;

namespace Start
{
    public class BattlePanel : UIPanel<BattlePanelData> , IInputHelper
    {
        public Button CloseButton;
        public UIJoystick UIJoystick;
        public List<BattleButton> BattleButtons;

        private bool _isGiveUp = false;
        public override void Initialize()
        {
            base.Initialize();
            CloseButton.onClick.AddListener(OnCloseButtonClick);
            InputController.Instance.SetHelper(this);
        }

        public override void DeInitialize()
        {
            base.DeInitialize();
            InputController.Instance.SetHelper(null);
            CloseButton.onClick.RemoveListener(OnCloseButtonClick);
        }

        public FrameInput GetFrameInput()
        {
            byte realButtonState = 0;
            for (int i = 0; i < BattleButtons.Count; ++i)
            {
                if (BattleButtons[i].IsDown)
                {
                    realButtonState |= (byte)(1 << i);
                }
            }
            FrameInput frameInput = new FrameInput();
            frameInput.Yaw = (byte)UIJoystick.GetRegion(UIJoystick.Direction, InputSystem.Divisions);
            frameInput.Button = realButtonState;
            frameInput.GiveUp = (byte)(_isGiveUp ? 1 : 0);
            return frameInput;
        }
        
        private void OnCloseButtonClick()
        {
            _isGiveUp = true;
        }
    }
}
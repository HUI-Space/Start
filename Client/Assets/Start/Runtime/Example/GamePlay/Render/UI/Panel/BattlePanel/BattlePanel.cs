using System.Collections.Generic;

namespace Start
{
    public class BattlePanel : UIPanel<BattlePanelData> , IInputHelper
    {
        public UIJoystick UIJoystick;
        public List<BattleButton> BattleButtons;

        public override void Initialize()
        {
            base.Initialize();
            InputController.Instance.SetHelper(this);
        }

        public FrameInput GetInput()
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
            frameInput.Yaw = (byte)UIJoystick.GetRegion(UIJoystick.Direction, 4);
            frameInput.Button = realButtonState;
            return frameInput;
        }
    }
}
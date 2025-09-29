using System;

namespace Start
{
    public class InputController : SingletonBase<InputController>
    {
        private IInputHelper _inputHelper;

        public void SetHelper(IInputHelper inputHelper)
        {
            _inputHelper = inputHelper;
        }
        
        public FrameInput GetInput()
        {
            return _inputHelper.GetInput();
        }
    }
}
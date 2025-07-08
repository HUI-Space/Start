namespace Start
{
    public class ScrollerPanelData : UIData
    {
        public override string UIName => nameof(ScrollerPanel);

        public EScrollerType ScrollerType { get; private set; }

        public override void Initialize()
        {
            base.Initialize();
            Register(UIActionTypes.SwitchScrollerType,OnSwitchScrollerType);
        }

        private void OnSwitchScrollerType(UIAction uiAction)
        {
            ScrollerType = uiAction.GetData1<EScrollerType>();
            IsDirty = true;
        }
        
    }
}
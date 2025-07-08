namespace Start
{
    public class ConfigPanelData : UIData
    {
        public override string UIName => nameof(ConfigPanel);

        public override void Initialize()
        {
            base.Initialize();
            Register(UIActionTypes.UpdateConfigPanel, OnUpdateConfigPanel);
        }
        

        private void OnUpdateConfigPanel(UIAction uiAction)
        {
            IsDirty = true;
        }
    }
}
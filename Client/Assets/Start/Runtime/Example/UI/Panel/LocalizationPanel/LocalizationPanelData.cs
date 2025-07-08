namespace Start
{
    public class LocalizationPanelData : UIData
    {
        public override string UIName  => nameof(LocalizationPanel);


        public override void Initialize()
        {
            base.Initialize();
            Register(UIActionTypes.ChangeLocalization, OnChangeLocalization);
        }

        private void OnChangeLocalization(UIAction uiAction)
        {
            ELocalization localization = uiAction.GetData1<ELocalization>();
            LocalizationManager.Instance.ChangeLocalization((int)localization);
            IsDirty = true;
        }
    }
}
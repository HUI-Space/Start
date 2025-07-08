namespace Start
{
    public static partial class UIActionTypes
    {
        public static string UpdateConfigPanel = nameof(UpdateConfigPanel);
    }
    public static partial class UIActions
    {
        public static void UpdateConfigPanel()
        {
            UIAction.Create(nameof(ConfigPanel), UIActionTypes.UpdateConfigPanel).Dispatch();
        }
    }
}
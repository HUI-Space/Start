namespace Start
{
    public static partial class UIActionTypes
    {
        public static string ChangeLocalization = nameof(ChangeLocalization);
    }
    public static partial class UIActions
    {
        public static void ChangeLocalization(ELocalization localization)
        {
            UIAction.Create(nameof(LocalizationPanel), UIActionTypes.ChangeLocalization)
                .SetData<ELocalization>(localization)
                .Dispatch();
        } 
    }
}
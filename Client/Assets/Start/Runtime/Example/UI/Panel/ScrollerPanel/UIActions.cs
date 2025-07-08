namespace Start
{
    public static partial class UIActionTypes
    {
        public static string SwitchScrollerType = nameof(SwitchScrollerType);
    }
    public static partial class UIActions
    {
        public static void SwitchScrollerType(EScrollerType scrollerType)
        {
            UIAction.Create(nameof(ScrollerPanel), UIActionTypes.SwitchScrollerType)
                .SetData(scrollerType)
                .Dispatch();
        }
    }
}
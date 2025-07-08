namespace Start
{
    public static partial class UIActionTypes
    {
        public static string LogPanel_Log = nameof(LogPanel_Log);
    }
    public static partial class UIActions
    {
        public static void LogPanel_Log(string log)
        {
            UIAction.Create(nameof(LogPanel), UIActionTypes.LogPanel_Log)
                .SetData<string>(log)
                .Dispatch();
        } 
    }
}
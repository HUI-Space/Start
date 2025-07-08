namespace Start
{
    public static partial class UIActionTypes
    {
        public static string FsmPanel_ChangeAsyncDropdownValue = nameof(FsmPanel_ChangeAsyncDropdownValue);
        public static string FsmPanel_ChangeSyncDropdownValue = nameof(FsmPanel_ChangeSyncDropdownValue);
        public static string FsmPanel_AsyncFsmLog = nameof(FsmPanel_AsyncFsmLog);
        public static string FsmPanel_SyncFsmLog = nameof(FsmPanel_SyncFsmLog);
    }
    public static partial class UIActions
    {
        public static void FsmPanel_ChangeAsyncDropdownValue(int value)
        {
            UIAction.Create(nameof(FsmPanel), UIActionTypes.FsmPanel_ChangeAsyncDropdownValue).SetData(value)
                .Dispatch();
        }
        
        public static void FsmPanel_ChangeSyncDropdownValue(int value)
        {
            UIAction.Create(nameof(FsmPanel), UIActionTypes.FsmPanel_ChangeSyncDropdownValue).SetData(value)
                .Dispatch();
        }
        
        /// <summary>
        /// AsyncFsmLog
        /// </summary>
        /// <param name="log"></param>
        public static void FsmPanel_AsyncFsmLog(string log)
        {
            UIAction.Create(nameof(FsmPanel), UIActionTypes.FsmPanel_AsyncFsmLog).SetData(log).Dispatch();
        }
        
        /// <summary>
        /// SyncFsmLog
        /// </summary>
        /// <param name="log"></param>
        public static void FsmPanel_SyncFsmLog(string log)
        {
            UIAction.Create(nameof(FsmPanel), UIActionTypes.FsmPanel_SyncFsmLog).SetData(log).Dispatch();
        }
    }
}
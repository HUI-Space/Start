using System.Text;

namespace Start
{
    public class FsmPanelData : UIData
    {
        public override string UIName => nameof(FsmPanel);

        public int AsyncValue { get; private set; }
        
        public int SyncValue { get; private set; }
        
        public StringBuilder AsyncFsmStringBuilder { get; private set; }
        public StringBuilder SyncFsmStringBuilder { get; private set; }

        public override void Initialize()
        {
            AsyncFsmStringBuilder = new StringBuilder();
            SyncFsmStringBuilder = new StringBuilder();
            base.Initialize();
            Register(UIActionTypes.FsmPanel_ChangeAsyncDropdownValue, OnChangeAsyncDropdownValue);
            Register(UIActionTypes.FsmPanel_ChangeSyncDropdownValue, OnChangeSyncDropdownValue);
            Register(UIActionTypes.FsmPanel_AsyncFsmLog, OnAsyncFsmLog);
            Register(UIActionTypes.FsmPanel_SyncFsmLog, OnSyncFsmLog);
        }

        public override void DeInitialize()
        {
            AsyncFsmStringBuilder.Clear();
            SyncFsmStringBuilder.Clear();
            base.DeInitialize();
        }

        private void OnChangeAsyncDropdownValue(UIAction uiAction)
        {
            AsyncValue = uiAction.GetData1<int>();
            IsDirty = true;
        }
        
        private void OnChangeSyncDropdownValue(UIAction uiAction)
        {
            SyncValue = uiAction.GetData1<int>();
            IsDirty = true;
        }

        private void OnAsyncFsmLog(UIAction uiAction)
        {
            AsyncFsmStringBuilder.AppendLine(uiAction.GetData1<string>());
            IsDirty = true;
        }
        
        private void OnSyncFsmLog(UIAction uiAction)
        {
            SyncFsmStringBuilder.AppendLine(uiAction.GetData1<string>());
            IsDirty = true;
        }
    }
}
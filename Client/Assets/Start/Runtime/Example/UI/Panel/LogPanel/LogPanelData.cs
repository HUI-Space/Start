using System.Text;

namespace Start
{
    public class LogPanelData : UIData
    {
        public override string UIName => nameof(LogPanel);

        public StringBuilder LogBuilder { get; private set; }

        public override void Initialize()
        {
            base.Initialize();
            LogBuilder = new StringBuilder();
            Register(UIActionTypes.LogPanel_Log, OnLog);
        }
        
        private void OnLog(UIAction uiAction)
        {
            LogBuilder.AppendLine(uiAction.GetData1<string>());
            IsDirty = true;
        }
        
    }
}
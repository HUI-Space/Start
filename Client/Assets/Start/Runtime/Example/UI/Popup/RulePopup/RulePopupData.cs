namespace Start
{
    public class RulePopupData : UIData
    {
        public override string UIName => nameof(RulePopup);
        
        public long TitleId { get; private set; }
        public long RuleId { get; private set; }


        protected override void Open(UIAction uiAction)
        {
            TitleId = uiAction.GetData1<long>();
            RuleId = uiAction.GetData2<long>();
            base.Open(uiAction);
        }
    }
}
namespace Start
{
    public static partial class UIActions
    {
        public static void OpenRulePopup(long TitleId ,long ruleId)
        {
            UIAction.Create(nameof(RulePopup), UIActionTypes.Open)
                .SetData(TitleId,ruleId)
                .Dispatch();
        }
    }
}
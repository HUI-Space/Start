namespace Start
{
    public class ScenePanelData :UIData
    {
        public override string UIName => nameof(ScenePanel);

        public int State { get; private set; }

        protected override void Open(UIAction uiAction)
        {
            if (uiAction.Data != null)
            {
                State = uiAction.GetData1<int>();
            }
            base.Open(uiAction);
        }
    }
}
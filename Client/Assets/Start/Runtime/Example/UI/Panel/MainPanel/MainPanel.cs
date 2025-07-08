namespace Start
{
    public class MainPanel : UIPanel<MainPanelData>
    {
        public ScrollBase Scroller;
        public MainItem mainItem;

        public override void Initialize()
        {
            base.Initialize();
            Scroller.SetElementUI(mainItem, RenderCell);
        }

        protected override void Render(MainPanelData uiData)
        {
            base.Render(uiData);
            Scroller.SetCount(ConfigManager.Instance.GetConfig<ExampleConfig>().DataList.Count);
        }

        private void RenderCell(UIElement uiElement, int cellIndex)
        {
            if (uiElement is MainItem item)
            {
                item.SetData(ConfigManager.Instance.GetConfig<ExampleConfig>().DataList[cellIndex]);
            }
        }
    }
    
    public class MainPanelData : UIData
    {
        public override string UIName => nameof(MainPanel);
    }
}


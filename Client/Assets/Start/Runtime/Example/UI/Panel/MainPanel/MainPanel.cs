using UnityEngine.UI;

namespace Start
{
    public class MainPanel : UIPanel<MainPanelData>
    {
        public ScrollBase Scroller;
        public MainItem mainItem;
        public Button Button;

        public override void Initialize()
        {
            base.Initialize();
            Scroller.SetElementUI(mainItem, RenderCell);
            Button.onClick.AddListener(StartBattle);
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

        private void StartBattle()
        {
            BattleData battleData = BattleData.Create();
            battleData.BattleType = EBattleType.Local;
            battleData.FrameInterval = FrameConst.FrameInterval;
            battleData.Player.Add(1);
            BattleManager.Instance.StartBattle(battleData);
        }
    }
    
    public class MainPanelData : UIData
    {
        public override string UIName => nameof(MainPanel);
    }
}


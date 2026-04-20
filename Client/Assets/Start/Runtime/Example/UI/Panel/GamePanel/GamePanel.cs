using System.Net;
using UnityEngine.UI;

namespace Start
{
    public class GamePanel : UIPanel<GamePanelData>
    {
        public Button FrameSyncButton;
        public Button ReplayButton;
        public Button MatchButton;
        public Button CloseButton;
        public Button QuestionButton;
        public override void Initialize()
        {
            base.Initialize();
            FrameSyncButton.onClick.AddListener(StartBattle);
            ReplayButton.onClick.AddListener(OnReplayButtonClick);
            MatchButton.onClick.AddListener(OnMatchButtonClick);
            CloseButton.onClick.AddListener(OnCloseButtonClick);
            QuestionButton.onClick.AddListener(OnQuestionButtonClick);
        }
        
        public override void DeInitialize()
        {
            base.DeInitialize();
            FrameSyncButton.onClick.RemoveListener(StartBattle);
            ReplayButton.onClick.RemoveListener(OnReplayButtonClick);
            MatchButton.onClick.RemoveListener(OnMatchButtonClick);
        }
        
        private void StartBattle()
        {
            BattleManager.Instance.BattleType = EBattleType.Local;
            BattleManager.Instance.StartState<PrepareState>();
        }

        private void OnReplayButtonClick()
        {
            BattleManager.Instance.BattleType = EBattleType.Observer;
            BattleManager.Instance.StartState<PrepareState>();
        }
        
        private void OnMatchButtonClick()
        {
            //SocketManager.Instance.KcpConnect(0,new IPEndPoint(IPAddress.Parse("192.168.1.102"), 8888), new KcpChannelHelper(), new KcpHelper());
        }
        
        private void OnCloseButtonClick()
        {
            Close();
        }
        
        private void OnQuestionButtonClick()
        {
            //UIActions.OpenRulePopup(911, 912);
        }
        
    }
}
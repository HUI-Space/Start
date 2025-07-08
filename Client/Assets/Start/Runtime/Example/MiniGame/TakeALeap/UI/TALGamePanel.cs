using System.Threading.Tasks;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine;

namespace Start
{
    public static partial class UIActionTypes
    {
        public static string TALGamePanel_GetScore = nameof(TALGamePanel_GetScore);
        public static string TALGamePanel_ShowResurgenceButton = nameof(TALGamePanel_ShowResurgenceButton);
        public static string TALGamePanel_OnRestart = nameof(TALGamePanel_OnRestart);
    }
    public static partial class UIActions
    {
        public static Task TALGamePanel_GetScore()
        {
            return UIAction.Create(nameof(TALGamePanel), UIActionTypes.TALGamePanel_GetScore).Dispatch();
        }
        
        public static Task TALGamePanel_ShowResurgenceButton()
        {
            return UIAction.Create(nameof(TALGamePanel), UIActionTypes.TALGamePanel_ShowResurgenceButton).Dispatch();
        }
        
        public static Task TALGamePanel_OnRestart()
        {
            return UIAction.Create(nameof(TALGamePanel), UIActionTypes.TALGamePanel_OnRestart).Dispatch();
        }
    }
    
    public class TALGamePanel : UIPanel<TALGamePanelData>
    {
        public Text ScoreText;
        public Text GetScore;
        
        public Button ResurgenceButton;
        public Button RestartButton;
        public Button ExitButton;
        

        public override void Initialize()
        {
            base.Initialize();
            RestartButton.onClick.AddListener(OnRestartButtonClick);
            ExitButton.onClick.AddListener(OnExitButtonClick);
            ResurgenceButton.onClick.AddListener(OnResurgenceButtonClick);
        }

        protected override void Render(TALGamePanelData uiData)
        {
            base.Render(uiData);
            ScoreText.text = $"Score : {TALController.Instance.LogicModule.CurrentScore}";
            if (uiData.IsGetScore)
            {
                GetScore.text = "+1";
                GetScore.transform.DOScale(Vector3.one,0.5f).OnComplete(() =>
                {
                    // 放大完成后缩小
                    GetScore.transform.DOScale(Vector3.zero, 0.5f);
                });
            }
            ResurgenceButton.gameObject.SetActive(uiData.IsShowResurgenceButton);
        }

        public override void DeInitialize()
        {
            base.DeInitialize();
            RestartButton.onClick.RemoveListener(OnRestartButtonClick);
            ExitButton.onClick.RemoveListener(OnExitButtonClick);
            ResurgenceButton.onClick.RemoveListener(OnResurgenceButtonClick);
        }
        
        public void OnRestartButtonClick()
        {
            UIActions.TALGamePanel_OnRestart();
        }
        
        public void OnExitButtonClick()
        {
            TALController.Instance.LogicModule.GameOver();
            Close();
        }
        
        public void OnResurgenceButtonClick()
        {
            ResurgenceButton.gameObject.SetActive(false);
            TALController.Instance.LogicModule.ConfirmedResurgence();
        }

    }
    
    public class TALGamePanelData : UIData
    {
        public override string UIName => nameof(TALGamePanel);

        public bool IsGetScore { get; private set; }
        
        public bool IsShowResurgenceButton { get; private set; }

        public override void Initialize()
        {
            base.Initialize();
            Register(UIActionTypes.TALGamePanel_GetScore, OnGetScore);
            Register(UIActionTypes.TALGamePanel_ShowResurgenceButton, OnShowResurgenceButton);
            Register(UIActionTypes.TALGamePanel_OnRestart, OnRestart);
        }

        private void OnRestart(UIAction uiAction)
        {
            TALController.Instance.LogicModule.RestartGame();
            IsDirty = true;
        }

        private void OnGetScore(UIAction uiAction)
        {
            IsGetScore = true;
            IsDirty = true;
            IsGetScore = false;
        }
        private void OnShowResurgenceButton(UIAction uiAction)
        {
            IsShowResurgenceButton = true;
            IsDirty = true;
            IsShowResurgenceButton = false;
        }
    }
}
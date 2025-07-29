using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Start
{
    public class ScenePanel : UIPanel<ScenePanelData>
    {
        public CanvasGroup BG_1;
        public CanvasGroup BG_2;

        public Text TitleText;
        public Text RunButtonText;
        public Text ReturnButtonText;
        
        public Button RunButton;
        public Button ReturnButton;

        public Button CloseButton;
        public Button QuestionButton;

        public override void Initialize()
        {
            base.Initialize();
            
            RunButton.onClick.AddListener(OnRunButtonClick);
            ReturnButton.onClick.AddListener(OnReturnButtonClick);
            CloseButton.onClick.AddListener(OnCloseButtonClick);
            QuestionButton.onClick.AddListener(OnQuestionButtonClick);
        }

        protected override void Render(ScenePanelData uiData)
        {
            base.Render(uiData);
            TitleText.text =  LocalizationManager.Instance.GetString(1300);
            RunButtonText.text = LocalizationManager.Instance.GetString(1303);
            ReturnButtonText.text = LocalizationManager.Instance.GetString(1304);
            BG_1.Switch(uiData.State == 0);
            BG_2.Switch(uiData.State == 1);
        }

        public override void DeInitialize()
        {
            base.DeInitialize();
            CloseButton.onClick.RemoveListener(OnCloseButtonClick);
            QuestionButton.onClick.RemoveListener(OnQuestionButtonClick);
        }

        
        private async void OnRunButtonClick()
        {
            await UIActions.ShowLoading("");
            await UIActions.UpdateLoadingProgress(0);
            await UIActions.CloseAllUI();
            await Task.Delay(1000);
            await UIActions.UpdateLoadingProgress(0.33f);
            await UIActions.ScenePanel_Open(1);
            await Task.Delay(1000);
            await UIActions.UpdateLoadingProgress(0.66f);
            SceneManager.Instance.UnloadScene("Assets/Asset/Scene/MainScene.unity");
            AsyncOperationHandle<Scene> asyncOperationHandle = SceneManager.Instance.LoadSceneAsync<Scene>("Assets/Asset/Scene/ExampleScene.unity",true);
            await asyncOperationHandle.Task;
            await UIActions.UpdateLoadingProgress(1f);
            await Task.Delay(1000);
            await UIActions.HideLoading();
        }
        

        private async void OnReturnButtonClick()
        {
            await UIActions.ShowLoading("");
            await UIActions.CloseAllUI();
            await UIActions.OpenUI(nameof(MainPanel));
            await UIActions.OpenUI(nameof(ScenePanel));
            SceneManager.Instance.UnloadScene("Assets/Asset/Scene/ExampleScene.unity");
            AsyncOperationHandle<Scene> asyncOperationHandle = SceneManager.Instance.LoadSceneAsync<Scene>("Assets/Asset/Scene/MainScene.unity",false);
            await asyncOperationHandle.Task;
            await UIActions.UpdateLoadingProgress(1f);
            await UIActions.HideLoading();
        }
        
        private void OnCloseButtonClick()
        {
            Close();
        }
        
        private void OnQuestionButtonClick()
        {
            UIActions.OpenRulePopup(1301, 1302);
        }
    }
}
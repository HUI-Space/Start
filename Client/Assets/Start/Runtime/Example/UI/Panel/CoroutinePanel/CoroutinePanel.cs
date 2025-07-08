using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Start
{
    public class CoroutinePanel : UIPanel<CoroutinePanelData>
    {
        public Text TitleText;
        public Text SimpleDisplayContentText;
        public Text ComplexDisplayContentText;

        public Text RunAfterSecondsButtonText;
        public Text RunAtEndOfFrameButtonText;
        public Text RunAtFixedUpdateButtonText;
        public Text RunAtNextFrameButtonText;
        public Text RunButtonText;
        public Text PauseButtonText;
        public Text ResumeButtonText;
        public Text StopButtonText;

        public Button CloseButton;
        public Button RuleButton;

        public Button RunAfterSecondsButton;
        public Button RunAtEndOfFrameButton;
        public Button RunAtFixedUpdateButton;
        public Button RunAtNextFrameButton;

        public Button RunButton;
        public Button PauseButton;
        public Button ResumeButton;
        public Button StopButton;

        private CoroutineHandle _coroutineHandle;
        private readonly WaitForSeconds _waitForSeconds = new WaitForSeconds(1f);
        
        public override void Initialize()
        {
            base.Initialize();
            CloseButton.onClick.AddListener(OnCloseButtonClick);
            RuleButton.onClick.AddListener(OnRuleButtonClick);
            RunAfterSecondsButton.onClick.AddListener(OnRunAfterSecondsButtonClick);
            RunAtEndOfFrameButton.onClick.AddListener(OnRunAtEndOfFrameButtonClick);
            RunAtFixedUpdateButton.onClick.AddListener(OnRunAtFixedUpdateButtonClick);
            RunAtNextFrameButton.onClick.AddListener(OnRunAtNextFrameButtonClick);
            RunButton.onClick.AddListener(OnRunButtonClick);
            PauseButton.onClick.AddListener(OnPauseButtonClick);
            ResumeButton.onClick.AddListener(OnResumeButtonClick);
            StopButton.onClick.AddListener(OnStopButtonClick);
        }


        protected override void Render(CoroutinePanelData uiData)
        {
            base.Render(uiData);
            TitleText.text = LocalizationManager.Instance.GetString(500);
            SimpleDisplayContentText.text = LocalizationManager.Instance.GetString(503);
            ComplexDisplayContentText.text = LocalizationManager.Instance.GetString(503);
            RunAfterSecondsButtonText.text = LocalizationManager.Instance.GetString(504);
            RunAtEndOfFrameButtonText.text = LocalizationManager.Instance.GetString(505);
            RunAtFixedUpdateButtonText.text = LocalizationManager.Instance.GetString(506);
            RunAtNextFrameButtonText.text = LocalizationManager.Instance.GetString(507);
            RunButtonText.text = LocalizationManager.Instance.GetString(508);
            PauseButtonText.text = LocalizationManager.Instance.GetString(509);
            ResumeButtonText.text = LocalizationManager.Instance.GetString(510);
            StopButtonText.text = LocalizationManager.Instance.GetString(511);
        }
        
        public override void DeInitialize()
        {
            base.DeInitialize();
            CloseButton.onClick.RemoveListener(OnCloseButtonClick);
            RuleButton.onClick.RemoveListener(OnRuleButtonClick);
            RunAfterSecondsButton.onClick.RemoveListener(OnRunAfterSecondsButtonClick);
            RunAtEndOfFrameButton.onClick.RemoveListener(OnRunAtEndOfFrameButtonClick);
            RunAtFixedUpdateButton.onClick.RemoveListener(OnRunAtFixedUpdateButtonClick);
            RunAtNextFrameButton.onClick.RemoveListener(OnRunAtNextFrameButtonClick);
            RunButton.onClick.RemoveListener(OnRunButtonClick);
            PauseButton.onClick.RemoveListener(OnPauseButtonClick);
            ResumeButton.onClick.RemoveListener(OnResumeButtonClick);
            StopButton.onClick.RemoveListener(OnStopButtonClick);
        }

        private void OnCloseButtonClick()
        {
            Close();
        }

        private void OnRuleButtonClick()
        {
            UIActions.OpenRulePopup(501, 502);
        }

        private void OnRunAfterSecondsButtonClick()
        {
            SimpleDisplayContentText.text = LocalizationManager.Instance.GetString(514);
            CoroutineController.RunAfterSeconds(5,
                () => { SimpleDisplayContentText.text = LocalizationManager.Instance.GetString(515); });
        }

        private void OnRunAtEndOfFrameButtonClick()
        {
            CoroutineController.RunAtEndOfFrame(
                () => { SimpleDisplayContentText.text = LocalizationManager.Instance.GetString(516); });
        }

        private void OnRunAtFixedUpdateButtonClick()
        {
            CoroutineController.RunAtFixedUpdate(
                () => { SimpleDisplayContentText.text = LocalizationManager.Instance.GetString(517); });
        }

        private void OnRunAtNextFrameButtonClick()
        {
            CoroutineController.RunAtNextFrame(
                () => { SimpleDisplayContentText.text = LocalizationManager.Instance.GetString(518); });
        }

        private void OnRunButtonClick()
        {
            _coroutineHandle = CoroutineController.Run(OnRun(), true, OnStop);
        }

        private void OnPauseButtonClick()
        {
            _coroutineHandle.Pause();
        }

        private void OnResumeButtonClick()
        {
            _coroutineHandle.Resume();
        }

        private void OnStopButtonClick()
        {
            _coroutineHandle.Stop();
        }

        private IEnumerator OnRun()
        {
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < 50; i++)
            {
                yield return _waitForSeconds;
                stringBuilder.Append($"{i} ");
                ComplexDisplayContentText.text = stringBuilder.ToString();
            }
        }

        private void OnStop(bool isManualStopped)
        {
            ComplexDisplayContentText.text =
                $"{(isManualStopped ? LocalizationManager.Instance.GetString(513) : LocalizationManager.Instance.GetString(512))}";
        }
    }
}
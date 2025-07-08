using System;
using System.Collections.Generic;
using UnityEngine.UI;

namespace Start
{
    public class FsmPanel : UIPanel<FsmPanelData>
    {
        public Text TitleText;
        
        public Text AsyncFsmTypeTitleText;
        public Text AsyncFsmTypeValueText;
        public Text AsyncFsmStateTitleText;
        public Text AsyncFsmStateValueText;
        public Text AsyncFsmDropdownTitleText;
        public Text AsyncFsmStartButtonText;
        public Text AsyncFsmChangeStateButtonText;
        public Text AsyncFsmLogText;
        
        public Text SyncFsmTypeTitleText;
        public Text SyncFsmTypeValueText;
        public Text SyncFsmStateTitleText;
        public Text SyncFsmStateValueText;
        public Text SyncFsmDropdownTitleText;
        public Text SyncFsmStartButtonText;
        public Text SyncFsmChangeStateButtonText;
        public Text SyncFsmLogText;
        
        public Dropdown AsyncFsmDropdown;
        public Dropdown SyncFsmDropdown;

        public Button AsyncFsmStartButton;
        public Button AsyncFsmChangeStateButton;
        public Button SyncFsmStartButton;
        public Button SyncFsmChangeStateButton;
        
        public Button CloseButton;
        public Button QuestionButton;
        
        private ExampleAsyncFsm _exampleAsyncFsm;
        private ExampleSyncFsm _exampleSyncFsm;
        
        private readonly List<Dropdown.OptionData> _asyncOptionData = new List<Dropdown.OptionData>();
        private readonly List<Dropdown.OptionData> _syncOptionData = new List<Dropdown.OptionData>();
        
        public override async void Initialize()
        {
            base.Initialize();
            TitleText.text = LocalizationManager.Instance.GetString(900);
            AsyncFsmTypeTitleText.text = LocalizationManager.Instance.GetString(901);
            AsyncFsmStateTitleText.text = LocalizationManager.Instance.GetString(902);
            AsyncFsmDropdownTitleText.text = LocalizationManager.Instance.GetString(903);
            AsyncFsmStartButtonText.text = LocalizationManager.Instance.GetString(904);
            AsyncFsmChangeStateButtonText.text = LocalizationManager.Instance.GetString(905);
            
            SyncFsmTypeTitleText.text = LocalizationManager.Instance.GetString(906);
            SyncFsmStateTitleText.text = LocalizationManager.Instance.GetString(907);
            SyncFsmDropdownTitleText.text = LocalizationManager.Instance.GetString(908);
            SyncFsmStartButtonText.text = LocalizationManager.Instance.GetString(909);
            SyncFsmChangeStateButtonText.text = LocalizationManager.Instance.GetString(910);

            AsyncFsmTypeValueText.text = nameof(ExampleAsyncFsm);
            SyncFsmTypeValueText.text = nameof(ExampleSyncFsm);
            
            AsyncFsmDropdown.onValueChanged.AddListener(OnAsyncFsmDropdownChanged);
            SyncFsmDropdown.onValueChanged.AddListener(OnSyncFsmDropdownChanged);
            AsyncFsmStartButton.onClick.AddListener(OnAsyncFsmStartButtonClick);
            AsyncFsmChangeStateButton.onClick.AddListener(OnAsyncFsmChangeStateButtonClick);
            SyncFsmStartButton.onClick.AddListener(OnSyncFsmStartButtonClick);
            SyncFsmChangeStateButton.onClick.AddListener(OnSyncFsmChangeStateButtonClick);
            CloseButton.onClick.AddListener(OnCloseButtonClick);
            QuestionButton.onClick.AddListener(OnQuestionButtonClick);
            
            _exampleSyncFsm = new ExampleSyncFsm();
            _exampleAsyncFsm = new ExampleAsyncFsm();
            _exampleSyncFsm.Initialize();
            await _exampleAsyncFsm.Initialize();
            foreach (var state in _exampleSyncFsm.GetAllStates())
            {
                _syncOptionData.Add(new Dropdown.OptionData(state.GetType().Name));
            }
            foreach (var state in _exampleAsyncFsm.GetAllStates())
            {
                _asyncOptionData.Add(new Dropdown.OptionData(state.GetType().Name));
            }
            SyncFsmDropdown.options = _syncOptionData;
            AsyncFsmDropdown.options = _asyncOptionData;
        }
        
        protected override void Render(FsmPanelData data)
        {
            base.Render(data);
            AsyncFsmLogText.text = data.AsyncFsmStringBuilder.ToString();
            SyncFsmLogText.text = data.SyncFsmStringBuilder.ToString();
            AsyncFsmStateValueText.text = _exampleAsyncFsm.CurrentState != null ? _exampleAsyncFsm.CurrentState.ToString() : string.Empty;
            SyncFsmStateValueText.text = _exampleSyncFsm.CurrentState !=null ? _exampleSyncFsm.CurrentState.ToString() : string.Empty;
        }

        public override void DeInitialize()
        {
            base.DeInitialize();
            AsyncFsmDropdown.onValueChanged.RemoveListener(OnAsyncFsmDropdownChanged);
            SyncFsmDropdown.onValueChanged.RemoveListener(OnSyncFsmDropdownChanged);
            AsyncFsmStartButton.onClick.RemoveListener(OnAsyncFsmStartButtonClick);
            AsyncFsmChangeStateButton.onClick.RemoveListener(OnAsyncFsmChangeStateButtonClick);
            SyncFsmStartButton.onClick.RemoveListener(OnSyncFsmStartButtonClick);
            SyncFsmChangeStateButton.onClick.RemoveListener(OnSyncFsmChangeStateButtonClick);
            CloseButton.onClick.RemoveListener(OnCloseButtonClick);
            QuestionButton.onClick.RemoveListener(OnQuestionButtonClick);
        }
        

        private void OnAsyncFsmDropdownChanged(int value)
        {
            UIActions.FsmPanel_ChangeAsyncDropdownValue(value);
        }

        private void OnSyncFsmDropdownChanged(int value)
        {
            UIActions.FsmPanel_ChangeSyncDropdownValue(value);
        }

        private async void OnAsyncFsmStartButtonClick()
        {
            if (_exampleAsyncFsm.CurrentState == null)
            {
                await _exampleAsyncFsm.StartState<ExampleAsyncFsmState_1>();
            }
        }

        private void OnAsyncFsmChangeStateButtonClick()
        {
            if (_exampleAsyncFsm.CurrentState != null && UIWindow.Instance.GetUIData(out FsmPanelData data))
            {
                Type type = Type.GetType($"Start.{_asyncOptionData[data.AsyncValue].text}");
                _exampleAsyncFsm.ChangeState(type);
            }
        }

        private void OnSyncFsmStartButtonClick()
        {
            if (_exampleSyncFsm.CurrentState == null)
            {
                _exampleSyncFsm.StartState<ExampleSyncFsmState_1>();
            }
        }

        private void OnSyncFsmChangeStateButtonClick()
        {
            if (_exampleSyncFsm.CurrentState != null && UIWindow.Instance.GetUIData(out FsmPanelData data))
            {
                Type type = Type.GetType($"Start.{_syncOptionData[data.SyncValue].text}");
                _exampleSyncFsm.ChangeState(type);
            }
        }
        
        private void OnCloseButtonClick()
        {
            Close();
        }
        
        private void OnQuestionButtonClick()
        {
            UIActions.OpenRulePopup(911, 912);
        }
    }
}
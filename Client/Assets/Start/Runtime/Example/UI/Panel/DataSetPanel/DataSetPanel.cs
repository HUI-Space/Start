using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UI;

namespace Start
{
    public class DataSetPanel : UIPanel<DataSetPanelData>
    {
        public Text TitleText;

        public Text ApiText;
        
        public Text DataSetNameTitleText;
        public Text DataSetFieldTitleText;
        public Text DataSetFieldValueTitleText;
        public Text DataSetFieldValue;
        public Text SNMSButtonText;
        
        public Dropdown DataSetNameDropdown;
        public Dropdown DataSetFieldDropdown;
        
        //Simulates network message synchronization
        public Button SNMSButton;
        
        public Button CloseButton;
        public Button QuestionButton;
        
        private readonly List<Dropdown.OptionData> _dataSetNameOptionData = new List<Dropdown.OptionData>();
        private readonly List<Dropdown.OptionData> _dataSetFieldOptionData = new List<Dropdown.OptionData>();
        
        public override void Initialize()
        {
            base.Initialize();
            DataSetNameDropdown.onValueChanged.AddListener(OnDataSetNameDropdownChanged);
            DataSetFieldDropdown.onValueChanged.AddListener(OnDataSetFieldDropdownChanged);
            SNMSButton.onClick.AddListener(OnSNMSButtonClick);
            CloseButton.onClick.AddListener(OnCloseButtonClick);
            QuestionButton.onClick.AddListener(OnQuestionButtonClick);
            List<Type> types = AssemblyUtility.GetChildType(typeof(DataSetBase));
            foreach (Type type in types)
            {
                if (type.IsAbstract && !type.Name.Contains("Simulation"))
                {
                    continue;
                }
                _dataSetNameOptionData.Add(new Dropdown.OptionData(type.Name));
            }
            DataSetNameDropdown.options = _dataSetNameOptionData;
        }

        protected override void Render(DataSetPanelData uiData)
        {
            base.Render(uiData);
            TitleText.text = LocalizationManager.Instance.GetString(700);
            DataSetNameTitleText.text = LocalizationManager.Instance.GetString(701);
            DataSetFieldTitleText.text = LocalizationManager.Instance.GetString(702);
            DataSetFieldValueTitleText.text = LocalizationManager.Instance.GetString(703);
            SNMSButtonText.text = LocalizationManager.Instance.GetString(704);
            if (DataSetNameDropdown.value == 0)
            {
                ApiText.text = _tempScript_1;
            }
            else
            {
                ApiText.text = _tempScript_2;
            }
            
            string dataSetName = DataSetNameDropdown.options[DataSetNameDropdown.value].text;
            Type type = Type.GetType($"Start.{dataSetName}");
            if (type != null)
            {
                _dataSetFieldOptionData.Clear();
                foreach (var field in type.GetFields())
                {
                    _dataSetFieldOptionData.Add(new Dropdown.OptionData(field.Name));
                }
                DataSetFieldDropdown.options = _dataSetFieldOptionData;
            }

            DataSetBase dataSetBase = DataSetManager.Instance.GetDataSetBase(type);
            string itemField = DataSetFieldDropdown.options[DataSetFieldDropdown.value].text;
            var configItemField = dataSetBase.GetType().GetField(itemField);
            if (configItemField != null)
            {
                DataSetFieldValue.text = configItemField.GetValue(dataSetBase).ToString();
            }
        }

        public override void DeInitialize()
        {
            base.DeInitialize();
            DataSetNameDropdown.onValueChanged.RemoveListener(OnDataSetNameDropdownChanged);
            DataSetFieldDropdown.onValueChanged.RemoveListener(OnDataSetFieldDropdownChanged);
            SNMSButton.onClick.RemoveListener(OnSNMSButtonClick);
            CloseButton.onClick.RemoveListener(OnCloseButtonClick);
            QuestionButton.onClick.RemoveListener(OnQuestionButtonClick);
        }
        
        private void OnDataSetNameDropdownChanged(int index)
        {
            UIActions.RenderUI(nameof(DataSetPanel));
        }
        
        private void OnDataSetFieldDropdownChanged(int index)
        {
            UIActions.RenderUI(nameof(DataSetPanel));
        }
        
        private void OnSNMSButtonClick()
        {
            string dataSetName = DataSetNameDropdown.options[DataSetNameDropdown.value].text;
            Type type = Type.GetType($"Start.{dataSetName}");
            if (type != null)
            {
                DataSetBase dataSetBase = DataSetManager.Instance.GetDataSetBase(type);
                MethodInfo privateMethod_1 = typeof(DataSetBase).GetMethod("OnCallback",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                MethodInfo privateMethod_2 = type.GetMethod("OnSimulationCallback",
                    BindingFlags.NonPublic | BindingFlags.Instance);
                Simulation_ToC toC = new Simulation_ToC()
                {
                    Id = UnityEngine.Random.Range(0, 100000),
                    Age = UnityEngine.Random.Range(0, 100000),
                    Name = "Jack" + UnityEngine.Random.Range(0, 100000),
                };
                privateMethod_1.Invoke(dataSetBase, new object[] { uint.MinValue,toC });
                privateMethod_2.Invoke(dataSetBase, new object[] { uint.MinValue,toC });
            }
            UIActions.RenderUI(nameof(DataSetPanel));
        }
        private void OnCloseButtonClick()
        {
            Close();
        }
        
        private void OnQuestionButtonClick()
        {
            UIActions.OpenRulePopup(706, 707);
        }
        
        private string _tempScript_1 = @"public class SimulationDataSet_1 : DataSetBase
        {
            public int Id = 10086;
            
            public int Age = 18;

            public string Name = ""Jack"";
            
            
            public override void Register()
            {
                RegisterNetMessage<Simulation_ToC>(1,1,OnSimulationCallback);
            }

            public override void UnRegister()
            {
                UnRegisterNetMessage<Simulation_ToC>(1,1,OnSimulationCallback);
            }

            private void OnSimulationCallback(uint code, Simulation_ToC msg)
            {
                Id = msg.Id;
                Age = msg.Age;
                Name = msg.Name;
            }
        }";
        private string _tempScript_2 = @"public class SimulationDataSet_2 : DataSetBase
        {
            public int Id = 100001;
            
            public int Age = 28;

            public string Name = ""Mick"";
            
            
            public override void Register()
            {
                RegisterNetMessage<Simulation_ToC>(1,1,OnSimulationCallback);
            }

            public override void UnRegister()
            {
                UnRegisterNetMessage<Simulation_ToC>(1,1,OnSimulationCallback);
            }

            private void OnSimulationCallback(uint code, Simulation_ToC msg)
            {
                Id = msg.Id;
                Age = msg.Age;
                Name = msg.Name;
            }
        }";
    }
}
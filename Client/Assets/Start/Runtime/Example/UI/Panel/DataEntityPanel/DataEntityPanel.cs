using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.UI;

namespace Start
{
    public class DataEntityPanel : UIPanel<DataEntityPanelData>
    {
        public Text TitleText;
        
        public Text SimulationDataEntity_1TitleText;
        public Text SimulationDataEntity_1Text;
        public Text SimulationDataEntity_1FieldTitleText;
        public Text SimulationDataEntity_1FieldValueTitleText;
        public Text SimulationDataEntity_1FieldValue;
        
        public Text SimulationDataEntity_2TitleText;
        public Text SimulationDataEntity_2Text;
        public Text SimulationDataEntity_2FieldTitleText;
        public Text SimulationDataEntity_2FieldValueTitleText;
        public Text SimulationDataEntity_2FieldValue;
        
        public Text SNMSButtonText;
        public Text InputFieldText;
        
        public Dropdown SimulationDataEntity_1FieldDropdown;
        public Dropdown SimulationDataEntity_2FieldDropdown;

        public InputField SimulationDataEntity_2InputField;
        
        public Button SNMSButton;
        public Button CloseButton;
        public Button QuestionButton;
        
        private readonly List<Dropdown.OptionData> _simulationDataEntity_1FieldOptionData = new List<Dropdown.OptionData>();
        private readonly List<Dropdown.OptionData> _simulationDataEntity_2FieldOptionData = new List<Dropdown.OptionData>();
        public override void Initialize()
        {
            base.Initialize();
            SimulationDataEntity_1FieldDropdown.onValueChanged.AddListener(OnSimulationDataEntity_1FieldDropdownChanged);
            SimulationDataEntity_2FieldDropdown.onValueChanged.AddListener(OnSimulationDataEntity_2FieldDropdownChanged);
            SimulationDataEntity_2InputField.onValueChanged.AddListener(OnSimulationDataEntity_2InputFieldChanged);
            SNMSButton.onClick.AddListener(OnSNMSButtonClick);
            CloseButton.onClick.AddListener(OnCloseButtonClick);
            QuestionButton.onClick.AddListener(OnQuestionButtonClick);
                
            TitleText.text = LocalizationManager.Instance.GetString(800);
            SimulationDataEntity_1TitleText.text = LocalizationManager.Instance.GetString(801);
            SimulationDataEntity_1Text.text = "Start.SimulationDataEntity_1";
            SimulationDataEntity_1FieldTitleText.text = LocalizationManager.Instance.GetString(802);
            SimulationDataEntity_1FieldValueTitleText.text = LocalizationManager.Instance.GetString(803);
            SimulationDataEntity_2TitleText.text = LocalizationManager.Instance.GetString(804);
            SimulationDataEntity_2FieldTitleText.text = LocalizationManager.Instance.GetString(805);
            SimulationDataEntity_2FieldValueTitleText.text = LocalizationManager.Instance.GetString(806);
            SNMSButtonText.text = LocalizationManager.Instance.GetString(807);
            InputFieldText.text = LocalizationManager.Instance.GetString(808);
            
            Type type = Type.GetType("Start.SimulationDataEntity_1");
            if (type != null)
            {
                _simulationDataEntity_1FieldOptionData.Clear();
                foreach (var field in type.GetFields())
                {
                    _simulationDataEntity_1FieldOptionData.Add(new Dropdown.OptionData(field.Name));
                }
                SimulationDataEntity_1FieldDropdown.options = _simulationDataEntity_1FieldOptionData;
            }
            
            Type type2 = Type.GetType("Start.SimulationDataEntity_2");
            if (type2 != null)
            {
                _simulationDataEntity_2FieldOptionData.Clear();
                foreach (var field in type2.GetFields())
                {
                    _simulationDataEntity_2FieldOptionData.Add(new Dropdown.OptionData(field.Name));
                }
                SimulationDataEntity_2FieldDropdown.options = _simulationDataEntity_2FieldOptionData;
            }
            
        }

        protected override void Render(DataEntityPanelData uiData)
        {
            base.Render(uiData);
            string fieldName =_simulationDataEntity_1FieldOptionData[SimulationDataEntity_1FieldDropdown.value].text;
            SimulationDataEntity_1 simulationDataEntity1 = DataEntityManager.Instance.GetDataEntity<SimulationDataEntity_1>();
            SimulationDataEntity_1FieldValue.text = simulationDataEntity1.GetType().GetField(fieldName).GetValue(simulationDataEntity1).ToString();

            if (int.TryParse(SimulationDataEntity_2InputField.text, out int index))
            {
                SimulationDataEntity_2Text.text = $"Start.SimulationDataEntity_2<int> : {index}"; 
                string field2Name =_simulationDataEntity_2FieldOptionData[SimulationDataEntity_2FieldDropdown.value].text;
                SimulationDataEntity_2 simulationDataEntity2 = DataEntityManager.Instance.GetDataEntity<int,SimulationDataEntity_2>(index);
                SimulationDataEntity_2FieldValue.text = simulationDataEntity2.GetType().GetField(field2Name).GetValue(simulationDataEntity2).ToString();
            }
            else
            {
                SimulationDataEntity_2Text.text = string.Empty;
                SimulationDataEntity_2FieldValue.text = string.Empty;
            }
        }

        public override void DeInitialize()
        {
            base.DeInitialize();
            SimulationDataEntity_1FieldDropdown.onValueChanged.RemoveListener(OnSimulationDataEntity_1FieldDropdownChanged);
            SimulationDataEntity_2FieldDropdown.onValueChanged.RemoveListener(OnSimulationDataEntity_2FieldDropdownChanged);
            SNMSButton.onClick.RemoveListener(OnSNMSButtonClick);
            CloseButton.onClick.RemoveListener(OnCloseButtonClick);
            QuestionButton.onClick.RemoveListener(OnQuestionButtonClick);
        }
        
        private void OnSimulationDataEntity_1FieldDropdownChanged(int index)
        {
            UIActions.RenderUI(nameof(DataEntityPanel));
        }
        
        private void OnSimulationDataEntity_2FieldDropdownChanged(int index)
        {
            UIActions.RenderUI(nameof(DataEntityPanel));
        }

        private void OnSimulationDataEntity_2InputFieldChanged(string value)
        {
            if (int.TryParse(value,out _))
            {
                UIActions.RenderUI(nameof(DataEntityPanel));
            }
        }

        private void OnSNMSButtonClick()
        {
            Type type1 = Type.GetType("Start.SimulationDataSet_1");
            Type type2 = Type.GetType("Start.SimulationDataSet_1");
            SNMS(type1);
            SNMS(type2);
            
            void SNMS(Type type)
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
                if (privateMethod_1 != null) privateMethod_1.Invoke(dataSetBase, new object[] { uint.MinValue, toC });
                if (privateMethod_2 != null) privateMethod_2.Invoke(dataSetBase, new object[] { uint.MinValue, toC });
            }
            UIActions.RenderUI(nameof(DataEntityPanel));
        }
        
        private void OnCloseButtonClick()
        {
            Close();
        }

        private void OnQuestionButtonClick()
        {
            UIActions.OpenRulePopup(501, 502);
        }
    }
}
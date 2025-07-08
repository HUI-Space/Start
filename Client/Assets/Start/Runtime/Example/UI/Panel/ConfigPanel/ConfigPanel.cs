using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Common;
using UnityEngine;
using UnityEngine.UI;

namespace Start
{
    public class ConfigPanel : UIPanel<ConfigPanelData>
    {
        public Text TitleText;
        public Text ConfigNameTitleText;
        public Text ConfigFieldTitleText;
        public Text ConfigFieldResultTitleText;
        public Text InputConfigFieldTitleText;
        public Text JsonTitleText;
        
        public Text UsedTitleText;
        public Text UsedText;
        
        public Text ConfirmButtonText;
        
        public Text ConfigFieldResultText;
        public Text JsonText;
        
        public Dropdown ConfigDropdown;
        public Dropdown FieldDropdown;
        
        public InputField InputConfigField;
        
        public Button ConfirmButton;
        public Button CloseButton;
        public Button QuestionButton;
        
        private readonly List<Dropdown.OptionData> _configOptionData = new List<Dropdown.OptionData>();
        private readonly List<Dropdown.OptionData> _fieldOptionData = new List<Dropdown.OptionData>();
        private AsyncOperationHandle<TextAsset> _handle;
        public override void Initialize()
        {
            base.Initialize();
            ConfigDropdown.onValueChanged.AddListener(OnConfigDropdownChanged);
            FieldDropdown.onValueChanged.AddListener(OnFieldDropdownChanged);
            ConfirmButton.onClick.AddListener(OnConfirmButtonClick);
            CloseButton.onClick.AddListener(OnCloseButtonClick);
            QuestionButton.onClick.AddListener(OnQuestionButtonClick);
            List<Type> types = AssemblyUtility.GetChildType(typeof(IConfig));
            foreach (Type type in types)
            {
                if (type.IsAbstract)
                {
                    continue;
                }
                _configOptionData.Add(new Dropdown.OptionData(type.Name));
            }
            ConfigDropdown.options = _configOptionData;
        }

        protected override void Render(ConfigPanelData uiData)
        {
            base.Render(uiData);
            TitleText.text = LocalizationManager.Instance.GetString(400,TitleText.text);
            ConfigNameTitleText.text = LocalizationManager.Instance.GetString(401,ConfigNameTitleText.text);
            ConfigFieldTitleText.text = LocalizationManager.Instance.GetString(402,ConfigFieldTitleText.text);
            InputConfigFieldTitleText.text = LocalizationManager.Instance.GetString(403,InputConfigFieldTitleText.text);
            ConfirmButtonText.text = LocalizationManager.Instance.GetString(404,ConfirmButtonText.text);
            ConfigFieldResultTitleText.text = LocalizationManager.Instance.GetString(405,ConfigFieldResultTitleText.text);
            JsonTitleText.text = LocalizationManager.Instance.GetString(406,ConfigFieldResultTitleText.text);
            UsedTitleText.text = LocalizationManager.Instance.GetString(407,UsedTitleText.text);
            UsedText.text = LocalizationManager.Instance.GetString(408,UsedText.text);
            
            string configName = ConfigDropdown.options[ConfigDropdown.value].text;
            Type type = Type.GetType($"Start.{configName}");
            Type baseType = type.BaseType;
            if (baseType != null)
            {
                Type[] genericArguments = baseType.GetGenericArguments();
                if (genericArguments.Length > 0)
                {
                    _fieldOptionData.Clear();
                    Type dataType = genericArguments[0];
                    foreach (var field in dataType.GetFields())
                    {
                        _fieldOptionData.Add(new Dropdown.OptionData(field.Name));
                    }
                    FieldDropdown.options = _fieldOptionData;
                }
            }
            
            if (type.Name.Equals(nameof(LocalizationConfig)))
            {
                configName = $"{nameof(LocalizationConfig)}_{(ELocalization)LocalizationManager.Instance.LanguageId}";
            }
            string path = AssetConfig.GetAssetPath(EAssetType.Config, configName + AssetConfig.Json);
            if (_handle != null)
            {
                ResourceManager.Instance.Unload(_handle);
            }
            _handle = ResourceManager.Instance.LoadAsset<TextAsset>(path);
            JsonText.text = _handle.Result.text;
            IConfig config = ConfigManager.Instance.GetConfig(type);
            
            
            if (!string.IsNullOrEmpty(InputConfigField.text) && int.TryParse(InputConfigField.text, out int Id))
            {
                var field = config.GetType().GetField("DataList");
                object dataList = field.GetValue(config);
                bool contain = false;
                foreach (var item in (IEnumerable)dataList)
                {
                    if (item is ConfigItemBase configItemBase && configItemBase.Id == Id)
                    {
                        string itemField = FieldDropdown.options[FieldDropdown.value].text;
                        var configItemField = item.GetType().GetField(itemField);
                        if (configItemField != null)
                        {
                            ConfigFieldResultText.text = configItemField.GetValue(item).ToString();
                            contain = true;
                        }
                        break;
                    }
                }
                if (!contain)
                {
                    ConfigFieldResultText.text = LocalizationManager.Instance.GetString(409,"");
                }
            }
            
        }

        public override void DeInitialize()
        {
            base.DeInitialize();
            ConfigDropdown.onValueChanged.RemoveListener(OnConfigDropdownChanged);
            FieldDropdown.onValueChanged.RemoveListener(OnFieldDropdownChanged);
        }

        private void OnConfigDropdownChanged(int arg0)
        {
            UIActions.UpdateConfigPanel();
        }

        private void OnFieldDropdownChanged(int arg0)
        {
            UIActions.UpdateConfigPanel();
        }

        private void OnConfirmButtonClick()
        {
            UIActions.UpdateConfigPanel();
        }
        
        private void OnCloseButtonClick()
        {
            Close();
        }
        
        private void OnQuestionButtonClick()
        {
            UIActions.OpenRulePopup(410, 411);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Text;
using Newtonsoft.Json.Linq;
using UnityEditor;
using UnityEngine;

namespace Start.Editor
{
    public class ConfigController
    {
        public static ConfigController Instance { get; private set;}
        
        public ExcelConfig ExcelConfig { get ; private set; }
        
        public Dictionary<string,string> Sheets { get ; private set; }

        private ConfigBuildView _configBuildView;
        private ConfigSettingView _configSettingView;
        public void OnEnable(ConfigBuildView configBuildView, ConfigSettingView configSettingView)
        {
            Instance = this;
            _configBuildView = configBuildView;
            _configSettingView = configSettingView;
            ExcelConfig = Utility.LoadJsonConfig<ExcelConfig>(ConfigPath.ExcelConfigPath);
            Sheets = new Dictionary<string,string>();
            LoadSheets();
        }

        private void LoadSheets()
        {
            List<string> excels = ExcelHelper.GetExcels(ExcelConfig.ExcelPath);
            foreach (string excelPath in excels)
            {
                List<string> sheetNames = ExcelHelper.GetSheetNames(excelPath);
                for (int i = ExcelConfig.ExcelStartSheet; i < sheetNames.Count; i++)
                {
                    Sheets.Add(sheetNames[i], excelPath); 
                }
            }
        }

        public void OnDestroy()
        {
            Instance = default;
            ExcelConfig = default;
            Sheets.Clear();
            Sheets = default;
        }

        public void SaveConfig()
        {
            string json = JsonUtility.ToJson(ExcelConfig, true);
            FileUtility.WriteAllText(ConfigPath.ExcelConfigPath, json);
            Sheets.Clear();
            LoadSheets();
            _configBuildView.OnReload();
        }

        public void OutputScript(List<string> sheets)
        {
            foreach (string sheet in sheets)
            {
                if (Sheets.TryGetValue(sheet, out string excelPath))
                {
                    Dictionary<int,ConfigFieldItem> configs = new Dictionary<int,ConfigFieldItem>();
                    DataTable dataTable = ExcelHelper.GetDataTable(excelPath, sheet);
                    if (dataTable == null)
                    {
                        continue;
                    }
                    
                    if (dataTable.Rows.Count < ExcelConfig.ConfigStart)
                    {
                        Debug.LogError($"{excelPath} {sheet} 配置不完整");
                        continue;
                    }

                    int count = dataTable.Rows[ExcelConfig.ConfigFieldType].ItemArray.Length;
                    
                    for (int i = 0; i < count; i++)
                    {
                        string fieldType = dataTable.Rows[ExcelConfig.ConfigFieldType].ItemArray[i].ToString();
                        string fieldName = dataTable.Rows[ExcelConfig.ConfigFieldName].ItemArray[i].ToString();
                        string fieldDescription = dataTable.Rows[ExcelConfig.ConfigFieldDescription].ItemArray[i].ToString();
                        
                        if (i == 0)
                        {
                            if (string.IsNullOrEmpty(fieldType))
                            {
                                Debug.LogError($"{excelPath} {sheet} 配置错误 第{ExcelConfig.ConfigFieldType}行 第{i}列 必须为字段类型int ");
                                continue;
                            }
                            if (string.IsNullOrEmpty(fieldName) || !fieldName.Equals(nameof(ConfigItemBase.Id)))
                            {
                                Debug.LogError($"{excelPath} {sheet} 配置错误 第{ExcelConfig.ConfigFieldName}行 第{i}列 必须为字段名称：{nameof(ConfigItemBase.Id)} ");
                                continue;
                            }
                            continue;
                        }
                        
                        if (!string.IsNullOrEmpty(fieldType) || !string.IsNullOrEmpty(fieldName))
                        {
                            ConfigFieldItem configFieldItem = new ConfigFieldItem{Type = fieldType, Name = fieldName, Description = fieldDescription};
                            configs.Add(i, configFieldItem);
                        }
                    }
                    
                    StringBuilder stringBuilder = new StringBuilder();
                    stringBuilder.AppendLine("using System;");
                    stringBuilder.AppendLine("namespace Start");
                    stringBuilder.AppendLine("{");
                    stringBuilder.AppendLine("    [Serializable]");
                    stringBuilder.AppendLine($"    public class {sheet}Item : ConfigItemBase");
                    stringBuilder.AppendLine("    {");
                    foreach (var item in configs)
                    {
                        stringBuilder.AppendLine("        /// <summary>");
                        stringBuilder.AppendLine($"        /// {item.Value.Description}");
                        stringBuilder.AppendLine("        /// </summary>");
                        stringBuilder.AppendLine($"        public {item.Value.Type} {item.Value.Name};");
                        stringBuilder.AppendLine();
                    }
                    stringBuilder.AppendLine("    }");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("    [Serializable]");
                    stringBuilder.AppendLine($"    public class {sheet} : ConfigBase<{sheet}Item>");
                    stringBuilder.AppendLine("    {");
                    stringBuilder.AppendLine();
                    stringBuilder.AppendLine("    }");
                    stringBuilder.AppendLine("}");
                    FileUtility.WriteAllText(Path.Combine(ExcelConfig.OutputScriptPath , sheet + ".cs") , stringBuilder.ToString());
                }
            }
        }

        
        public void OutputConfig(List<string> sheets)
        {
            foreach (string sheet in sheets)
            {
                if (Sheets.TryGetValue(sheet, out string excelPath))
                {
                    DataTable dataTable = ExcelHelper.GetDataTable(excelPath, sheet);
                    List<List<ConfigItem>> allConfigItems = new List<List<ConfigItem>>();
                    HashSet<string> Ids = new HashSet<string>();

                    string fieldType = dataTable.Rows[ExcelConfig.ConfigFieldType].ItemArray[0].ToString();
                    string fieldName = dataTable.Rows[ExcelConfig.ConfigFieldName].ItemArray[0].ToString();
                    if (string.IsNullOrEmpty(fieldType))
                    {
                        EditorUtility.DisplayDialog("提示",$"{excelPath} {sheet} 配置错误 第{ExcelConfig.ConfigFieldType + 1}行 第1列 必须为字段类型int ","确定");
                        continue;
                    }
                    if (string.IsNullOrEmpty(fieldName) || !fieldName.Equals(nameof(ConfigItemBase.Id)))
                    {
                        EditorUtility.DisplayDialog("提示",$"{excelPath} {sheet} 配置错误 第{ExcelConfig.ConfigFieldName + 1}行 第1列 必须为字段名称：{nameof(ConfigItemBase.Id)} ","确定");
                        continue;
                    }
                    
                    for (int i = ExcelConfig.ConfigStart; i < dataTable.Rows.Count; i++)
                    {
                        string Id = dataTable.Rows[i].ItemArray[0].ToString();
                        if (string.IsNullOrEmpty(dataTable.Rows[i].ItemArray[0].ToString()))
                        {
                            continue;
                        }
                        List<ConfigItem> configItems = new List<ConfigItem>();
                        for (int j = 0; j < dataTable.Rows[i].ItemArray.Length; j++)
                        {
                            string name = dataTable.Rows[ExcelConfig.ConfigFieldName].ItemArray[j].ToString();
                            string type = dataTable.Rows[ExcelConfig.ConfigFieldType].ItemArray[j].ToString();
                            string value = dataTable.Rows[i].ItemArray[j].ToString();
                            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(type))
                            {
                                continue;
                            }
                            configItems.Add(new ConfigItem{Type = type, Name = name, Value = value});
                        }
                        if (!Ids.Add(Id))
                        {
                            EditorUtility.DisplayDialog("提示", $"{excelPath} {sheet} 配置错误 Id重复：{Id} ","确定");
                        }
                        allConfigItems.Add(configItems);
                    }
                    TypeHelper.OutputConfig(allConfigItems, Path.Combine(ExcelConfig.OutputConfigPath , sheet + ".json"));
                }
            }
            
        }
        
        private struct ConfigFieldItem
        {
            public string Type;
            public string Name;
            public string Description;
        }
        
        public struct ConfigItem
        {
            public string Type;
            public string Name;
            public string Value;
        }
    }
}
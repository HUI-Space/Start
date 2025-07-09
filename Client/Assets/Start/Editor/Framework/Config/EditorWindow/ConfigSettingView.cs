using System;
using System.Collections.Generic;
using System.Text;
using PlasticGui.Gluon.WorkspaceWindow.Views.IncomingChanges;
using UnityEditor;
using UnityEngine;

namespace Start.Editor
{
    public class ConfigSettingView
    {
        private ExcelConfig _excelConfig => ConfigController.Instance.ExcelConfig;
        
        private string _excelPath;
        private string _outputScriptPath;
        private string _outputConfigPath;
        private string _excelStartSheet;
        private string _configFieldTypeRow;
        private string _configFieldNameRow;
        private string _configFieldDescriptionRow;
        private string _configStartRow;
        private string _save;
        
        public void OnEnable()
        {
            _excelPath = "Excel路径:";
            _outputScriptPath = "输出脚本路径:";
            _outputConfigPath = "输出配置路径:";
            _excelStartSheet = "Excel起始页:";
            _configFieldTypeRow = "配置类型行:";
            _configFieldNameRow = "配置名称行:";
            _configFieldDescriptionRow = "配置描述行:";
            _configStartRow = "配置起始行:";
            _save = "保存";
        }
        
        public void OnGUI(Rect position)
        {
            EditorGUILayoutExtension.OpenFolderPanel(_excelPath, ref _excelConfig.ExcelPath);
            EditorGUILayoutExtension.OpenFolderPanel(_outputScriptPath, ref _excelConfig.OutputScriptPath);
            EditorGUILayoutExtension.OpenFolderPanel(_outputConfigPath, ref _excelConfig.OutputConfigPath);
            _excelConfig.ExcelStartSheet = EditorGUILayout.IntField(_excelStartSheet, _excelConfig.ExcelStartSheet);
            _excelConfig.ConfigFieldType = EditorGUILayout.IntField(_configFieldTypeRow,_excelConfig.ConfigFieldType);
            _excelConfig.ConfigFieldName = EditorGUILayout.IntField(_configFieldNameRow, _excelConfig.ConfigFieldName);
            _excelConfig.ConfigFieldDescription = EditorGUILayout.IntField(_configFieldDescriptionRow,_excelConfig.ConfigFieldDescription);
            _excelConfig.ConfigStart = EditorGUILayout.IntField(_configStartRow, _excelConfig.ConfigStart);
            if (GUILayout.Button(_save))
            {
                ConfigController.Instance.SaveConfig();
            }
        }
        
        public void OnDestroy()
        {
            _excelPath = default;
            _outputScriptPath = default;
            _outputConfigPath = default;
            _excelStartSheet = default;
            _configFieldTypeRow = default;
            _configFieldNameRow = default;
            _configFieldDescriptionRow = default;
            _configStartRow = default;
        }
    }
}
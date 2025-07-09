using System;
using System.Collections;
using System.Collections.Generic;
using Start;
using UnityEditor;
using UnityEngine;

public class ConfigData
{
    private class ConfigItemData
    {
        public IEnumerable DataList;
        public readonly Dictionary<string,List<object>> FieldDataDic = new Dictionary<string, List<object>>();
    }
    
    private static readonly Dictionary<string,ConfigItemData> _configDataDic = new Dictionary<string, ConfigItemData>();
    
    public static List<T> GetExcelFieldData<T>(string configName, string fieldName)
    {
        Type configType = Type.GetType($"Start.{configName},Client.Runtime");
        if (configType == null)
        {
            return null;
        }
        
        List<T> fieldDataT = new List<T>();
        string configPath = AssetConfig.GetAssetPath(EAssetType.Config, configName + AssetConfig.Json);
        if (_configDataDic.TryGetValue(configPath, out ConfigItemData configData))
        {
            if (!configData.FieldDataDic.TryGetValue(fieldName, out List<object> fieldData))
            {
                fieldData = new List<object>();
                foreach (var item in configData.DataList)
                {
                    var configItemField = item.GetType().GetField(fieldName);
                    if (configItemField != null)
                    {
                        fieldData.Add(configItemField.GetValue(item));
                    }
                }
                configData.FieldDataDic.Add(fieldName, fieldData);
            }
            foreach (var item in fieldData)
            {
                fieldDataT.Add((T)item);
            }
        }
        else
        {
            configData = new ConfigItemData();
            TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(configPath);
            IConfig config = (IConfig)JsonUtility.FromJson(textAsset.text, configType);
            var field = config.GetType().GetField("DataList");
            configData.DataList = (IEnumerable)field.GetValue(config);
            List<object> fieldData = new List<object>();
            foreach (var item in configData.DataList)
            {
                var configItemField = item.GetType().GetField(fieldName);
                if (configItemField != null)
                {
                    var data = configItemField.GetValue(item);
                    fieldData.Add(data);
                    fieldDataT.Add((T)data);
                }
            }
            configData.FieldDataDic.Add(fieldName, fieldData);
            _configDataDic.Add(configPath, configData);
        }
        
        return fieldDataT;
    }
}
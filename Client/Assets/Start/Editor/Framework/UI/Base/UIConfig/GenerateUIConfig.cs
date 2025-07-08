using System;
using System.IO;
using Common;
using UnityEditor;
using UnityEngine;

namespace Start
{
    /// <summary>
    /// 生成UI配置文件
    /// </summary>
    public static class GenerateUIConfig 
    {
        public static void Generate()
        {
            UIConfig uiConfig = new UIConfig();
            int index = 1;
            foreach (Type type in TypeCache.GetTypesDerivedFrom(typeof(UIPanel<>)))
            {
                if (type.BaseType != null)
                {
                    Type dataType = type.BaseType.GetGenericArguments()[0];
                    UIConfigItem item = new UIConfigItem();
                    item.Id = index++;
                    item.UIName = type.Name;
                    item.UIType = EUIType.Panel;
                    item.UIDataName = dataType.FullName;
                    uiConfig.DataList.Add(item);
                }
            }
            
            foreach (Type type in TypeCache.GetTypesDerivedFrom(typeof(UIPopup<>)))
            {
                if (type.BaseType != null)
                {
                    Type dataType = type.BaseType.GetGenericArguments()[0];
                    UIConfigItem item = new UIConfigItem();
                    item.Id = index++;
                    item.UIName = type.Name;
                    item.UIType = EUIType.PopUp;
                    item.UIDataName = dataType.FullName;
                    uiConfig.DataList.Add(item);
                }
            }

            string json = UnityUtility.ToJson(uiConfig);
            FileUtility.WriteAllText(Path.Combine(Application.dataPath, 
                AssetConfig.GetAssetPathWithoutAssets(EAssetType.Config,$"{nameof(UIConfig)}{AssetConfig.Json}")).RegularPath(), json);
            AssetDatabase.Refresh();
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(ExcelConfigReferenceAttribute))]
public class ExcelConfigReferenceDrawer : PropertyDrawer
{
    private ExcelConfigReferenceAttribute configRefAttribute => (ExcelConfigReferenceAttribute) attribute;
    int _selectIndex = -1;
    private List<string> _descList = new List<string>();
    private string _excelInfo => $"[{configRefAttribute.excelName}-{configRefAttribute.fieldName}]  ";

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        if (property.serializedObject.isEditingMultipleObjects)
        {
            return;
        }

        string fieldType = property.type;
        switch (fieldType)
        {
            case "string":
                DrawPopup<string>(position, property);
                return;
            case "float":
                DrawPopup<float>(position, property);
                return;
            case "int":
                DrawPopup<int>(position, property);
                return;
            case "long":
                DrawPopup<long>(position, property);
                return;
            case "bool":
                DrawPopup<bool>(position, property);
                return;
        }

        base.OnGUI(position, property, label);
        Debug.LogError("类型不支持 " + fieldType);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        if (property.serializedObject.isEditingMultipleObjects)
        {
            return 0;
        }

        return base.GetPropertyHeight(property, label);
    }

    private void DrawPopup<T>(Rect position, SerializedProperty prop)
    {
        List<T> list = ExcelConfigData.GetExcelFieldData<T>(configRefAttribute.excelName, configRefAttribute.fieldName);
        list = FilterListByID(list);
        switch (list)
        {
            case List<string> stringList:
                SetSavedDataIndex(stringList, prop.stringValue, prop);
                break;
            case List<int> intList:
                SetSavedDataIndex(intList, prop.intValue, prop);
                break;
            case List<float> floatList:
                SetSavedDataIndex(floatList, prop.floatValue, prop);
                break;
            case List<bool> boolList:
                SetSavedDataIndex(boolList, prop.boolValue, prop);
                break;
            case List<long> longList:
                SetSavedDataIndex(longList, prop.longValue, prop);
                break;
        }

        ExcelRefDropdown.Draw(prop, position, _excelInfo, _selectIndex, ConstructDescArray(list), (index) =>
        {
            EditorGUI.BeginChangeCheck();
            if (prop.serializedObject.targetObject is EmittedScriptableObject<T> emitted)
            {
                EditorUtility.SetDirty(emitted);
                // Debug.Log(prop.serializedObject.hasModifiedProperties);
                // Debug.Log(prop.serializedObject.targetObjects.Length);
            }
            else
            {
                prop.serializedObject.UpdateIfRequiredOrScript();
                switch (list)
                {
                    case List<string> stringList:
                        prop.stringValue = stringList[index];
                        Debug.Log("设置值为 " + stringList[index]);
                        break;
                    case List<int> intList:
                        prop.intValue = intList[index];
                        break;
                    case List<float> floatList:
                        prop.floatValue = floatList[index];
                        break;
                    case List<bool> boolList:
                        prop.boolValue = boolList[index];
                        break;
                    case List<long> longList:
                        prop.longValue = longList[index];
                        break;
                }
            }

            prop.serializedObject.ApplyModifiedProperties();
            _selectIndex = index;
            EditorGUI.EndChangeCheck();
        });
    }

    /// <summary>
    /// 设置初始索引并保存初始值，如果原来有值则查找该值在List中的index，没有则为0
    /// </summary>
    void SetSavedDataIndex<T>(List<T> valueList, T value, SerializedProperty prop)
    {
        if (_selectIndex == -1)
        {
            T savedValue = value;
            _selectIndex = valueList.FindIndex(0, val => { return val.Equals(savedValue); });
            _selectIndex = _selectIndex == -1 ? 0 : _selectIndex;
            T currentSelectVal = valueList[_selectIndex];
            switch (currentSelectVal)
            {
                case string str:
                    prop.stringValue = str;
                    break;
                case int intVal:
                    prop.intValue = intVal;
                    break;
                case float floatVal:
                    prop.floatValue = floatVal;
                    break;
                case bool boolVal:
                    prop.boolValue = boolVal;
                    break;
                case long longVal:
                    prop.longValue = longVal;
                    break;
            }
        }
    }

    /// <summary>
    /// 下拉列表描述
    /// </summary>
    /// <param name="valueList">字段值list</param>
    string[] ConstructDescArray<T>(List<T> valueList)
    {
        if (!string.IsNullOrEmpty(configRefAttribute.descFieldName))
        {
            _descList = ExcelConfigData.GetExcelFieldData<string>(configRefAttribute.excelName, configRefAttribute.descFieldName);
            _descList = FilterListByID(_descList);
        }

        string[] descArray = new string[valueList.Count];
        if (_descList != null && _descList.Count == valueList.Count)
        {
            for (int i = 0; i < descArray.Length; i++)
            {
                descArray[i] = $"{_excelInfo} {valueList[i]}({_descList[i]})";
            }
        }
        else
        {
            for (int i = 0; i < descArray.Length; i++)
            {
                descArray[i] = $"{_excelInfo}  {valueList[i]}";
            }
        }

        return descArray;
    }

    private List<T> FilterListByID<T>(List<T> rawList)
    {
        if (rawList == null)
        {
            return null;
        }

        long startID = configRefAttribute.idStart;
        long endID = configRefAttribute.idEnd;
        if (startID != -1)
        {
            try
            {
                List<long> idList = ExcelConfigData.GetExcelFieldData<long>(configRefAttribute.excelName, "Id").ToList();
                List<long> indexNeedRemove = new List<long>();
                for (int i = 0; i < idList.Count; i++)
                {
                    long currentID = idList[i];
                    // 移除ID范围外的值
                    if (currentID < startID || currentID > endID)
                    {
                        indexNeedRemove.Add(i);
                    }
                }

                foreach (int index in indexNeedRemove.OrderByDescending(x => x))
                {
                    rawList.RemoveAt(index);
                }
            }
            finally
            {
            }
        }

        return rawList;
    }
}
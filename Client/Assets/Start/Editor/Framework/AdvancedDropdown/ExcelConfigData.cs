using System.Collections.Generic;

public class ExcelConfigData
{
    public static List<T> GetExcelFieldData<T>(string excelName, string fieldName)
    {
        /*if (!TypeIsCorrect<T>(excelName, fieldName))
        {
            return null;
        }*/
        
        /*string path = excelName
        TextAsset textAsset = AssetDatabase.LoadAssetAtPath<TextAsset>(path);
        
        List<T> resultList = new List<T>();
        List<object> fieldValueList = excelFieldListDic[excelName][fieldName];
        for (int i = 0; i < fieldValueList.Count; i++)
        {
            resultList.Add((T) fieldValueList[i]);
        }*/
        List<int> resultList = new List<int>();
        
        for (int i = 0; i < 10; i++)
        {
            resultList.Add(i);
        }
        
        return resultList as List<T>;
    }
}
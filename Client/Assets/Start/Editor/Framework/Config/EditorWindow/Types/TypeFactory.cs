using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;

namespace Start.Editor
{
    public class TypeFactory
    {
        public Dictionary<string, TType> _simple = new Dictionary<string, TType>()
        {
            ["bool"] = new TBool(),
            ["int"] = new TInt(),
            ["uint"] = new TUInt(),
            ["float"] = new TFloat(),
            ["double"] = new TDouble(),
            ["long"] = new TLong(),
            ["ulong"] = new TULong(),
            ["short"] = new TShort(),
            ["ushort"] = new TUShort(),
            ["byte"] = new TByte(),
            ["sbyte"] = new TSByte(),
            ["char"] = new TChar(),
            ["string"] = new TString(),
            ["DateTime"] = new TDateTime(),
        };
        
        
        public void OutputConfig(List<List<ConfigController.ConfigItem>> configItems,string path)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine("  \"DataList\": [");
            for (int i = 0; i < configItems.Count; i++)
            {
                stringBuilder.AppendLine("    {");
                for (int j = 0; j < configItems[i].Count; j++)
                {
                    var configItem = configItems[i][j];
                    if (TryGetType(configItem.Type,out TType type))
                    {
                        stringBuilder.AppendLine(j == configItems[i].Count - 1
                            ? $"      \"{configItem.Name}\": {type.GetJsonFormat(configItem.Value)}"
                            : $"      \"{configItem.Name}\": {type.GetJsonFormat(configItem.Value)},");
                    }
                    else
                    {
                        //出错
                        EditorUtility.DisplayDialog("提示", $"不存在类型：{configItem.Type}请重新配置 ","确定");
                        return;
                    }
                }
                stringBuilder.AppendLine(i == configItems.Count - 1 ? "    }" : "    },");
            }
            stringBuilder.AppendLine("  ]");
            stringBuilder.AppendLine("}");
            JToken parsedJson = JToken.Parse(stringBuilder.ToString());
        
            // 格式化 JSON 字符串
            string formattedJson = parsedJson.ToString(Formatting.Indented);
            FileUtility.WriteAllText(path,formattedJson);
        }

        public bool TryGetType(string typeName,out TType type)
        {
            if (_simple.TryGetValue(typeName, out type))
            {
                return true;
            }
            if (typeName.EndsWith("[]"))
            {
                string typeName1 = typeName.Remove(typeName.Length - 2,2);
                if (TryGetType(typeName1,out TType type1))
                {
                    type = new TArray(type1);
                    return true;
                }
            }
            if (typeName.EndsWith("[][]"))
            {
                string typeName1 = typeName.Remove(typeName.Length - 2,4);
                if (TryGetType(typeName1,out TType type1))
                {
                    type = new TDoubleArray(type1);
                    return true;
                }
            }
            if (typeName.StartsWith("List<") && typeName.EndsWith(">"))
            {
                string typeName1 = typeName.Remove(typeName.Length - 1, 1).Remove(0, 5);
                if (TryGetType(typeName1,out TType type1))
                {
                    type = new TList(type1);
                    return true;
                }
            }
            if (typeName.StartsWith("HashSet<") && typeName.EndsWith(">"))
            {
                string typeName1 = typeName.Remove(typeName.Length - 1, 1).Remove(0, 8);
                if (TryGetType(typeName1,out TType type1))
                {
                    type = new THashSet(type1);
                    return true;
                }
            }
            if (typeName.StartsWith("Dictionary<") && typeName.EndsWith(">"))
            {
                string genericParams = typeName.Remove(typeName.Length - 1, 1).Remove(0, 11);
                string[] paramTypes = genericParams.Split(',');
                /*List<string> paramTypes = ParseInnerTypes(genericParams);*/
                if (paramTypes.Length == 2)
                {
                    if (TryGetType(paramTypes[0], out TType type1) && TryGetType(paramTypes[1], out TType type2))
                    {
                        type = new TDictionary(type1, type2);
                        return true;
                    }
                }
            }
            return false;
        }
        
        /*static List<string> ParseInnerTypes(string input)
        {
            List<string> types = new List<string>();
            int startIndex = 0;
            int bracketCount = 0;

            for (int i = 0; i < input.Length; i++)
            {
                if (input[i] == '<')
                {
                    bracketCount++;
                }
                else if (input[i] == '>')
                {
                    bracketCount--;
                }
                else if (input[i] == ',' && bracketCount == 0)
                {
                    types.Add(input.Substring(startIndex, i - startIndex).Trim());
                    startIndex = i + 1;
                }
            }

            types.Add(input.Substring(startIndex).Trim());
            return types;
        }*/
    }
}
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEditor;

namespace Start.Editor
{
    public static class TypeHelper
    {
        public static void OutputConfig(List<List<ConfigController.ConfigItem>> configItems,string path)
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
                    if (TypeFactory.TryGetType(configItem.Type,out TType type))
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

        
    }
}
using System.Collections.Generic;
using System.Linq;

namespace Start
{
    public static class TypeFactory
    {
        private static readonly Dictionary<string, TType> _simple = new Dictionary<string, TType>()
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
        

        public static bool TryGetType(string typeName,out TType type)
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
            if (typeName.EndsWith("[,]"))
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
                if (TryParseOuterDictionaryTypes(typeName,out string outerKeyTypeString, out string outerValueTypeString))
                {
                    if (TryGetType(outerKeyTypeString, out TType type1) && TryGetType(outerValueTypeString, out TType type2))
                    {
                        type = new TDictionary(type1, type2);
                        return true;
                    }
                }
            }
            return false;
        }
        
        
        public static bool TryParseOuterDictionaryTypes(
            string typeString, 
            out string outerKeyTypeString, 
            out string outerValueTypeString)
        {
            outerKeyTypeString = null;
            outerValueTypeString = null;

            // 基本验证
            if (string.IsNullOrWhiteSpace(typeString))
                return false;

            // 检查是否是Dictionary泛型类型
            const string dictPrefix = "Dictionary<";
            if (!typeString.StartsWith(dictPrefix) || !typeString.EndsWith(">"))
                return false;

            // 提取泛型参数部分（去掉"Dictionary<"和最后的">"）
            string genericContent = typeString.Substring(dictPrefix.Length, typeString.Length - dictPrefix.Length - 1);
            if (string.IsNullOrWhiteSpace(genericContent))
                return false;

            // 分割外层泛型参数（处理嵌套泛型的情况）
            var parameters = SplitGenericParameters(genericContent).ToList();
            if (parameters.Count != 2)
                return false;

            outerKeyTypeString = parameters[0].Trim();
            outerValueTypeString = parameters[1].Trim();
            return true;
        }
        
        /// <summary>
        /// 分割泛型参数（处理嵌套<>的情况）
        /// </summary>
        private static IEnumerable<string> SplitGenericParameters(string input)
        {
            int startIndex = 0;
            int bracketDepth = 0;

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (c == '<')
                {
                    bracketDepth++;
                }
                else if (c == '>')
                {
                    bracketDepth--;
                    if (bracketDepth < 0)
                        bracketDepth = 0; // 处理不匹配的情况
                }
                else if (c == ',' && bracketDepth == 0)
                {
                    // 只有当括号深度为0时，才是顶级参数的分隔符
                    yield return input.Substring(startIndex, i - startIndex);
                    startIndex = i + 1;
                }
            }

            // 返回最后一个参数
            if (startIndex <= input.Length - 1)
                yield return input.Substring(startIndex);
        }
    }
}
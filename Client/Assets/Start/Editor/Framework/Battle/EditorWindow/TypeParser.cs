using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Start
{
    public static class TypeParser
    {
        private static readonly Dictionary<string, Type> _containerAliasMap = new Dictionary<string, Type>()
        {
            // 泛型集合
            { "List", typeof(List<>) },
            { "Dictionary", typeof(Dictionary<,>) },
            { "HashSet", typeof(HashSet<>) },
            { "Queue", typeof(Queue<>) },
            { "Stack", typeof(Stack<>) },
            { "LinkedList", typeof(LinkedList<>) },
            { "SortedList", typeof(SortedList<,>) },
            { "SortedDictionary", typeof(SortedDictionary<,>) },
            { "SortedSet", typeof(SortedSet<>) },

            // 泛型接口
            { "IList", typeof(IList<>) },
            { "IDictionary", typeof(IDictionary<,>) },
            { "ICollection", typeof(ICollection<>) },
            { "IEnumerable", typeof(IEnumerable<>) },
            { "IEnumerator", typeof(IEnumerator<>) },

            // 其他常用集合
            { "ObservableCollection", typeof(ObservableCollection<>) },
            { "ReadOnlyCollection", typeof(ReadOnlyCollection<>) }
        };


        // 获取C#数据类型别名映射表
        private static readonly Dictionary<string, Type> _typeAliasMap = new Dictionary<string, Type>()
        {
            // 整数类型
            { "sbyte", typeof(sbyte) },
            { "byte", typeof(byte) },
            { "short", typeof(short) },
            { "ushort", typeof(ushort) },
            { "int", typeof(int) },
            { "uint", typeof(uint) },
            { "long", typeof(long) },
            { "ulong", typeof(ulong) },

            // 浮点类型
            { "float", typeof(float) },
            { "double", typeof(double) },
            { "decimal", typeof(decimal) },

            // 字符和布尔类型
            { "char", typeof(char) },
            { "bool", typeof(bool) },

            // 字符串和对象
            { "string", typeof(string) },
            { "object", typeof(object) },

            // 指针类型（不常用）
            { "IntPtr", typeof(IntPtr) },
            { "UIntPtr", typeof(UIntPtr) },

            // 可空值类型的基础类型
            { "void", typeof(void) }
        };


        /// <summary>
        /// 将类型字符串转换为Type对象
        /// </summary>
        public static Type Parse(string typeString)
        {
            if (string.IsNullOrWhiteSpace(typeString))
                throw new ArgumentNullException(nameof(typeString));

            // 处理泛型类型
            if (typeString.Contains('<'))
            {
                int genericStart = typeString.IndexOf('<');
                int genericEnd = typeString.LastIndexOf('>');

                // 提取泛型类型名称（如"Dictionary"）
                string baseTypeName = typeString.Substring(0, genericStart).Trim();
                // 提取泛型参数（如"int,Copy"）
                string genericParams = typeString.Substring(genericStart + 1, genericEnd - genericStart - 1);

                // 获取泛型类型定义（如Dictionary<>）
                _containerAliasMap.TryGetValue(baseTypeName, out Type genericTypeDef);
                if (genericTypeDef == null || !genericTypeDef.IsGenericTypeDefinition)
                    throw new InvalidOperationException($"找不到泛型类型: {baseTypeName}");

                // 解析泛型参数类型
                Type[] paramTypes = SplitGenericParameters(genericParams)
                    .Select(Parse) // 递归解析每个参数
                    .ToArray();

                // 构建具体泛型类型
                return genericTypeDef.MakeGenericType(paramTypes);
            }
            // 处理非泛型类型
            else
            {
                return GetBaseType(typeString)
                       ?? throw new TypeLoadException($"找不到类型: {typeString}");
            }
        }

        /// <summary>
        /// 分割泛型参数（支持嵌套泛型）
        /// </summary>
        private static IEnumerable<string> SplitGenericParameters(string paramString)
        {
            List<string> parameters = new List<string>();
            int depth = 0;
            int start = 0;

            for (int i = 0; i < paramString.Length; i++)
            {
                char c = paramString[i];
                if (c == '<') depth++;
                else if (c == '>') depth--;
                else if (c == ',' && depth == 0)
                {
                    parameters.Add(paramString.Substring(start, i - start).Trim());
                    start = i + 1;
                }
            }

            // 添加最后一个参数
            parameters.Add(paramString.Substring(start).Trim());
            return parameters;
        }

        /// <summary>
        /// 获取基础类型（系统类型或自定义类型）
        /// </summary>
        private static Type GetBaseType(string typeName)
        {
            // 检查系统类型别名
            if (_typeAliasMap.TryGetValue(typeName, out Type type))
                return type;

            // 尝试直接获取类型（支持带命名空间的类型）
            type = Type.GetType(typeName);
            if (type != null)
                return type;
            
            // 尝试直接获取类型（支持带命名空间的类型）
            type = Type.GetType($"Start.{typeName}");
            if (type != null)
                return type;

            // 搜索所有已加载的程序集
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                type = assembly.GetType(typeName);
                if (type != null)
                    return type;
                type = assembly.GetType($"Start.{typeName}");
                if (type != null)
                    return type;
            }

            return null;
        }
    }
}
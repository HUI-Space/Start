using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Start
{
    public static class GenericTypeNameHelper
    {
        /// <summary>
        /// 获取泛型容器的代码编写名称（如 List<string>、Dictionary<int, string>）
        /// </summary>
        public static string GetFriendlyName(Type type)
        {
            // 处理可空类型（如 int? → Nullable<int>）
            if (type.IsNullableType())
            {
                Type underlyingType = Nullable.GetUnderlyingType(type);
                return $"{GetFriendlyName(underlyingType)}?";
            }

            // 非泛型类型直接返回名称
            if (!type.IsGenericType)
            {
                // 处理数组（如 int[] → int[]）
                if (type.IsArray)
                {
                    return $"{GetFriendlyName(type.GetElementType())}[]";
                }
                return type.Name;
            }

            // 泛型类型：获取泛型定义（如 List<>）和参数（如 string）
            string genericTypeName = type.GetGenericTypeDefinition().Name;
            // 移除泛型标记（如 List`1 → List，Dictionary`2 → Dictionary）
            genericTypeName = genericTypeName.Substring(0, genericTypeName.IndexOf('`'));

            // 递归处理泛型参数（支持嵌套泛型，如 Dictionary<string, List<int>>）
            Type[] genericArguments = type.GetGenericArguments();
            string[] genericArgumentNames = Array.ConvertAll(genericArguments, GetFriendlyName);

            // 拼接泛型名称（如 List<string>、Dictionary<int, string>）
            return $"{genericTypeName}<{string.Join(", ", genericArgumentNames)}>";
        }

        
        /// <summary>
        /// 判断类型是否为泛型容器（如 List<T>、Dictionary<TKey, TValue> 等）
        /// </summary>
        /// <param name="type">待判断的类型</param>
        /// <returns>true 表示是泛型容器，false 否则</returns>
        public static bool IsGenericContainer(Type type)
        {
            if (type == null)
                return false;

            // 1. 必须是泛型类型（排除非泛型容器，如 ArrayList）
            if (!type.IsGenericType)
                return false;

            // 2. 排除泛型接口本身（如 IEnumerable<T> 不是容器，而是接口）
            if (type.IsInterface)
                return false;

            // 3. 排除特殊泛型类型（如 Nullable<T>、Task<T> 等非容器类型）
            Type genericTypeDef = type.GetGenericTypeDefinition();
            if (genericTypeDef == typeof(Nullable<>) || genericTypeDef == typeof(Task<>))
                return false;

            // 4. 判断是否实现了泛型集合接口（如 IEnumerable<T>）
            // 检查所有实现的接口，看是否有泛型集合接口
            foreach (Type interfaceType in type.GetInterfaces())
            {
                if (interfaceType.IsGenericType)
                {
                    Type interfaceDef = interfaceType.GetGenericTypeDefinition();
                    // 匹配常见的泛型容器接口
                    if (interfaceDef == typeof(IEnumerable<>) 
                        || interfaceDef == typeof(ICollection<>) 
                        || interfaceDef == typeof(IList<>))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        
        /// <summary>
        /// 扩展方法：判断类型是否为可空值类型（如 int?）
        /// </summary>
        private static bool IsNullableType(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }
    }
}
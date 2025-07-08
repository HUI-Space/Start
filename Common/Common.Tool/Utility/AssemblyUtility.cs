using System;
using System.Collections.Generic;

namespace Start
{
    /// <summary>
    /// 程序集相关的实用函数。
    /// </summary>
    public static class AssemblyUtility
    {
        private static readonly System.Reflection.Assembly[] _assemblies = null;

        private static readonly Dictionary<string, Type> _cachedTypes =
            new Dictionary<string, Type>(StringComparer.Ordinal);

        static AssemblyUtility()
        {
            _assemblies = AppDomain.CurrentDomain.GetAssemblies();
        }

        /// <summary>
        /// 获取已加载的程序集。
        /// </summary>
        /// <returns>已加载的程序集。</returns>
        public static System.Reflection.Assembly[] GetAssemblies()
        {
            return _assemblies;
        }

        /// <summary>
        /// 获取已加载的程序集中的所有类型。
        /// </summary>
        /// <returns>已加载的程序集中的所有类型。</returns>
        public static Type[] GetTypes()
        {
            List<Type> results = new List<Type>();
            foreach (System.Reflection.Assembly assembly in _assemblies)
            {
                results.AddRange(assembly.GetTypes());
            }

            return results.ToArray();
        }

        /// <summary>
        /// 获取已加载的程序集中的所有类型。
        /// </summary>
        /// <param name="results">已加载的程序集中的所有类型。</param>
        public static void GetTypes(List<Type> results)
        {
            if (results == null)
            {
                throw new Exception("Results is invalid.");
            }

            results.Clear();
            foreach (System.Reflection.Assembly assembly in _assemblies)
            {
                results.AddRange(assembly.GetTypes());
            }
        }

        /// <summary>
        /// 获取已加载的程序集中的指定类型。
        /// </summary>
        /// <param name="typeName">要获取的类型名。</param>
        /// <returns>已加载的程序集中的指定类型。</returns>
        public static Type GetType(string typeName)
        {
            if (string.IsNullOrEmpty(typeName))
            {
                throw new Exception("Type name is invalid.");
            }

            Type type = null;
            if (_cachedTypes.TryGetValue(typeName, out type))
            {
                return type;
            }

            type = Type.GetType(typeName);
            if (type != null)
            {
                _cachedTypes.Add(typeName, type);
                return type;
            }

            foreach (System.Reflection.Assembly assembly in _assemblies)
            {
                type = Type.GetType($"{typeName}, {assembly.FullName}");
                if (type != null)
                {
                    _cachedTypes.Add(typeName, type);
                    return type;
                }
            }

            return null;
        }

        /// <summary>
        /// 获取指定类型的子类型列表
        /// </summary>
        /// <param name="type">要检查的类型</param>
        /// <returns>子类型列表</returns>
        public static List<Type> GetChildType(Type type)
        {
            // 初始化一个类型列表，用于存储找到的子类型
            List<Type> implementingTypes = new List<Type>();
            // 获取所有类型
            Type[] types = GetTypes();
            // 遍历所有类型，寻找继承自指定类型的子类型
            foreach (Type t in types)
            {
                // 检查当前类型是否继承自指定类型，并且不是指定类型本身
                if (type.IsAssignableFrom(t) && type != t)
                {
                    // 如果是，则添加到子类型列表中
                    implementingTypes.Add(t);
                }
            }
        
            // 返回子类型列表
            return implementingTypes;
        }
    }
}
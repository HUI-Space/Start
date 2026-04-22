using System;
using System.Collections.Generic;
using System.Reflection;

namespace Start
{
    /// <summary>
    /// 程序集相关的实用函数。
    /// 支持动态加载程序集，自动更新缓存。
    /// </summary>
    public static class AssemblyUtility
    {
        private static readonly List<Assembly> _assemblies = new List<Assembly>();
        private static readonly Dictionary<string, Type> _cachedTypes = new Dictionary<string, Type>(StringComparer.Ordinal);

        static AssemblyUtility()
        {
            _assemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies());
            AppDomain.CurrentDomain.AssemblyLoad += OnAssemblyLoaded;
        }

        private static void OnAssemblyLoaded(object sender, AssemblyLoadEventArgs args)
        {
            if (!_assemblies.Contains(args.LoadedAssembly))
            {
                _assemblies.Add(args.LoadedAssembly);
                _cachedTypes.Clear();
            }
        }

        /// <summary>
        /// 手动刷新程序集列表
        /// </summary>
        public static void RefreshAssemblies()
        {
            _assemblies.Clear();
            _assemblies.AddRange(AppDomain.CurrentDomain.GetAssemblies());
            _cachedTypes.Clear();
        }

        /// <summary>
        /// 获取已加载的程序集。
        /// </summary>
        /// <returns>已加载的程序集。</returns>
        public static Assembly[] GetAssemblies()
        {
            return _assemblies.ToArray();
        }

        /// <summary>
        /// 获取已加载的程序集中的所有类型。
        /// </summary>
        /// <returns>已加载的程序集中的所有类型。</returns>
        public static Type[] GetTypes()
        {
            List<Type> results = new List<Type>();
            GetTypes(results);
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
                throw new ArgumentNullException(nameof(results), "Results is invalid.");
            }

            results.Clear();
            foreach (Assembly assembly in _assemblies)
            {
                try
                {
                    results.AddRange(assembly.GetTypes());
                }
                catch (ReflectionTypeLoadException ex)
                {
                    foreach (Type type in ex.Types)
                    {
                        if (type != null)
                        {
                            results.Add(type);
                        }
                    }
                }
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
                throw new ArgumentException("Type name is invalid.", nameof(typeName));
            }

            if (_cachedTypes.TryGetValue(typeName, out Type cachedType))
            {
                return cachedType;
            }

            Type type = Type.GetType(typeName);
            if (type != null)
            {
                _cachedTypes[typeName] = type;
                return type;
            }

            foreach (Assembly assembly in _assemblies)
            {
                type = assembly.GetType(typeName);
                if (type != null)
                {
                    _cachedTypes[typeName] = type;
                    return type;
                }
            }

            return null;
        }

        /// <summary>
        /// 获取指定类型的子类型列表
        /// </summary>
        /// <param name="baseType">要检查的类型</param>
        /// <returns>子类型列表</returns>
        public static List<Type> GetChildTypes(Type baseType)
        {
            List<Type> implementingTypes = new List<Type>();
            Type[] types = GetTypes();

            foreach (Type type in types)
            {
                if (baseType.IsAssignableFrom(type) && baseType != type && !type.IsAbstract)
                {
                    implementingTypes.Add(type);
                }
            }

            return implementingTypes;
        }
    }
}

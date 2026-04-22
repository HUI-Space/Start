using System;
using System.Reflection;

namespace Start
{
    /// <summary>
    /// 反射工具类
    /// 提供类型、字段、属性等反射操作的辅助方法
    /// </summary>
    public static class ReflectionUtility
    {
        /// <summary>
        /// 获取指定对象中指定字段的类型名称
        /// </summary>
        /// <param name="obj">包含字段的对象实例</param>
        /// <param name="fieldName">字段名称</param>
        /// <returns>字段的类型名称</returns>
        /// <exception cref="ArgumentNullException">对象为 null 时抛出</exception>
        /// <exception cref="ArgumentException">字段名称为空或字段不存在时抛出</exception>
        public static string GetFieldTypeName(object obj, string fieldName)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "对象不能为 null");
            }

            if (string.IsNullOrEmpty(fieldName))
            {
                throw new ArgumentException("字段名称不能为空", nameof(fieldName));
            }

            Type type = obj.GetType();
            FieldInfo fieldInfo = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (fieldInfo == null)
            {
                throw new ArgumentException($"字段 {fieldName} 不存在", nameof(fieldName));
            }

            return fieldInfo.FieldType.Name;
        }

        /// <summary>
        /// 获取指定对象中指定字段的值
        /// </summary>
        /// <typeparam name="T">字段值的类型</typeparam>
        /// <param name="obj">包含字段的对象实例</param>
        /// <param name="fieldName">字段名称</param>
        /// <returns>字段的值</returns>
        public static T GetFieldValue<T>(object obj, string fieldName)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "对象不能为 null");
            }

            if (string.IsNullOrEmpty(fieldName))
            {
                throw new ArgumentException("字段名称不能为空", nameof(fieldName));
            }

            Type type = obj.GetType();
            FieldInfo fieldInfo = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (fieldInfo == null)
            {
                throw new ArgumentException($"字段 {fieldName} 不存在", nameof(fieldName));
            }

            return (T)fieldInfo.GetValue(obj);
        }

        /// <summary>
        /// 设置指定对象中指定字段的值
        /// </summary>
        /// <param name="obj">包含字段的对象实例</param>
        /// <param name="fieldName">字段名称</param>
        /// <param name="value">要设置的值</param>
        public static void SetFieldValue(object obj, string fieldName, object value)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj), "对象不能为 null");
            }

            if (string.IsNullOrEmpty(fieldName))
            {
                throw new ArgumentException("字段名称不能为空", nameof(fieldName));
            }

            Type type = obj.GetType();
            FieldInfo fieldInfo = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (fieldInfo == null)
            {
                throw new ArgumentException($"字段 {fieldName} 不存在", nameof(fieldName));
            }

            fieldInfo.SetValue(obj, value);
        }

        /// <summary>
        /// 判断类型是否实现了指定的泛型接口
        /// </summary>
        /// <param name="classType">要检查的类型</param>
        /// <param name="interfaceType">泛型接口类型（如 typeof(IList&lt;&gt;)）</param>
        /// <returns>是否实现了该泛型接口</returns>
        public static bool ImplementsGenericInterface(Type classType, Type interfaceType)
        {
            if (classType == null)
            {
                throw new ArgumentNullException(nameof(classType));
            }

            if (interfaceType == null)
            {
                throw new ArgumentNullException(nameof(interfaceType));
            }

            Type[] interfaces = classType.GetInterfaces();
            foreach (Type type in interfaces)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == interfaceType)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// 判断类型是否实现了指定的接口
        /// </summary>
        /// <param name="classType">要检查的类型</param>
        /// <param name="interfaceType">接口类型</param>
        /// <returns>是否实现了该接口</returns>
        public static bool ImplementsInterface(Type classType, Type interfaceType)
        {
            if (classType == null)
            {
                throw new ArgumentNullException(nameof(classType));
            }

            if (interfaceType == null)
            {
                throw new ArgumentNullException(nameof(interfaceType));
            }

            return interfaceType.IsAssignableFrom(classType);
        }

        /// <summary>
        /// 获取类型的所有字段（包括私有字段）
        /// </summary>
        /// <param name="type">要获取字段的类型</param>
        /// <returns>字段信息数组</returns>
        public static FieldInfo[] GetAllFields(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        }

        /// <summary>
        /// 获取类型的所有属性
        /// </summary>
        /// <param name="type">要获取属性的类型</param>
        /// <returns>属性信息数组</returns>
        public static PropertyInfo[] GetAllProperties(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }

            return type.GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
        }
    }
}

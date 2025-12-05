using System;
using System.Reflection;

namespace Start
{
    public static class ReflectionUtility
    {
        /*/// <summary>
        /// 根据别名获取类型
        /// </summary>
        /// <param name="alias">别名</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static Type GetTypeFromAlias(string alias)
        {
            // 动态生成代码：定义一个返回指定类型的方法
            string code = $@"
            using System;
            public class TypeHelper {{
                public static Type GetType() {{
                    return typeof({alias});
                }}
            }}";

            // 使用 C# 编译器动态编译
            using (CSharpCodeProvider provider = new CSharpCodeProvider())
            {
                CompilerParameters parameters = new CompilerParameters();
                parameters.GenerateInMemory = true; // 仅在内存中生成，不写入磁盘

                CompilerResults results = provider.CompileAssemblyFromSource(parameters, code);
                if (results.Errors.HasErrors)
                {
                    throw new ArgumentException($"无效的类型别名: {alias}");
                }

                // 调用动态生成的方法获取类型
                Assembly assembly = results.CompiledAssembly;
                Type helperType = assembly.GetType("TypeHelper");
                MethodInfo method = helperType.GetMethod("GetType");
                return (Type)method.Invoke(null, null);
            }
        }*/
        
        /// <summary>
        /// 获取指定对象中指定字段的类型名称
        /// </summary>
        /// <param name="obj">包含字段的对象实例</param>
        /// <param name="fieldName">字段名称</param>
        /// <returns>字段的类型名称（如int、string等），获取失败返回null</returns>
        public static string GetFieldTypeName(object obj, string fieldName)
        {
            if (obj == null)
                throw new ArgumentNullException(nameof(obj), "对象不能为null");
        
            if (string.IsNullOrEmpty(fieldName))
                throw new ArgumentException("字段名称不能为空", nameof(fieldName));

            // 获取对象的类型
            Type type = obj.GetType();
        
            // 获取指定名称的字段（包括公共和非公共实例字段）
            FieldInfo fieldInfo = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        
            if (fieldInfo == null)
            {
                throw new ArgumentException("字段 " + fieldName + " 不存在");
            }

            // 返回字段的类型名称
            return fieldInfo.FieldType.Name;
        }
        
        /// <summary>
        /// 判断类是否实现泛型接口
        /// </summary>
        /// <param name="classType">类的类型</param>
        /// <param name="interfaceType">接口的类型</param>
        /// <returns></returns>
        public static bool ImplementsGenericInterface(Type classType,Type interfaceType)
        {
            Type[] types = classType.GetInterfaces();
            foreach (Type type in types)
            {
                if (type.IsInterface && type.IsGenericType)
                {
                    Type genericTypeDefinition = type.GetGenericTypeDefinition();
                    if (genericTypeDefinition != null && genericTypeDefinition == interfaceType)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
        
    }
}
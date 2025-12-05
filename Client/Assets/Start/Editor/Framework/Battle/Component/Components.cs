using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Start.Editor
{
    public class Components
    {
        [MenuItem("Start/Generate")]
        public static void Generate()
        {
            string path = EditorUtility.SaveFolderPanel("保存组件脚本", "Assets", "");
            if (string.IsNullOrEmpty(path)) return;

            if (!path.StartsWith(Application.dataPath))
            {
                EditorUtility.DisplayDialog("错误", "请保存在项目Assets目录下！", "确定");
                return;
            }

            List<Type> types = AssemblyUtility.GetChildType(typeof(IComponent));
            StringBuilder code = new StringBuilder();
            List<FieldInfo> valueFieldInfo = new List<FieldInfo>();
            List<FieldInfo> arrayFieldInfo = new List<FieldInfo>();
            List<FieldInfo> GenericFieldInfo = new List<FieldInfo>();
            List<FieldInfo> otherFieldInfo = new List<FieldInfo>();
            foreach (Type type in types)
            {
                //获取属性
                FieldInfo[] fieldInfos = type.GetFields();
                foreach (FieldInfo fieldInfo in fieldInfos)
                {
                    if (fieldInfo.FieldType.IsValueType)
                    {
                        valueFieldInfo.Add(fieldInfo);
                    }
                    else if (fieldInfo.FieldType.IsArray)
                    {
                        arrayFieldInfo.Add(fieldInfo);
                    }
                    else if (fieldInfo.FieldType.IsGenericType)
                    {
                        GenericFieldInfo.Add(fieldInfo);
                    }
                    else
                    {
                        otherFieldInfo.Add(fieldInfo);
                    }
                }

                if (type.IsAbstract)
                {
                    continue;
                }

                code.AppendLine("/*-----------------------------------------------------------------");
                code.AppendLine("    以下代码为自动生成的代码，任何对以下代码的修改将在下次自动生成后被覆");
                code.AppendLine("    盖，不要对以下代码进行修改");
                code.AppendLine("-----------------------------------------------------------------*/");
                code.AppendLine();
                code.AppendLine("using System;");
                code.AppendLine("using System.Collections.Generic;");
                code.AppendLine("using Start;");
                code.AppendLine("namespace Start");
                code.AppendLine("{");
                var attributes = type.GetCustomAttributes(false);
                if (attributes.Length > 0)
                {
                    foreach (var attribute in attributes)
                    {
                        if (attribute is CommentAttribute commentAttribute)
                        {
                            code.AppendLine("    /// <summary>");
                            code.AppendLine("    /// 自动生成" + commentAttribute.Comment);
                            code.AppendLine("    /// </summary>");
                        }
                    }
                }

                code.AppendLine($"    public class {type.Name} : IComponent<{type.Name}>");
                code.AppendLine("    {");

                if (valueFieldInfo.Count > 0)
                {
                    code.AppendLine(
                        $"        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Auto, Pack = 4)]");
                    code.AppendLine($"        private struct Common");
                    code.AppendLine("        {");
                    foreach (FieldInfo fieldInfo in valueFieldInfo)
                    {
                        code.AppendLine($"            public {fieldInfo.FieldType.Name} {fieldInfo.Name};");
                    }

                    code.AppendLine();
                    code.AppendLine($"            public Common(int no)");
                    code.AppendLine("            {");
                    foreach (FieldInfo fieldInfo in valueFieldInfo)
                    {
                        code.AppendLine(TryGetAttribute(fieldInfo, out DefaultAttribute attribute)
                            ? $"                {fieldInfo.Name} = {attribute.DefaultValue};"
                            : $"                {fieldInfo.Name} = default;");
                    }

                    code.AppendLine("            }");
                    code.AppendLine("        }");
                    code.AppendLine();
                    code.AppendLine("        private Common _common = new Common();");
                }

                code.AppendLine();
                foreach (FieldInfo fieldInfo in valueFieldInfo)
                {
                    GenerateCommentCode(fieldInfo, code);
                    code.AppendLine($"        public {fieldInfo.FieldType.Name} {fieldInfo.Name}");
                    code.AppendLine("        {");
                    code.AppendLine($"            get => _common.{fieldInfo.Name};");
                    code.AppendLine($"            set => _common.{fieldInfo.Name} = value;");
                    code.AppendLine("        }");
                    code.AppendLine();
                }

                foreach (FieldInfo fieldInfo in arrayFieldInfo)
                {
                    GenerateCommentCode(fieldInfo, code);
                    code.AppendLine(TryGetAttribute(fieldInfo, out DefaultAttribute attribute)
                        ? $"        private {fieldInfo.FieldType.Name} {fieldInfo.Name} = {attribute.DefaultValue};"
                        : $"        private {fieldInfo.FieldType.Name} {fieldInfo.Name} {{ get; set; }}");
                    code.AppendLine();
                }


                foreach (FieldInfo fieldInfo in GenericFieldInfo)
                {
                    Type genericDef = fieldInfo.FieldType.GetGenericTypeDefinition();
                    // 处理 List<T>                   处理 HashSet<T>
                    if (genericDef == typeof(List<>) || genericDef == typeof(HashSet<>))
                    {
                        Type[] genericArgs = fieldInfo.FieldType.GetGenericArguments();
                        if (GenericTypeNameHelper.IsGenericContainer(genericArgs[0]) || genericArgs[0].IsArray)
                        {
                            continue;
                        }
                    }
                    else if (genericDef == typeof(Dictionary<,>))
                    { 
                        Type[] genericArgs = fieldInfo.FieldType.GetGenericArguments();
                        if (GenericTypeNameHelper.IsGenericContainer(genericArgs[0]) || genericArgs[0].IsArray || GenericTypeNameHelper.IsGenericContainer(genericArgs[1]) || genericArgs[1].IsArray)
                        {
                            continue;
                        }
                    }
                    string typeName = GenericTypeNameHelper.GetFriendlyName(fieldInfo.FieldType);
                    GenerateCommentCode(fieldInfo, code);
                    code.AppendLine(TryGetAttribute(fieldInfo, out DefaultAttribute attribute)
                        ? $"        private {typeName} {fieldInfo.Name} = {attribute.DefaultValue};"
                        : $"        public {typeName} {fieldInfo.Name} = new {typeName}();");
                    code.AppendLine();
                }

                foreach (FieldInfo fieldInfo in otherFieldInfo)
                {
                    GenerateCommentCode(fieldInfo, code);
                    string typeName = GenericTypeNameHelper.GetFriendlyName(fieldInfo.FieldType);
                    code.AppendLine(TryGetAttribute(fieldInfo, out DefaultAttribute attribute)
                        ? $"        private {fieldInfo.FieldType.Name} {fieldInfo.Name} = {attribute.DefaultValue};"
                        : $"        public {fieldInfo.FieldType.Name} {fieldInfo.Name} = new {typeName}();");

                    code.AppendLine();
                }

                code.AppendLine($"        public void CopyTo({type.Name} component)");
                code.AppendLine("        {");
                if (valueFieldInfo.Count > 0)
                {
                    code.AppendLine($"            component._common = _common;");
                }

                if (arrayFieldInfo.Count > 0)
                {
                    foreach (FieldInfo fieldInfo in arrayFieldInfo)
                    {
                        code.AppendLine(
                            $"            Array.Copy({fieldInfo.Name}, component.{fieldInfo.Name}, {fieldInfo.Name}.Length);");
                    }
                }

                if (GenericFieldInfo.Count > 0)
                {
                    foreach (FieldInfo fieldInfo in GenericFieldInfo)
                    {
                        string generateCode = GenerateCodeByType(fieldInfo.FieldType, fieldInfo.Name,
                            $"component.{fieldInfo.Name}", "            ");
                        if (string.IsNullOrEmpty(generateCode))
                        {
                            continue;
                        }
                        code.AppendLine(            generateCode);
                    }
                }

                foreach (FieldInfo fieldInfo in otherFieldInfo)
                {
                    if (ReflectionUtility.ImplementsGenericInterface(fieldInfo.FieldType, typeof(IComponent<>)))
                    {
                        code.AppendLine($"            {fieldInfo.Name}.CopyTo(component.{fieldInfo.Name});");
                    }
                    else
                    {
                        code.AppendLine($"            component.{fieldInfo.Name} = {fieldInfo.Name};");
                    }
                }
                code.AppendLine("        }");
                code.AppendLine();
                code.AppendLine("        public void Reset()");
                code.AppendLine("        {");
                code.AppendLine("        }");
                
                code.AppendLine("    }");
                code.AppendLine("}");
                File.WriteAllText(Path.Combine(path, type.Name + ".cs"), code.ToString());

                valueFieldInfo.Clear();
                arrayFieldInfo.Clear();
                GenericFieldInfo.Clear();
                otherFieldInfo.Clear();
                code.Clear();
            }

            AssetDatabase.Refresh();
        }
        
        /// <summary>
        /// 生成注释代码
        /// </summary>
        /// <param name="fieldInfo"></param>
        /// <param name="code"></param>
        private static void GenerateCommentCode(FieldInfo fieldInfo, StringBuilder code)
        {
            if (TryGetAttribute(fieldInfo,out CommentAttribute attribute) && !string.IsNullOrEmpty(attribute.Comment))
            {
                code.AppendLine("        /// <summary>");
                code.AppendLine("        /// " + attribute.Comment);
                code.AppendLine("        /// </summary>");
            }
        }
        
        
        /// <summary>
        /// 尝试获取属性
        /// </summary>
        /// <param name="fieldInfo">字段</param>
        /// <param name="attribute">属性</param>
        /// <typeparam name="T">属性类型</typeparam>
        /// <returns></returns>
        private static bool TryGetAttribute<T>(FieldInfo fieldInfo,out T attribute) where T : Attribute
        {
            attribute = fieldInfo.FieldType.GetCustomAttribute<T>();
            return attribute != null;
        }
        
        /// <summary>
        /// 生成拷贝代码（字符串）拒绝嵌套
        /// </summary>
        /// <param name="containerType"></param>
        /// <param name="sourceVarName"></param>
        /// <param name="targetVarName"></param>
        /// <param name="space"></param>
        /// <returns></returns>
        private static string GenerateCodeByType(Type containerType,string sourceVarName,string targetVarName,string space)
        {
            StringBuilder code = new StringBuilder();
            if (containerType.IsGenericType)
            {
                Type genericDef = containerType.GetGenericTypeDefinition();
                Type[] genericArgs = containerType.GetGenericArguments();
                // 处理 List<T>                   
                if (genericDef == typeof(List<>))
                {
                    if (GenericTypeNameHelper.IsGenericContainer(genericArgs[0]))
                    {
                        EditorUtility.DisplayDialog("错误", $"请勿嵌套泛型容器{GenericTypeNameHelper.GetFriendlyName(genericDef)}", "确定");
                        return string.Empty;
                    }

                    if (ReflectionUtility.ImplementsGenericInterface(genericArgs[0],typeof(IComponent<>)))
                    {
                        string elementType = GenericTypeNameHelper.GetFriendlyName(genericArgs[0]);
                        code.AppendLine($"{space}component.{sourceVarName}.Capacity = component.{sourceVarName}.Capacity > {sourceVarName}.Capacity ? component.{sourceVarName}.Capacity : {sourceVarName}.Capacity;");
                        code.AppendLine($"{space}for (int i = 0; i < {sourceVarName}.Count; i++)");
                        code.AppendLine($"{space}{{");
                        code.AppendLine($"{space}   {elementType} item = i < component.{sourceVarName}.Count ? component.{sourceVarName}[i] : RecyclableObjectPool.Acquire<{elementType}>();");
                        code.AppendLine($"{space}   {sourceVarName}[i].CopyTo(item);");
                        code.AppendLine($"{space}   component.{sourceVarName}.Add(item);");
                        code.AppendLine($"{space}}}");
                    }
                    else
                    {
                        code.AppendLine($"{space}component.{sourceVarName}.Clear();");
                        code.AppendLine($"{space}component.{sourceVarName}.AddRange(Data);");
                    }
                    
                }
                else if (genericDef == typeof(Dictionary<,>))
                { 
                    if (GenericTypeNameHelper.IsGenericContainer(genericArgs[0]) ||  GenericTypeNameHelper.IsGenericContainer(genericArgs[1]))
                    {
                        EditorUtility.DisplayDialog("错误", $"请勿嵌套泛型容器{GenericTypeNameHelper.GetFriendlyName(genericDef)}", "确定");
                        return  string.Empty;
                    }
                    string keyType = GenericTypeNameHelper.GetFriendlyName(genericArgs[0]);
                    string valueType = GenericTypeNameHelper.GetFriendlyName(genericArgs[1]);

                    if (!genericArgs[0].IsValueType || !genericArgs[1].IsEnum)
                    {
                        EditorUtility.DisplayDialog("错误", $"Dictionary 的 Key 必须为值类型或者枚举类型", "确定");
                        return string.Empty;
                        /*if (ReflectionUtility.ImplementsGenericInterface(genericArgs[1],typeof(IComponent<>)))
                        {
                            code.AppendLine($"{space}foreach (KeyValuePair<{keyType},{valueType}> item in {sourceVarName})");
                            code.AppendLine($"{space}{{");
                            code.AppendLine($"{space}    {keyType} key = RecyclableObjectPool.Acquire<{keyType}>();");
                            code.AppendLine($"{space}    item.Key.CopyTo(key);");
                            code.AppendLine($"{space}    {valueType} value = RecyclableObjectPool.Acquire<{valueType}>();");
                            code.AppendLine($"{space}    item.Value.CopyTo(value);");
                            code.AppendLine($"{space}    component.{sourceVarName}.Add(key, value);");
                            code.AppendLine($"{space}}}");
                        }
                        else
                        {
                            code.AppendLine($"{space}foreach (KeyValuePair<{keyType},{valueType}> item in {sourceVarName})");
                            code.AppendLine($"{space}{{");
                            code.AppendLine($"{space}    {keyType} key = RecyclableObjectPool.Acquire<{keyType}>();");
                            code.AppendLine($"{space}    item.Key.CopyTo(key);");
                            code.AppendLine($"{space}    component.{sourceVarName}.Add(key, item.Value);");
                            code.AppendLine($"{space}}}");
                        }*/
                    }
                    if (ReflectionUtility.ImplementsGenericInterface(genericArgs[1],typeof(IComponent<>)))
                    {
                        code.AppendLine($"{space}foreach (KeyValuePair<{keyType},{valueType}> item in {sourceVarName})");
                        code.AppendLine($"{space}{{");
                        code.AppendLine($"{space}    {valueType} value = RecyclableObjectPool.Acquire<{valueType}>();");
                        code.AppendLine($"{space}    item.Value.CopyTo(value);");
                        code.AppendLine($"{space}    component.{sourceVarName}.Add(item.Key, value);");
                        code.AppendLine($"{space}}}");
                    }
                    else
                    {
                        code.AppendLine($"{space}foreach (KeyValuePair<{keyType},{valueType}> item in {sourceVarName})");
                        code.AppendLine($"{space}{{");
                        code.AppendLine($"{space}    {targetVarName}.Add(item.Key, item.Value);");
                        code.AppendLine($"{space}}}");
                    }
                }
            }
            return code.ToString();
        }
        
    }

    public interface IComponent
    {
        
    }
    
    [Comment("坐标组件")]
    public class TransformComponent : IComponent
    {
        [Comment("坐标")] public TSVector3 Position;
        [Comment("旋转")] public TSVector4 Rotation;
        [Comment("缩放")] public TSVector3 Scale;
        [Comment("前向向量")] public TSVector3 Forward;
        [Comment("偏移量")] public TSVector3 MoveDelta;
    }


    [Comment("移动组件")]
    public class MoveComponent : IComponent
    {
        [Comment("移动方向")] public TSVector3 MoveDirection;
        [Comment("移动速度")] public FP MoveSpeed;

        [Comment("上一次移动方向")] public TSVector3 LastMoveDirection;
        [Comment("上一次移动速度")] public FP LastMoveSpeed;
    }

    [Comment("输入组件")]
    public class InputComponent : IComponent
    {
        [Comment("玩家输入")] public int Yaw;
    }

    [Comment("状态组件")]
    public class StateComponent : IComponent
    {
        [Comment("当前状态")] public int CurrentState;
        [Comment("下一状态")] public int NextState;
        [Comment("上一状态")] public int PrevState;
        [Comment("强制状态")] public int ForceStateId;
        [Comment("状态开始时间")] public FP EnterTime;
        [Comment("状态结束时间")] public FP EndTime;
    }
}
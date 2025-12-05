using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Start.Editor
{
    /// <summary>
    /// TODO 生成代码后续再修改吧
    /// 将就用着吧
    /// </summary>
    public class BattleEditorWindow : EditorWindow
    {
        private static BattleEditorWindow _window;

        public static void OpenEditorWindow()
        {
            _window = (BattleEditorWindow)GetWindow(typeof(BattleEditorWindow), false, "战斗编辑器");
            _window.minSize = new Vector2(1000f, 600f);
            _window.Show();
        }


        private string _newClassName;
        
        private int _changeIndex = -1;
        
        /// <summary>
        /// 标记要删除的类索引
        /// </summary>
        private int _classToRemove = -1;

        /// <summary>
        /// 标记要删除的属性索引
        /// </summary>
        private (int classIndex, int propIndex) _propToRemove = (-1, -1);

        /// <summary>
        /// 滚动位置
        /// </summary>
        private Vector2 _scrollPosition;

        /// <summary>
        /// 动态类配置
        /// </summary>
        private BattleComponentConfig _battleComponentConfig;

        /// <summary>
        /// 类的特性列表
        /// </summary>
        private readonly List<string> _componentAttributes = new List<string>()
        {
            "SerializeComponent",
        };

        /// <summary>
        /// 属性特性列表
        /// </summary>
        private readonly List<string> _propertyAttributes = new List<string>()
        {
            "NoneSerializeFiled",
            "ReadOnlyField",
            "DirtyCheck",
            "NoDirtyCheck",
        };

        private void OnEnable()
        {
            _battleComponentConfig = File.Exists(BattlePath.BattleComponentConfigPath)
                ? Utility.LoadJsonConfig<BattleComponentConfig>(BattlePath.BattleComponentConfigPath)
                : new BattleComponentConfig();
        }

        private void OnGUI()
        {
            ProcessDeletions();
            EditorGUILayout.LabelField("Battles Editor");

            // 新建类输入区域
            EditorGUILayout.BeginVertical("box");
            {
                EditorGUILayout.BeginHorizontal();
                {
                    _newClassName = EditorGUILayout.TextField("类名:", _newClassName);
                    if (GUILayout.Button("创建", GUILayout.Width(80)) && !string.IsNullOrEmpty(_newClassName))
                    {
                        CreateNewClass();
                    }
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndVertical();


            EditorGUILayout.Space();
            _scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);


            for (int i = 0; i < _battleComponentConfig.DataList.Count; i++)
            {
                BattleComponentClass battleComponentClass = _battleComponentConfig.DataList[i];

                if (_changeIndex == i)
                {
                    ShowComponentProperty(battleComponentClass, i);
                }
                else
                {
                    UpdateComponentProperty(battleComponentClass, i);
                }
            }

            EditorGUILayout.EndScrollView();
            if (GUILayout.Button("保存配置", GUILayout.Height(30)))
            {
                SaveConfig();
            }

            // 保存按钮
            if (GUILayout.Button("生成脚本", GUILayout.Height(30)))
            {
                SaveToScript();
            }
        }

        private void UpdateComponentProperty(BattleComponentClass battleComponentClass, int i)
        {
            EditorGUILayout.BeginVertical("box");
            {
                // 类名和删除按钮
                EditorGUILayout.BeginVertical("box");
                {
                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("类的特性:", GUILayout.Width(148));
                        string content = string.Join(",", battleComponentClass.ClassAttributes);
                        if (EditorGUILayout.DropdownButton(new GUIContent(content), FocusType.Keyboard))
                        {
                            GenericMenu menu = new GenericMenu();

                            // 添加所有选项到菜单
                            for (int k = 0; k < _componentAttributes.Count; k++)
                            {
                                int index = k; // 捕获当前索引
                                bool isSelected =
                                    battleComponentClass.ClassAttributes.Contains(_componentAttributes[index]);
                                menu.AddItem(new GUIContent(_componentAttributes[index]), isSelected, () =>
                                {
                                    if (battleComponentClass.ClassAttributes.Contains(_componentAttributes[index]))
                                    {
                                        battleComponentClass.ClassAttributes.Remove(_componentAttributes[index]);
                                    }
                                    else
                                    {
                                        battleComponentClass.ClassAttributes.Add(_componentAttributes[index]);
                                    }
                                });
                            }

                            menu.ShowAsContext();
                        }

                        if (GUILayout.Button("删除类", GUILayout.Width(80)))
                        {
                            if (EditorUtility.DisplayDialog("确认", $"确定要删除 {battleComponentClass.ClassName} 吗？", "是",
                                    "否"))
                            {
                                _classToRemove = i; // 记录要删除的索引，延迟到布局结束后处理
                            }
                        }
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    {
                        battleComponentClass.ClassComment =
                            EditorGUILayout.TextField("类注释:", battleComponentClass.ClassComment);
                    }
                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.BeginHorizontal();
                    {
                        EditorGUILayout.LabelField("类名:", battleComponentClass.ClassName);
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();

                EditorGUILayout.BeginVertical("box");
                {
                    for (int j = 0; j < _battleComponentConfig.DataList[i].DynamicProperties.Count; j++)
                    {
                        // 属性名
                        EditorGUILayout.Space();

                        BattleComponentProperty battleComponentProperty =
                            _battleComponentConfig.DataList[i].DynamicProperties[j];

                        // 绘制多选下拉框
                        EditorGUILayout.BeginHorizontal();
                        {
                            EditorGUILayout.LabelField("属性特性:", GUILayout.Width(148));
                            string content = string.Join(",", battleComponentProperty.PropertyAttributes);
                            if (EditorGUILayout.DropdownButton(new GUIContent(content), FocusType.Keyboard))
                            {
                                GenericMenu menu = new GenericMenu();

                                // 添加所有选项到菜单
                                for (int k = 0; k < _propertyAttributes.Count; k++)
                                {
                                    int index = k; // 捕获当前索引
                                    bool isSelected =
                                        battleComponentProperty.PropertyAttributes.Contains(
                                            _propertyAttributes[index]);
                                    menu.AddItem(new GUIContent(_propertyAttributes[index]), isSelected, () =>
                                    {
                                        if (battleComponentProperty.PropertyAttributes.Contains(
                                                _propertyAttributes[index]))
                                        {
                                            battleComponentProperty.PropertyAttributes.Remove(
                                                _propertyAttributes[index]);
                                        }
                                        else
                                        {
                                            battleComponentProperty.PropertyAttributes.Add(
                                                _propertyAttributes[index]);
                                        }
                                    });
                                }

                                menu.ShowAsContext();
                            }

                            // 删除属性按钮
                            if (GUILayout.Button("删除属性", GUILayout.Width(80)))
                            {
                                _propToRemove = (i, j); // 记录要删除的索引，延迟处理
                            }
                        }
                        EditorGUILayout.EndHorizontal();

                        // 属性注释
                        battleComponentProperty.PropertyComment =
                            EditorGUILayout.TextField("属性注释:", battleComponentProperty.PropertyComment);

                        EditorGUILayout.BeginHorizontal();
                        // 属性类型
                        battleComponentProperty.PropertyType =
                            EditorGUILayout.TextField("属性类型:", battleComponentProperty.PropertyType);
                        // 删除属性按钮
                        if (GUILayout.Button("设置属性", GUILayout.Width(80)))
                        {
                            TTypeEditorWindow.ShowWindow(
                                new[]
                                {
                                    typeof(int),
                                    typeof(FP),
                                    typeof(TSVector2),
                                    typeof(Array),
                                    typeof(Array[,]),
                                    typeof(List<>),
                                    typeof(Dictionary<,>)
                                }, (value) =>
                                {
                                    battleComponentProperty.PropertyType = value.ToString();
                                    battleComponentProperty.PropertyDefaultValue = string.Empty;
                                });
                        }

                        EditorGUILayout.EndHorizontal();

                        // 属性名称
                        battleComponentProperty.PropertyName =
                            EditorGUILayout.TextField("属性名称:", battleComponentProperty.PropertyName);

                        // 属性类型
                        battleComponentProperty.PropertyDefaultValue =
                            EditorGUILayout.TextField("属性默认值:", battleComponentProperty.PropertyDefaultValue);
                    }

                    EditorGUILayout.BeginHorizontal();
                    {
                        // 显示属性列表
                        //EditorGUILayout.LabelField("属性:");
                        if (GUILayout.Button("添加属性"))
                        {
                            _battleComponentConfig.DataList[i].DynamicProperties.Add(new BattleComponentProperty
                            {
                                PropertyName = "NewProperty",
                                PropertyType = "NewType",
                                PropertyDefaultValue = "",
                                PropertyComment = "",
                                PropertyAttributes = new List<string>(),
                            });
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndVertical();
        }

        private void ShowComponentProperty(BattleComponentClass battleComponentClass, int i)
        {
            EditorGUILayout.BeginVertical("box");
            {
                for (int j = 0; j < _battleComponentConfig.DataList[i].DynamicProperties.Count; j++)
                {
                    
                }
            }
            EditorGUILayout.EndVertical();
        }
        
        private void CreateNewClass()
        {
            if (_battleComponentConfig.DataList.Exists(c => c.ClassName == _newClassName))
            {
                EditorUtility.DisplayDialog("错误", "该类名已存在！", "确定");
                return;
            }

            if (!IsValidClassName(_newClassName))
            {
                EditorUtility.DisplayDialog("错误", "类名包含无效字符！", "确定");
                return;
            }

            _battleComponentConfig.DataList.Add(new BattleComponentClass { ClassName = _newClassName });
            _newClassName = "";
        }

        private bool IsValidClassName(string className)
        {
            if (string.IsNullOrEmpty(className)) return false;
            if (!char.IsLetter(className[0]) && className[0] != '_') return false;

            for (int i = 1; i < className.Length; i++)
            {
                if (!char.IsLetterOrDigit(className[i]) && className[i] != '_')
                    return false;
            }

            return true;
        }

        // 处理延迟删除，在布局完成后执行
        private void ProcessDeletions()
        {
            if (_classToRemove != -1)
            {
                _battleComponentConfig.DataList.RemoveAt(_classToRemove);
                _classToRemove = -1;
            }

            if (_propToRemove.classIndex != -1 && _propToRemove.propIndex != -1)
            {
                _battleComponentConfig.DataList[_propToRemove.classIndex].DynamicProperties
                    .RemoveAt(_propToRemove.propIndex);
                _propToRemove = (-1, -1);
            }
        }

        private void SaveConfig()
        {
            string json = JsonUtility.ToJson(_battleComponentConfig, true);
            FileUtility.WriteAllText(BattlePath.BattleComponentConfigPath, json);
            AssetDatabase.Refresh();
        }

        private void SaveToScript()
        {
            if (_battleComponentConfig.DataList.Count == 0)
            {
                EditorUtility.DisplayDialog("提示", "没有可保存的类！", "确定");
                return;
            }

            string path = EditorUtility.SaveFolderPanel("保存组件脚本", "Assets", "");

            if (string.IsNullOrEmpty(path)) return;

            if (!path.StartsWith(Application.dataPath))
            {
                EditorUtility.DisplayDialog("错误", "请保存在项目Assets目录下！", "确定");
                return;
            }

            StringBuilder code = new StringBuilder();

            List<BattleComponentProperty> classTypeProperties = new List<BattleComponentProperty>();
            List<BattleComponentProperty> valueTypeProperties = new List<BattleComponentProperty>();
            List<BattleComponentProperty> arrayTypeProperties = new List<BattleComponentProperty>();
            List<BattleComponentProperty> genericTypeProperties = new List<BattleComponentProperty>();
            List<BattleComponentProperty> copyValueTypeProperties = new List<BattleComponentProperty>();
            
            foreach (BattleComponentClass component in _battleComponentConfig.DataList)
            {
                foreach (BattleComponentProperty battleComponentProperty in component.DynamicProperties)
                {
                    if (TypeFactory.TryGetType(battleComponentProperty.PropertyType, out TType tType))
                    {
                        if (tType is TGenericType tGenericType)
                        {
                            if (IsNestedPropertyName(tGenericType))
                            {
                                EditorUtility.DisplayDialog("提示", $"类名：{component.ClassName} \n属性：{battleComponentProperty.PropertyName} \n类型: {tGenericType} \n不支持嵌套！", "确定");
                                return;
                            }
                            if (tGenericType is TArray)
                            {
                                arrayTypeProperties.Add(battleComponentProperty);
                            }
                            else
                            {
                                genericTypeProperties.Add(battleComponentProperty);
                            }
                        }
                        else if (tType is TDictionary tDictionary)
                        {
                            if (IsNestedPropertyName(tDictionary))
                            {
                                EditorUtility.DisplayDialog("提示", $"类名：{component.ClassName} \n属性：{battleComponentProperty.PropertyName} \n类型：{tDictionary} \n不支持嵌套！", "确定");
                                return;
                            }
                            genericTypeProperties.Add(battleComponentProperty);
                        }
                        else
                        {
                            valueTypeProperties.Add(battleComponentProperty);
                        }
                    }
                    else
                    {
                        Type type = TypeParser.Parse(battleComponentProperty.PropertyType);
                        
                        if (type == null)
                        {
                            Debug.LogError($"类型{battleComponentProperty.PropertyType}未找到！");
                        }
                        
                        if (type != null)
                        {
                            if (type.GetInterfaces().Any(i => i.IsGenericType && 
                                                              i.GetGenericTypeDefinition() == typeof(IComponent<>)))
                            {
                                copyValueTypeProperties.Add(battleComponentProperty);
                            }
                            else if (type.IsGenericType)
                            {
                                genericTypeProperties.Add(battleComponentProperty);
                            }
                            else if (type.IsArray)
                            {
                                arrayTypeProperties.Add(battleComponentProperty);
                            }
                            else if (type.IsValueType)
                            {
                                valueTypeProperties.Add(battleComponentProperty);
                            }
                            else
                            {
                                classTypeProperties.Add(battleComponentProperty);
                            }
                        }
                    }
                }
                
                
                code.AppendLine("using System;");
                code.AppendLine("using System.Collections.Generic;");
                code.AppendLine();
                code.AppendLine("namespace Start");
                code.AppendLine("{");

                if (!string.IsNullOrEmpty(component.ClassComment))
                {
                    code.AppendLine($"    /// <summary>");
                    code.AppendLine($"    /// 动态生成的组件类: {component.ClassComment}");
                    code.AppendLine($"    /// </summary>");
                }
                
                code.AppendLine($"    public class {component.ClassName} : IComponent<{component.ClassName}>");
                code.AppendLine("    {");
                
                if (valueTypeProperties.Count > 0)
                {
                    code.AppendLine(
                        $"        [System.Runtime.InteropServices.StructLayout(System.Runtime.InteropServices.LayoutKind.Auto, Pack = 4)]");
                    code.AppendLine($"        private struct Common");
                    code.AppendLine("        {");

                    foreach (BattleComponentProperty value in valueTypeProperties)
                    {
                        code.AppendLine($"            public {value.PropertyType} {value.PropertyName};");
                    }

                    code.AppendLine($"            public Common(int no)");
                    code.AppendLine("            {");
                    foreach (BattleComponentProperty value in valueTypeProperties)
                    {
                        code.AppendLine(!string.IsNullOrEmpty(value.PropertyDefaultValue)
                            ? $"                {value.PropertyName} = {value.PropertyDefaultValue};"
                            : $"                {value.PropertyName} = default;");
                    }

                    code.AppendLine("            }");
                    code.AppendLine("        }");

                    code.AppendLine();
                    code.AppendLine("        private Common _common = new Common();");
                    code.AppendLine();
                    foreach (BattleComponentProperty value in valueTypeProperties)
                    {
                        code.AppendLine($"        public {value.PropertyType} {value.PropertyName}");
                        code.AppendLine("        {");
                        code.AppendLine($"            get => _common.{value.PropertyName};");
                        code.AppendLine($"            set => _common.{value.PropertyName} = value;");
                        code.AppendLine("        }");
                        code.AppendLine();
                    }
                }
                
                foreach (BattleComponentProperty value in classTypeProperties)
                {
                    string lowerPropertyName = value.PropertyName.ToLower();
                    code.AppendLine($"        private {value.PropertyType} _{lowerPropertyName} = new {value.PropertyType}();");
                    code.AppendLine($"        public {value.PropertyType} {value.PropertyName}");
                    code.AppendLine("        {");
                    code.AppendLine($"            get => _{lowerPropertyName};");
                    code.AppendLine($"            set => _{lowerPropertyName} = value;");
                    code.AppendLine("        }");
                    code.AppendLine();
                }
                
                foreach (BattleComponentProperty value in copyValueTypeProperties)
                {
                    string lowerPropertyName = value.PropertyName.ToLower();
                    code.AppendLine($"        private {value.PropertyType} _{lowerPropertyName} = new {value.PropertyType}();");
                    code.AppendLine($"        public {value.PropertyType} {value.PropertyName}");
                    code.AppendLine("        {");
                    code.AppendLine($"            get => _{lowerPropertyName};");
                    code.AppendLine($"            set => _{lowerPropertyName} = value;");
                    code.AppendLine("        }");
                    code.AppendLine();
                }
                
                
                foreach (BattleComponentProperty value in arrayTypeProperties)
                {
                    string lowerPropertyName = value.PropertyName.ToLower();
                    code.AppendLine($"        private {value.PropertyType} _{lowerPropertyName} = {value.PropertyDefaultValue};");
                    code.AppendLine($"        public {value.PropertyType} {value.PropertyName}");
                    code.AppendLine("        {");
                    code.AppendLine($"            get => _{lowerPropertyName};");
                    code.AppendLine($"            set => _{lowerPropertyName} = value;");
                    code.AppendLine("        }");
                    code.AppendLine();
                }

                foreach (BattleComponentProperty value in genericTypeProperties)
                {
                    if (value.PropertyType.Contains("List") || value.PropertyType.Contains("Dictionary"))
                    {
                        string lowerPropertyName = value.PropertyName.ToLower();
                        code.AppendLine($"        private {value.PropertyType} _{lowerPropertyName} = new {value.PropertyType}();");
                        code.AppendLine($"        public {value.PropertyType} {value.PropertyName}");
                        code.AppendLine("        {");
                        code.AppendLine($"            get => _{lowerPropertyName};");
                        code.AppendLine($"            set => _{lowerPropertyName} = value;");
                        code.AppendLine("        }");
                        code.AppendLine();
                    }
                }
                code.AppendLine();
                
                code.AppendLine($"        public void CopyTo({component.ClassName} component)");
                code.AppendLine("        {");
                if (valueTypeProperties.Count > 0)
                {
                    code.AppendLine($"            component._common = _common;");
                }
                
                foreach (BattleComponentProperty value in classTypeProperties)
                {
                    string lowerPropertyName = value.PropertyName.ToLower();
                    code.AppendLine($"            component._{lowerPropertyName} = _{lowerPropertyName};");
                }
                
                foreach (BattleComponentProperty value in copyValueTypeProperties)
                {
                    string lowerPropertyName = value.PropertyName.ToLower();
                    code.AppendLine($"            _{lowerPropertyName}.CopyTo(component._{lowerPropertyName});");
                }
                
                // 处理数组类型（包括多维数组和数组的数组）
                foreach (BattleComponentProperty value in arrayTypeProperties)
                {
                    string lowerPropertyName = value.PropertyName.ToLower();
                    code.AppendLine($"            Array.Copy(_{lowerPropertyName}, component._{lowerPropertyName}, _{lowerPropertyName}.Length);");
                }

                // 处理泛型类型（List和Dictionary）
                foreach (BattleComponentProperty value in genericTypeProperties)
                {
                    string lowerPropertyName = value.PropertyName.ToLower();
                    if (value.PropertyType.StartsWith("List<") && value.PropertyType.EndsWith(">"))
                    {
                        code.AppendLine($"            component._{lowerPropertyName}.Clear();");
                        code.AppendLine($"            component._{lowerPropertyName}.Capacity = _{lowerPropertyName}.Count;");
                        code.AppendLine($"            component._{lowerPropertyName}.AddRange(_{lowerPropertyName});");
                        code.AppendLine();
                    }
                    else if (value.PropertyType.StartsWith("Dictionary<") && value.PropertyType.EndsWith(">"))
                    {
                        code.AppendLine($"            component._{lowerPropertyName}.Clear();");
                        code.AppendLine($"            foreach (var item in _{lowerPropertyName})");
                        code.AppendLine("            {");
                        code.AppendLine($"                component._{lowerPropertyName}.Add(item.Key, item.Value);");
                        code.AppendLine("            }");
                    }
                }

                code.AppendLine("        }");
                code.AppendLine("    }");
                code.AppendLine("}");
                
                File.WriteAllText(Path.Combine(path, component.ClassName + ".cs"), code.ToString());
                valueTypeProperties.Clear();
                genericTypeProperties.Clear();
                arrayTypeProperties.Clear();
                copyValueTypeProperties.Clear();
            }
            
            AssetDatabase.Refresh();
            EditorUtility.DisplayDialog("成功", $"已保存到 {path}", "确定");
        }
        
        
        private bool IsNestedPropertyName(TType tType)
        {
            if (tType is TGenericType tGenericType)
            {
                return IsNestedProperty(tGenericType.GenericType);
            }
            if (tType is TDictionary tDictionary)
            {
                return IsNestedProperty(tDictionary.KeyGenericType) || IsNestedProperty(tDictionary.ValueGenericType);
            }
            return false;
        }
        
        private bool IsNestedProperty(TType tType)
        {
            if (tType is TGenericType tGenericType)
            {
                return true;
            }
            if (tType is TDictionary tDictionary)
            {
                return true;
            }
            return false;
        }
        
        
    }
}
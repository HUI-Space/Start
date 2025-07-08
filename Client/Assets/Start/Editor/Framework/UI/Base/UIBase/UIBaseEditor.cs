using System;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Start.Editor
{
    [CustomEditor(typeof(UIBase),true)]
    public class UIBaseEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("快捷绑定（脚本内序列化名称与游戏物体名称相同）"))
            {
                var go = target as UIBase;
                
                Type type = target.GetType();
                FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
                foreach (FieldInfo field in fields)
                {
                    var child = go.GetComponentsInChildren(field.FieldType,true);
                    foreach (var component in child)
                    {
                        if (component.name.Equals(field.Name))
                        {
                            var serialized = serializedObject.FindProperty(field.Name);
                            serialized.objectReferenceValue = component;
                        }
                    }
                }
                serializedObject.ApplyModifiedProperties();
            }
        }
    }
}
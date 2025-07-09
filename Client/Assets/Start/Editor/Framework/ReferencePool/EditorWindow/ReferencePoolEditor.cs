using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Start.Editor
{
    public class ReferencePoolEditor : EditorWindow
    {
        private static ReferencePoolEditor _window;

        private readonly Dictionary<string, List<ReferencePoolInfo>> m_ReferencePoolInfos = new Dictionary<string, List<ReferencePoolInfo>>(StringComparer.Ordinal);

        private readonly HashSet<string> m_OpenedItems = new HashSet<string>();
        private bool m_ShowFullClassName = false;
        
        public static void OpenEditorWindow()
        {
            _window = (ReferencePoolEditor)GetWindow(typeof(ReferencePoolEditor), false, "ReferencePool");
            _window.minSize = new Vector2(1400f, 800f);
            _window.Show();
        }

        private void OnGUI()
        {
            

            EditorGUILayout.LabelField("Reference Pool Count", ReferencePool.Count.ToString());
            m_ShowFullClassName = EditorGUILayout.Toggle("Show Full Class Name", m_ShowFullClassName);
            m_ReferencePoolInfos.Clear();
            ReferencePoolInfo[] referencePoolInfos = ReferencePool.GetAllReferencePoolInfos();
            foreach (ReferencePoolInfo referencePoolInfo in referencePoolInfos)
            {
                string assemblyName = referencePoolInfo.Type.Assembly.GetName().Name;
                List<ReferencePoolInfo> results = null;
                if (!m_ReferencePoolInfos.TryGetValue(assemblyName, out results))
                {
                    results = new List<ReferencePoolInfo>();
                    m_ReferencePoolInfos.Add(assemblyName, results);
                }

                results.Add(referencePoolInfo);
            }

            foreach (KeyValuePair<string, List<ReferencePoolInfo>> assemblyReferencePoolInfo in m_ReferencePoolInfos)
            {
                bool lastState = m_OpenedItems.Contains(assemblyReferencePoolInfo.Key);
                bool currentState = EditorGUILayout.Foldout(lastState, assemblyReferencePoolInfo.Key);
                if (currentState != lastState)
                {
                    if (currentState)
                    {
                        m_OpenedItems.Add(assemblyReferencePoolInfo.Key);
                    }
                    else
                    {
                        m_OpenedItems.Remove(assemblyReferencePoolInfo.Key);
                    }
                }

                if (currentState)
                {
                    EditorGUILayout.BeginVertical("box");
                    {
                        DrawReferencePoolInfo(m_ShowFullClassName ? "对象的全程" : "对象的名称",
                            "未使用引用数量",
                            "正在使用引用数量",
                            "获取引用数量",
                            "归还引用数量",
                            "增加引用数量",
                            "移除引用数量");
                        assemblyReferencePoolInfo.Value.Sort(Comparison);
                        foreach (ReferencePoolInfo referencePoolInfo in assemblyReferencePoolInfo.Value)
                        {
                            DrawReferencePoolInfo(m_ShowFullClassName ? referencePoolInfo.Type.FullName : referencePoolInfo.Type.GetFormattedName(),
                                $"{referencePoolInfo.UnusedReferenceCount}",
                                $"{referencePoolInfo.UsingReferenceCount}",
                                $"{referencePoolInfo.AcquireReferenceCount}",
                                $"{referencePoolInfo.ReleaseReferenceCount}",
                                $"{referencePoolInfo.AddReferenceCount}",
                                $"{referencePoolInfo.RemoveReferenceCount}");
                        }

                        if (GUILayout.Button("Export CSV Data"))
                        {
                            string exportFileName = EditorUtility.SaveFilePanel("Export CSV Data", string.Empty,
                                $"Reference Pool Data - {assemblyReferencePoolInfo.Key}.csv",
                                string.Empty);
                            if (!string.IsNullOrEmpty(exportFileName))
                            {
                                try
                                {
                                    int index = 0;
                                    string[] data = new string[assemblyReferencePoolInfo.Value.Count + 1];
                                    data[index++] =
                                        "Class Name,Full Class Name,Unused,Using,Acquire,Release,Add,Remove";
                                    foreach (ReferencePoolInfo referencePoolInfo in assemblyReferencePoolInfo.Value)
                                    {
                                        data[index++] =
                                            $"{referencePoolInfo.Type.Name},{referencePoolInfo.Type.FullName},{referencePoolInfo.UnusedReferenceCount},{referencePoolInfo.UsingReferenceCount},{referencePoolInfo.AcquireReferenceCount},{referencePoolInfo.ReleaseReferenceCount},{referencePoolInfo.AddReferenceCount},{referencePoolInfo.RemoveReferenceCount}";
                                    }

                                    File.WriteAllLines(exportFileName, data, Encoding.UTF8);
                                    Debug.Log($"Export reference pool CSV data to '{exportFileName}' success.");
                                }
                                catch (Exception exception)
                                {
                                    Debug.LogError(
                                        $"Export reference pool CSV data to '{exportFileName}' failure, exception is '{exception}'.");
                                }
                            }
                        }
                    }
                    EditorGUILayout.EndVertical();

                    EditorGUILayout.Separator();
                }
            }
        }
        
        
        
        
        private void DrawReferencePoolInfo(params string[] parameterName)
        {
            EditorGUILayout.BeginHorizontal();
            foreach (string item in parameterName)
            {
                EditorGUILayout.LabelField(item);  
            }
            EditorGUILayout.EndHorizontal();
        }
        

        private int Comparison(ReferencePoolInfo a, ReferencePoolInfo b)
        {
            if (m_ShowFullClassName)
            {
                return a.Type.FullName.CompareTo(b.Type.FullName);
            }
            else
            {
                return a.Type.Name.CompareTo(b.Type.Name);
            }
        }
        
        
    }
}
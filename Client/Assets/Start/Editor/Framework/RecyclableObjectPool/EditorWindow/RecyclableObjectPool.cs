using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Start.Editor
{
    public class RecyclableObjectPoolEditor : EditorWindow
    {
        private static RecyclableObjectPoolEditor _window;

        private readonly Dictionary<string, List<RecyclablePoolStats>> m_RecyclableObjectPoolInfos = new Dictionary<string, List<RecyclablePoolStats>>(StringComparer.Ordinal);

        private readonly HashSet<string> m_OpenedItems = new HashSet<string>();
        private bool m_ShowFullClassName = false;
        
        public static void OpenEditorWindow()
        {
            _window = (RecyclableObjectPoolEditor)GetWindow(typeof(RecyclableObjectPoolEditor), false, "RecyclableObjectPool");
            _window.minSize = new Vector2(1400f, 800f);
            _window.Show();
        }

        private void OnGUI()
        {
            

            EditorGUILayout.LabelField("Reference Pool Count", RecyclableObjectPool.Count.ToString());
            m_ShowFullClassName = EditorGUILayout.Toggle("Show Full Class Name", m_ShowFullClassName);
            m_RecyclableObjectPoolInfos.Clear();
            RecyclablePoolStats[] RecyclableObjectPoolInfos = RecyclableObjectPool.GetAllPoolInfos();
            foreach (RecyclablePoolStats RecyclablePoolStats in RecyclableObjectPoolInfos)
            {
                string assemblyName = RecyclablePoolStats.Type.Assembly.GetName().Name;
                List<RecyclablePoolStats> results = null;
                if (!m_RecyclableObjectPoolInfos.TryGetValue(assemblyName, out results))
                {
                    results = new List<RecyclablePoolStats>();
                    m_RecyclableObjectPoolInfos.Add(assemblyName, results);
                }

                results.Add(RecyclablePoolStats);
            }

            foreach (KeyValuePair<string, List<RecyclablePoolStats>> assemblyRecyclableObjectPoolInfo in m_RecyclableObjectPoolInfos)
            {
                bool lastState = m_OpenedItems.Contains(assemblyRecyclableObjectPoolInfo.Key);
                bool currentState = EditorGUILayout.Foldout(lastState, assemblyRecyclableObjectPoolInfo.Key);
                if (currentState != lastState)
                {
                    if (currentState)
                    {
                        m_OpenedItems.Add(assemblyRecyclableObjectPoolInfo.Key);
                    }
                    else
                    {
                        m_OpenedItems.Remove(assemblyRecyclableObjectPoolInfo.Key);
                    }
                }

                if (currentState)
                {
                    EditorGUILayout.BeginVertical("box");
                    {
                        DrawRecyclableObjectPoolInfo(m_ShowFullClassName ? "对象的全程" : "对象的名称",
                            "未使用引用数量",
                            "正在使用引用数量",
                            "获取引用数量",
                            "归还引用数量",
                            "增加引用数量",
                            "移除引用数量");
                        assemblyRecyclableObjectPoolInfo.Value.Sort(Comparison);
                        foreach (RecyclablePoolStats RecyclablePoolStats in assemblyRecyclableObjectPoolInfo.Value)
                        {
                            DrawRecyclableObjectPoolInfo(m_ShowFullClassName ? RecyclablePoolStats.Type.FullName : RecyclablePoolStats.Type.GetFormattedName(),
                                $"{RecyclablePoolStats.UnusedCount}",
                                $"{RecyclablePoolStats.UsingCount}",
                                $"{RecyclablePoolStats.AcquireCount}",
                                $"{RecyclablePoolStats.ReleaseCount}",
                                $"{RecyclablePoolStats.AddCount}",
                                $"{RecyclablePoolStats.RemoveCount}");
                        }

                        if (GUILayout.Button("Export CSV Data"))
                        {
                            string exportFileName = EditorUtility.SaveFilePanel("Export CSV Data", string.Empty,
                                $"Reference Pool Data - {assemblyRecyclableObjectPoolInfo.Key}.csv",
                                string.Empty);
                            if (!string.IsNullOrEmpty(exportFileName))
                            {
                                try
                                {
                                    int index = 0;
                                    string[] data = new string[assemblyRecyclableObjectPoolInfo.Value.Count + 1];
                                    data[index++] =
                                        "Class Name,Full Class Name,Unused,Using,Acquire,Release,Add,Remove";
                                    foreach (RecyclablePoolStats RecyclablePoolStats in assemblyRecyclableObjectPoolInfo.Value)
                                    {
                                        data[index++] =
                                            $"{RecyclablePoolStats.Type.Name},{RecyclablePoolStats.Type.FullName},{RecyclablePoolStats.UnusedCount},{RecyclablePoolStats.UsingCount},{RecyclablePoolStats.AcquireCount},{RecyclablePoolStats.ReleaseCount},{RecyclablePoolStats.AddCount},{RecyclablePoolStats.RemoveCount}";
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
        
        
        
        
        private void DrawRecyclableObjectPoolInfo(params string[] parameterName)
        {
            EditorGUILayout.BeginHorizontal();
            foreach (string item in parameterName)
            {
                EditorGUILayout.LabelField(item);  
            }
            EditorGUILayout.EndHorizontal();
        }
        

        private int Comparison(RecyclablePoolStats a, RecyclablePoolStats b)
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
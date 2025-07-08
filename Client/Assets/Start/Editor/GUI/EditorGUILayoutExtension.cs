using System;
using UnityEditor;
using UnityEngine;

namespace Start.Editor
{
    public static class EditorGUILayoutExtension
    {
        public static string On = nameof(On);
        public static string Off = nameof(Off);
        public static string Choose = nameof(Choose);
        public static GUILayoutOption CommonWidth = GUILayout.Width(60);
        public static GUILayoutOption CommonHeight = GUILayout.Height(18);

        /// <summary>
        /// 编辑器GUI 文件选择
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="filePath">选择文件路径</param>
        /// <param name="callback">选择文件路径成功后回调</param>
        public static void OpenFilePanel(string title, ref string filePath, Action<string> callback = default)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel(title);
                GUILayout.Space(2f);
                EditorGUILayout.LabelField(filePath, EditorStyles.textField);
                if (GUILayout.Button(Choose, CommonWidth))
                {
                    string path = EditorUtility.OpenFilePanel(title, Application.dataPath, string.Empty);
                    if (!string.IsNullOrEmpty(path))
                    {
                        filePath = path;
                        callback?.Invoke(path);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 编辑器GUI 文件夹选择
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="folderPath">选择文件夹路径</param>
        /// <param name="callback">选择文件夹路径成功后回调</param>
        public static void OpenFolderPanel(string title, ref string folderPath, Action<string> callback = default)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel(title);
                GUILayout.Space(2f);
                EditorGUILayout.LabelField(folderPath, EditorStyles.textField);
                if (GUILayout.Button(Choose, CommonWidth))
                {
                    string path = EditorUtility.OpenFolderPanel(title, Application.dataPath, string.Empty);
                    if (!string.IsNullOrEmpty(path))
                    {
                        folderPath = path;
                        callback?.Invoke(path);
                    }
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// 禁用文本字段
        /// </summary>
        /// <param name="title">标题</param>
        /// <param name="text">内容</param>
        public static void DisableTextField(string title, string text)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel(title);
                GUILayout.Space(2f);
                EditorGUILayout.LabelField(text, EditorStyles.textField);
            }
            EditorGUILayout.EndHorizontal();
        }

        public static void SwitchTextField(string title, ref string text, ref bool isEnable)
        {
            EditorGUILayout.BeginHorizontal();
            {
                EditorGUILayout.PrefixLabel(title);
                GUILayout.Space(2f);
                GUI.enabled = isEnable;
                text = EditorGUILayout.TextField(text, EditorStyles.textField);
                GUI.enabled = true;
                string buttonTitle = isEnable ? On : Off;
                if (GUILayout.Button(buttonTitle, CommonWidth))
                {
                    isEnable = !isEnable;
                }
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}
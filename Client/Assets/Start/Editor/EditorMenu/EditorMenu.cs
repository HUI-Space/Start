using UnityEditor;
using UnityEngine;

namespace Start.Editor
{
    public static class EditorMenu
    {
        #region OpenFolder
        
        [MenuItem("Start/打开文件夹/PersistentDataPath", false, 500)]
        public static void OpenPersistentDataFolder()
        {
            Application.OpenURL(Application.persistentDataPath);
        }

        [MenuItem("Start/打开文件夹/StreamingAssetsPath", false, 500)]
        public static void OpenStreamingAssetsFolder()
        {
            Application.OpenURL(Application.streamingAssetsPath);
        }
        
        #endregion

        #region 框架
        
        [MenuItem("Start/框架/资源引用编辑器窗口", false, 100)]
        public static void OpenReferencePoolEditor()
        {
            ReferencePoolEditor.OpenEditorWindow();
        }
        
        [MenuItem("Start/框架/配置编辑器窗口", false, 200)]
        public static void OpenConfigEditorWindow()
        {
            ConfigEditorWindow.OpenEditorWindow();
        }
        
        [MenuItem("Start/框架/资源编辑器窗口", false, 1300)]
        public static void OpenResourceEditorWindow()
        {
            ResourceEditorWindow.OpenEditorWindow();
        }
        
        [MenuItem("Start/框架/生成UI配置", false, 1600)]
        public static void Generate()
        {
            GenerateUIConfig.Generate();
        }
        
        #endregion
        
        #region 工具
        
        [MenuItem("Start/工具/查找资源引用编辑器窗口", false, 1700)]
        public static void OpenFindReferencesEditorWindow()
        {
            FindReferenceEditorWindow.OpenEditorWindow();
        }
        
        [MenuItem("Start/工具/查找编辑器图标编辑器窗口", false, 1600)]
        public static void OpenEditorIconEditorWindow()
        {
            EditorIconEditorWindow.OpenEditorWindow();
        }
        
        [MenuItem("Start/工具/查找GUI类型编辑器窗口", false, 1600)]
        public static void OpenGUIStyleEditorWindow()
        {
            GUIStyleEditorWindow.OpenEditorWindow();
        }
        
        #endregion
        
    }
}
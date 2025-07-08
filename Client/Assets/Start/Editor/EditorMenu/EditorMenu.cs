using UnityEditor;
using UnityEngine;

namespace Start.Editor
{
    public static class EditorMenu
    {
        #region OpenFolder
        
        [MenuItem("Start/OpenFolder/PersistentDataPath", false, 500)]
        public static void OpenPersistentDataFolder()
        {
            Application.OpenURL(Application.persistentDataPath);
        }

        [MenuItem("Start/OpenFolder/StreamingAssetsPath", false, 500)]
        public static void OpenStreamingAssetsFolder()
        {
            Application.OpenURL(Application.streamingAssetsPath);
        }
        
        #endregion

        #region Framework

        [MenuItem("Start/Framework/模板编辑器窗口", false, 100)]
        public static void OpenTemplateEditorWindow()
        {
            TemplateEditorWindow.OpenEditorWindow();
        }
        
        [MenuItem("Start/Framework/ConfigEditorWindow", false, 200)]
        public static void OpenConfigEditorWindow()
        {
            ConfigEditorWindow.OpenEditorWindow();
        }
        
        [MenuItem("Start/Framework/ResourceEditorWindow", false, 1300)]
        public static void OpenResourceEditorWindow()
        {
            ResourceEditorWindow.OpenEditorWindow();
        }
        
        [MenuItem("Start/Framework/GenerateUIConfig", false, 1600)]
        public static void Generate()
        {
            GenerateUIConfig.Generate();
        }

        #endregion
        
        #region Tools
        
        [MenuItem("Start/Tools/FindReferencesEditorWindow", false, 1600)]
        public static void OpenFindReferencesEditorWindow()
        {
            FindReferences.OpenEditorWindow();
        }
        
        #endregion
        
    }
}
using Start.Runtime;
using UnityEditor;
using UnityEngine;

namespace Start.Editor.ResourceEditor
{
    public class ResourceBuildView
    {
        private ResourceBuilderController _resourceBuilderController;
        public void OnEnable()
        {
            _resourceBuilderController = new ResourceBuilderController();
            _resourceBuilderController.OnEnable();
            ResourceBuildConfig resourceBuildConfig = _resourceBuilderController.ResourceBuildConfig;
            _currentVersion = resourceBuildConfig.LocalGameVersion;
            _setCurrentVersion = false;
        }

        private string _currentVersion;
        private bool _setCurrentVersion;
        public void OnGUI(Rect position)
        {
            if (_resourceBuilderController!=null && _resourceBuilderController .ResourceBuildConfig!=null)
            {
                ResourceBuildConfig resourceBuildConfig = _resourceBuilderController.ResourceBuildConfig;
                
                resourceBuildConfig.DisableWriteTypeTree = EditorGUILayout.Toggle(nameof(ResourceBuildConfig.DisableWriteTypeTree),resourceBuildConfig.DisableWriteTypeTree);
                resourceBuildConfig.IgnoreTypeTreeChanges = EditorGUILayout.Toggle(nameof(ResourceBuildConfig.IgnoreTypeTreeChanges),resourceBuildConfig.IgnoreTypeTreeChanges);
                resourceBuildConfig.BuildTarget = (BuildTarget)EditorGUILayout.EnumPopup(nameof(ResourceBuildConfig.BuildTarget), resourceBuildConfig.BuildTarget);
                resourceBuildConfig.BuildModeType = (BuildModeType)EditorGUILayout.EnumPopup(nameof(ResourceBuildConfig.BuildModeType), resourceBuildConfig.BuildModeType);
                resourceBuildConfig.CompressionType = (CompressionType)EditorGUILayout.EnumPopup(nameof(ResourceBuildConfig.CompressionType), resourceBuildConfig.CompressionType);
                resourceBuildConfig.resourceModeType = (EResourceModeType)EditorGUILayout.EnumPopup(nameof(ResourceBuildConfig.resourceModeType), resourceBuildConfig.resourceModeType);
                resourceBuildConfig.NameType = (NameType)EditorGUILayout.EnumPopup(nameof(ResourceBuildConfig.NameType), resourceBuildConfig.NameType);

                EditorGUILayout.LabelField("BuildVersion",resourceBuildConfig.LocalGameVersion);
                
                EditorGUILayout.BeginHorizontal();
                {
                    GUI.enabled = _setCurrentVersion;
                    _currentVersion = EditorGUILayout.TextField(nameof(ResourceBuildConfig.LocalGameVersion),_currentVersion);
                    GUI.enabled = true;
                    if (GUILayout.Button("Change", GUILayout.Width(60), GUILayout.Height(18)))
                    {
                        _setCurrentVersion = !_setCurrentVersion;
                    }
                }
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(nameof(ResourceBuildConfig.OutputDirectory),GUILayout.Width(150), GUILayout.Height(18));
                    GUI.enabled = false;
                    EditorGUILayout.TextField(resourceBuildConfig.OutputDirectory);
                    GUI.enabled = true;
                    if (GUILayout.Button("Choose", GUILayout.Width(60), GUILayout.Height(18)))
                    {
                        string path = UnityEditor.EditorUtility.OpenFolderPanel("Build AssetBundle Path",Application.dataPath,string.Empty);
                        if (!string.IsNullOrEmpty(path))
                        {
                            resourceBuildConfig.OutputDirectory = path.Replace(Application.dataPath,"Assets");
                        }
                    }
                }
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(nameof(ResourceBuildConfig.WorkingPath),GUILayout.Width(150), GUILayout.Height(18));
                    GUI.enabled = false;
                    EditorGUILayout.TextField(resourceBuildConfig.WorkingPath);
                    GUI.enabled = true;
                }
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(nameof(ResourceBuildConfig.OutputPath),GUILayout.Width(150), GUILayout.Height(18));
                    GUI.enabled = false;
                    EditorGUILayout.TextField(resourceBuildConfig.OutputPath);
                    GUI.enabled = true;
                }
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.BeginHorizontal();
                {
                    EditorGUILayout.LabelField(nameof(ResourceBuildConfig.ReportPath),GUILayout.Width(150), GUILayout.Height(18));
                    GUI.enabled = false;
                    EditorGUILayout.TextField(resourceBuildConfig.ReportPath);
                    GUI.enabled = true;
                }
                EditorGUILayout.EndHorizontal();
                
                /*if (GUILayout.Button("Reload"))
                {
                    _resourceBuilderController.Reload();
                }*/
                EditorGUILayout.Space(10f);
                if (GUILayout.Button("BuildAll"))
                {
                    SaveConfig();
                    _resourceBuilderController.BuildAll();
                }
                EditorGUILayout.Space(10f);
                if (GUILayout.Button("BuildPatch"))
                {
                    SaveConfig();
                    _resourceBuilderController.BuildPatch();
                }
                EditorGUILayout.Space(10f);
                if (GUILayout.Button("Save"))
                {
                    SaveConfig();
                }
            }
        }


        private void SaveConfig()
        {
            string[] v = _currentVersion.Split('.');
            if (!string.IsNullOrEmpty(_currentVersion) && v.Length == 3 && !string.IsNullOrEmpty(v[2]))
            {
                ResourceBuildConfig resourceBuildConfig = _resourceBuilderController.ResourceBuildConfig;
                resourceBuildConfig.LocalGameVersion = _currentVersion;
            }
            _resourceBuilderController.SaveConfig();
        }

        public void OnDestroy()
        {
            _resourceBuilderController.OnDestroy();
            _resourceBuilderController = default;
        }
    }
}
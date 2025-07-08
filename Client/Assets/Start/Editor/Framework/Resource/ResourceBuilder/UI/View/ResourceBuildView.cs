using UnityEditor;
using UnityEngine;

namespace Start.Editor
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
                resourceBuildConfig.ResourceModeType = (EResourceModeType)EditorGUILayout.EnumPopup(nameof(ResourceBuildConfig.ResourceModeType), resourceBuildConfig.ResourceModeType);
                resourceBuildConfig.NameType = (NameType)EditorGUILayout.EnumPopup(nameof(ResourceBuildConfig.NameType), resourceBuildConfig.NameType);
                
                EditorGUILayout.LabelField("BuildVersion",resourceBuildConfig.LocalGameVersion);
                
                EditorGUILayoutExtension.SwitchTextField(nameof(ResourceBuildConfig.LocalGameVersion), ref _currentVersion,ref _setCurrentVersion);
                EditorGUILayoutExtension.OpenFolderPanel(nameof(ResourceBuildConfig.OutputDirectory),ref resourceBuildConfig.OutputDirectory);
                EditorGUILayoutExtension.DisableTextField(nameof(ResourceBuildConfig.WorkingPath),resourceBuildConfig.WorkingPath);
                EditorGUILayoutExtension.DisableTextField(nameof(ResourceBuildConfig.OutputPath),resourceBuildConfig.OutputPath);
                EditorGUILayoutExtension.DisableTextField(nameof(ResourceBuildConfig.ReportPath),resourceBuildConfig.ReportPath);
                
                EditorGUILayout.Space(10f);
                if (GUILayout.Button("Save"))
                {
                    SaveConfig();
                }
                
                EditorGUILayout.Space(10f);
                if (GUILayout.Button("Build"))
                {
                    SaveConfig();
                    _resourceBuilderController.Build();
                }
                if (resourceBuildConfig.ResourceModeType == EResourceModeType.Updatable)
                {
                    EditorGUILayout.Space(10f);
                    if (GUILayout.Button("BuildPatch"))
                    {
                        SaveConfig();
                        _resourceBuilderController.BuildPatch();
                    }
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
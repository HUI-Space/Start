using UnityEditor;
using UnityEngine;

namespace Start.Editor
{
    public class ConfigEditorWindow : EditorWindow
    {
        private static ConfigEditorWindow _window;
        
        public static void OpenEditorWindow()
        {
            _window = (ConfigEditorWindow)GetWindow(typeof(ConfigEditorWindow), false, "导表工具");
            _window.minSize = new Vector2(1000f, 600f);
            _window.Show();
        }
        
        private string[] _toolbar;
        private EConfigToolbarType _configToolbarType;
        private ConfigController _configController;
        private ConfigSettingView _configSettingView;
        private ConfigBuildView _configBuildView;
        
        private void OnEnable()
        {
            _configToolbarType = EConfigToolbarType.Build;
            _toolbar = new[] { EConfigToolbarType.Setting.ToString(), EConfigToolbarType.Build.ToString() };
            _configController = new ConfigController();
            _configSettingView = new ConfigSettingView();
            _configBuildView = new ConfigBuildView();
            _configController.OnEnable(_configBuildView,_configSettingView);
            _configSettingView.OnEnable();
            _configBuildView.OnEnable();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();
            _configToolbarType = (EConfigToolbarType)GUILayout.Toolbar((int)_configToolbarType, _toolbar, new GUIStyle("LargeButton"));
            EditorGUILayout.Space();
            switch (_configToolbarType)
            {
                case EConfigToolbarType.Setting:
                    _configSettingView?.OnGUI(position);
                    break;
                case EConfigToolbarType.Build:
                    _configBuildView?.OnGUI(position);
                    break;
            }
        }

        private void OnDestroy()
        {
            _toolbar = default;
            _configToolbarType = default;
            _configController.OnDestroy();
            _configSettingView.OnDestroy();
            _configBuildView.OnDestroy();
            _configController = default;
            _configSettingView = default;
            _configBuildView = default;
        }
    }
}
using UnityEditor;
using UnityEngine;

namespace Start.Editor
{
    public class ResourceEditorWindow : EditorWindow
    {
        private static ResourceEditorWindow _window;

        public static void OpenEditorWindow()
        {
            _window = (ResourceEditorWindow)GetWindow(typeof(ResourceEditorWindow), false, "资源管理器");
            _window.minSize = new Vector2(1400f, 800f);
            _window.Show();
        }

        private ResourceConfigView _resourceConfigView;
        private ResourceBuildView _resourceBuildView;
        private ToolbarType _toolbarType;
        private string[] _toolbar;

        private void OnEnable()
        {
            _resourceConfigView = new ResourceConfigView();
            _resourceBuildView = new ResourceBuildView();
            _toolbar = new[] { ToolbarType.Build.ToString(), ToolbarType.Config.ToString(), ToolbarType.Analysis.ToString() };
            _toolbarType = ToolbarType.Build;
            _resourceConfigView.OnEnable();
            _resourceBuildView.OnEnable();
        }

        private void OnGUI()
        {
            EditorGUILayout.Space();
            _toolbarType = (ToolbarType)GUILayout.Toolbar((int)_toolbarType, _toolbar, "LargeButton");
            EditorGUILayout.Space();
            switch (_toolbarType)
            {
                case ToolbarType.Config:
                    _resourceConfigView?.OnGUI(position);
                    break;
                case ToolbarType.Analysis:

                    break;
                case ToolbarType.Build:
                    _resourceBuildView?.OnGUI(position);
                    break;
            }
        }

        private void OnDestroy()
        {
            _resourceConfigView.OnDestroy();
            _resourceBuildView.OnDestroy();
        }
    }
}
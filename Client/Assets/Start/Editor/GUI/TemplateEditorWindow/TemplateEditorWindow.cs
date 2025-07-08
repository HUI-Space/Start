using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Start.Editor
{
    public class TemplateEditorWindow : EditorWindow
    {
        private static TemplateEditorWindow _window;

        public static void OpenEditorWindow()
        {
            _window = (TemplateEditorWindow)GetWindow(typeof(ResourceEditorWindow), false, "模板编辑器窗口");
            _window.minSize = new Vector2(1400f, 800f);
            _window.Show();
        }

        private void OnEnable()
        {
            
        }

        private void OnGUI()
        {
            
        }

        private void OnDestroy()
        {
            
        }
    }
}
using UnityEditor;
using UnityEngine;

namespace Start.Editor
{
    [CustomEditor(typeof(ScrollBase),true)]
    public class ScrollBaseEditor : UnityEditor.Editor
    {
        public static bool Preview;
        public static GameObject Prefab;

        public override void OnInspectorGUI()
        {
            ScrollBase scroll = (ScrollBase)target;
            
            base.OnInspectorGUI();
            Preview = EditorGUILayout.Toggle("是否预览预制体？",Preview);
            if (Preview)
            {
                Prefab = (GameObject)EditorGUILayout.ObjectField("预览预制体",Prefab,typeof(GameObject),true);
                if (GUILayout.Button("预览"))
                {
                    UIElement uiElement = Prefab.GetComponent<UIElement>();
                    bool contain = true;
                    if (uiElement == null)
                    {
                        uiElement = Prefab.AddComponent<DefaultUIElement>();
                        contain = false;
                    }
                    scroll.Preview(Prefab);
                    if (!contain)
                    {
                        DestroyImmediate(uiElement);
                    }
                }
                if (GUILayout.Button("结束预览"))
                {
                    scroll.EndPreview();
                    Preview = false;
                    Prefab = default;
                }
            }
        }
    }
}
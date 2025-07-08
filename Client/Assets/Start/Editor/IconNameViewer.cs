using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class IconNameViewer : EditorWindow
{
    private Object selectedAsset;
    private string iconName;

    [MenuItem("Tools/Icon Name Viewer")]
    public static void ShowWindow()
    {
        GetWindow<IconNameViewer>("Icon Name Viewer");
    }

    private void OnGUI()
    {
        GUILayout.Label("Select an asset to view its icon name", EditorStyles.boldLabel);

        // 选择资源
        selectedAsset = EditorGUILayout.ObjectField("Selected Asset", selectedAsset, typeof(Object), false);

        if (selectedAsset != null)
        {
            // 获取资源对应的图标
            var iconContent = EditorGUIUtility.ObjectContent(selectedAsset, selectedAsset.GetType());
            if (iconContent.image != null)
            {
                // 显示图标
                GUILayout.Label("Icon Preview:");
                GUILayout.Box(iconContent.image, GUILayout.Width(64), GUILayout.Height(64));

                // 输出图标名称
                iconName = iconContent.image.name;
                GUILayout.Label($"Icon Name: {iconName}");
            }
            else
            {
                GUILayout.Label("No icon found for this asset.");
            }
        }
        else
        {
            GUILayout.Label("Please select an asset.");
        }
    }
}
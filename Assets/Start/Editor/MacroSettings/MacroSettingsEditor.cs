using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MacroSettings))]
public class MacroSettingsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // 绘制默认的 Inspector 界面

        MacroSettings myObject = (MacroSettings)target;

        if (GUILayout.Button("保存宏"))
        {
            // 按钮点击时执行的操作
            myObject.SavaMacro();
        }
    }
}
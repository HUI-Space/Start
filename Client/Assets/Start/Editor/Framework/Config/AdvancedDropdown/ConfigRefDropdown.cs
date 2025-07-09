using System;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class ConfigRefDropdown : AdvancedDropdown
{
    private Action<int> _onSelect;
    private string[] _optionDes;
    private string _title;

    public ConfigRefDropdown(string title, string[] optionDes, Action<int> onSelect) : base(new AdvancedDropdownState())
    {
        _onSelect = onSelect;
        _optionDes = optionDes;
        _title = title;
    }

    protected override AdvancedDropdownItem BuildRoot()
    {
        AdvancedDropdownItem root = new AdvancedDropdownItem(_title);
        for (int i = 0; i < _optionDes.Length; i++)
        {
            var item = new AdvancedDropdownItem(_optionDes[i]);
            item.id = i;
            root.AddChild(item);
        }

        return root;
    }

    protected override void ItemSelected(AdvancedDropdownItem item)
    {
        base.ItemSelected(item);
        _onSelect?.Invoke(item.id);
    }

    public static void Draw(SerializedProperty property, Rect rect, string title, int currentSelectIndex, string[] descriptions, Action<int> onSelect)
    {
        if (currentSelectIndex == -1)
        {
            return;
        }

        GUILayout.BeginHorizontal(SirenixGUIStyles.Label);
        GUILayout.Space(12);
        GUILayout.Label(property.displayName);
        GUILayout.FlexibleSpace();
        if (GUILayout.Button(descriptions[currentSelectIndex], EditorStyles.popup))
        {
            ConfigRefDropdown configRefDropdown = new ConfigRefDropdown(title, descriptions, onSelect);
            configRefDropdown.Show(rect);
        }

        GUILayout.EndHorizontal();
    }
}
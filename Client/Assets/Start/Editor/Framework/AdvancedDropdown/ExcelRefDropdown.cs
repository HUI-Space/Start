using System;
using Sirenix.Utilities.Editor;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

public class ExcelRefDropdown : AdvancedDropdown
{
    private Action<int> _onSelect;
    private string[] _optionDescs;
    private string _title;

    public ExcelRefDropdown(string title, string[] _optionDescs, Action<int> onSelect) : base(new AdvancedDropdownState())
    {
        this._onSelect = onSelect;
        this._optionDescs = _optionDescs;
        this._title = title;
    }

    protected override AdvancedDropdownItem BuildRoot()
    {
        AdvancedDropdownItem root = new AdvancedDropdownItem(_title);
        for (int i = 0; i < _optionDescs.Length; i++)
        {
            var item = new AdvancedDropdownItem(_optionDescs[i]);
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
            ExcelRefDropdown excelRefDropdown = new ExcelRefDropdown(title, descriptions, onSelect);
            excelRefDropdown.Show(rect);
        }

        GUILayout.EndHorizontal();
    }
}
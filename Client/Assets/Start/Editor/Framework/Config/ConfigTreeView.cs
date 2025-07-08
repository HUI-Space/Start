using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Start.Editor
{
    public partial class ConfigTreeView : TreeView
    {
        const float ToggleWidth = 18f;
        public ConfigTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader) : base(state, multiColumnHeader)
        {
            showBorder = true;
            showAlternatingRowBackgrounds = true;
        }
        
        public void OnEnable()
        {
            Reload();
        }

        public void IsSelectAll(bool select)
        {
            foreach (var item in rootItem.children)
            {
                if (item is ConfigTreeViewItem configTreeViewItem)
                {
                    configTreeViewItem.Enable = select;
                }
            }
        }
        
        public List<string> GetSelectSheets()
        {
            List<string> result = new List<string>();
            foreach (var item in rootItem.children)
            {
                if (item is ConfigTreeViewItem configTreeViewItem)
                {
                    if (configTreeViewItem.Enable)
                    {
                        result.Add(configTreeViewItem.displayName);
                    }
                }
            }
            return result;
        }

        protected override TreeViewItem BuildRoot()
        {
            TreeViewItem root = new TreeViewItem(GetHashCode(), -1);
            root.children = new List<TreeViewItem>();
            int index = 0;
            foreach (var item in ConfigController.Instance.Sheets)
            {
                root.AddChild(new ConfigTreeViewItem(index, 0, false, item.Key, item.Value));
                index++;
            }
            
            return root;
        }


        protected override void RowGUI(RowGUIArgs args)
        {
            for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
            {
                CellGUI(args.GetCellRect(i), (ConfigTreeViewItem)args.item, args.GetColumn(i), ref args);
            }
        }

        private void CellGUI(Rect cellRect, ConfigTreeViewItem item, int column, ref RowGUIArgs args)
        {
            CenterRectUsingSingleLineHeight(ref cellRect);
            switch (column)
            {
                case 0:
                {
                    Rect toggleRect = cellRect;
                    toggleRect.x += GetContentIndent(item);
                    toggleRect.width = ToggleWidth;
                    item.Enable = EditorGUI.Toggle(toggleRect, item.Enable);
                    
                    Rect textureRect = toggleRect;
                    textureRect.x += toggleRect.x;
                    GUI.DrawTexture (textureRect, EditorGUIUtility.IconContent("TextAsset Icon").image, ScaleMode.ScaleToFit);
                    
                    cellRect.x = textureRect.x;
                    args.rowRect = cellRect;
                    base.RowGUI(args);
                    break;
                }
                case 1:
                {
                    DefaultGUI.Label(cellRect, item.FullName, args.selected, args.focused);
                    break;
                }
            }
        }
    }

    public partial class ConfigTreeView
    {
        public static MultiColumnHeaderState CreateAssetMultiColumnHeaderState()
        {
            var columns = new[]
            {
                new MultiColumnHeaderState.Column
                {
                    headerContent = new GUIContent("Name","Excel Name"),
                    headerTextAlignment = TextAlignment.Left,
                    canSort = true,
                    width = 400,
                    minWidth = 400,
                    autoResize = false,
                    allowToggleVisibility = false,
                    sortingArrowAlignment = TextAlignment.Center
                },
                new MultiColumnHeaderState.Column()
                {
                    headerContent = new GUIContent("Path", "Excel Path!"),
                    width = 600,
                    minWidth = 600,
                    headerTextAlignment = TextAlignment.Left,
                    canSort = false,
                    autoResize = true,
                }
            };
            return new MultiColumnHeaderState(columns);
        }
    }
}
using System.Collections.Generic;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEditor;

namespace Start.Editor
{
    public partial class ResourceTreeView : TreeView
    {
        private ResourceConfigController _controller;

        private readonly ObjectPool<ResourceTreeViewItem> _objectPool =
            new ObjectPool<ResourceTreeViewItem>();

        public ResourceTreeView(TreeViewState state, MultiColumnHeader multiColumnHeader,
            ResourceConfigController controller) : base(state, multiColumnHeader)
        {
            showBorder = true;
            showAlternatingRowBackgrounds = true;
            _controller = controller;
            _controller.OnChooseChange += OnChooseChange;
        }

        private void OnChooseChange()
        {
            if (_controller.ChooseType == ChooseType.ResourceGroupItem)
                ReloadData();
        }

        public void ReloadData()
        {
            if (_root.children != null)
            {
                foreach (var item in _root.children)
                {
                    if (item is ResourceTreeViewItem treeViewItem)
                    {
                        _objectPool.Release(treeViewItem);
                    }
                }
                _root.children.Clear();
            }
            Reload();
        }

        public void OnEnable()
        {
            Reload();
        }

        public void OnDestroy()
        {
            _controller = default;
            _objectPool.Clear();
        }

        private TreeViewItem _root;

        protected override TreeViewItem BuildRoot()
        {
            if (_root == null)
            {
                _root = new TreeViewItem(GetHashCode(), -1);
            }
            ResourceGroupItem resourceGroupItem = _controller.ChooseResourceGroupItem;
            if (resourceGroupItem != null && !string.IsNullOrEmpty(resourceGroupItem.Path))
            {
                string[] paths = Utility.FindAssets(ESearchType.All, resourceGroupItem.Path);
                for (int i = 0; i < paths.Length; i++)
                {
                    ResourceTreeViewItem resourceTreeViewItem = _objectPool.Get();
                    resourceTreeViewItem.OnEnable(i, paths[i]);
                    _root.AddChild(resourceTreeViewItem);
                }
            }

            return _root;
        }

        protected override void RowGUI(RowGUIArgs args)
        {
            for (int i = 0; i < args.GetNumVisibleColumns(); ++i)
            {
                CellGUI(args.GetCellRect(i), (ResourceTreeViewItem)args.item, args.GetColumn(i), ref args);
            }
        }

        private void CellGUI(Rect cellRect, ResourceTreeViewItem item, int column, ref RowGUIArgs args)
        {
            Color oldColor = GUI.color;
            CenterRectUsingSingleLineHeight(ref cellRect);

            switch (column)
            {
                case 0:
                {
                    var iconRect = new Rect(cellRect.x + 1, cellRect.y + 1, cellRect.height - 2, cellRect.height - 2);
                    if (item.Icon != null)
                        GUI.DrawTexture(iconRect, item.Icon, ScaleMode.ScaleToFit);
                    DefaultGUI.Label(
                        new Rect(cellRect.x + iconRect.xMax + 1, cellRect.y, cellRect.width - iconRect.width,
                            cellRect.height),
                        item.Name,
                        args.selected,
                        args.focused);
                }
                    break;
                case 1:
                    DefaultGUI.Label(cellRect, item.FullName, args.selected, args.focused);
                    break;
                case 2:
                    DefaultGUI.Label(cellRect, item.Dependencies.Length.ToString(), args.selected, args.focused);
                    break;
            }

            GUI.color = oldColor;
        }

        protected override IList<TreeViewItem> BuildRows(TreeViewItem root)
        {
            return base.BuildRows(root);
        }

        protected override void SingleClickedItem(int id)
        {
            base.SingleClickedItem(id);
            var treeViewItem = (ResourceTreeViewItem)FindItem(id, rootItem);
            Object o = AssetDatabase.LoadAssetAtPath<Object>(treeViewItem.FullName);
            EditorGUIUtility.PingObject(o);
        }
    }

    public partial class ResourceTreeView
    {
        public static MultiColumnHeaderState CreateAssetMultiColumnHeaderState()
        {
            MultiColumnHeaderState.Column[] columns = new MultiColumnHeaderState.Column[]
            {
                new MultiColumnHeaderState.Column()
                {
                    headerContent = new GUIContent("Name", "Asset In Project Name!"),
                    minWidth = 200,
                    width = 200,
                    headerTextAlignment = TextAlignment.Left,
                    canSort = false,
                    autoResize = true,
                },
                new MultiColumnHeaderState.Column()
                {
                    headerContent = new GUIContent("Path", "Asset In Project Path!"),
                    minWidth = 400,
                    width = 400,
                    headerTextAlignment = TextAlignment.Left,
                    canSort = false,
                    autoResize = true,
                },
                new MultiColumnHeaderState.Column()
                {
                    headerContent = new GUIContent("Dependency", "Dependency Asset Count"),
                    minWidth = 80,
                    width = 80,
                    headerTextAlignment = TextAlignment.Left,
                    canSort = true,
                    autoResize = false,
                },
            };
            return new MultiColumnHeaderState(columns);
        }
    }
}
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Start.Editor
{
    public class GroupTreeView:TreeView
    {
        private static readonly Texture2D Folder = EditorGUIUtility.FindTexture("Folder Icon");
        private static readonly Texture2D Group = EditorGUIUtility.FindTexture("Prefab On Icon");
        private static readonly Texture2D EmptyGroup = (Texture2D)EditorGUIUtility.IconContent("GameObject Icon").image;
        private static readonly Texture2D GroupItem = EditorGUIUtility.FindTexture("Prefab Icon");
        private static readonly string GroupName = $"[{nameof(ResourceGroup)}]:";
        public GroupTreeView(TreeViewState state,ResourceConfigController controller) : base(state)
        {
            _controller = controller;
            showBorder = true;
        }
        private ResourceConfigController _controller;
        private List<TreeViewItem> _selectedNodes;
        private List<int> _selectItems;
        private List<int> _expandItems;
        private TreeViewItem _groupsRoot;
        private GenericMenu _emptyMenu;
        private GenericMenu _groupMenu;
        private GenericMenu _groupItemMenu;
        private GUIContent _createGroup;
        private GUIContent _renameGroup;
        private GUIContent _deleteGroup;
        private GUIContent _createGroupItem;
        private GUIContent _deleteGroupItem;
        private bool _contextOnItem;
        
        public void OnEnable()
        {
            _contextOnItem = false;
            _selectedNodes = new List<TreeViewItem>();
            _selectItems = new List<int>();
            _expandItems = new List<int>();
            _createGroup = new GUIContent("Create ResourceGroup");
            _renameGroup = new GUIContent("Rename ResourceGroup");
            _deleteGroup = new GUIContent("Delete ResourceGroup");
            _createGroupItem = new GUIContent("Create ResourceGroup Item");
            _deleteGroupItem = new GUIContent("Delete ResourceGroup Item");
            _emptyMenu = new GenericMenu();
            _groupMenu = new GenericMenu();
            _groupItemMenu = new GenericMenu();
            _emptyMenu.AddItem(_createGroup, false, CreateGroup, _selectedNodes);
            _groupMenu.AddItem(_createGroup, false, CreateGroup, _selectedNodes);
            _groupMenu.AddItem(_renameGroup, false, RenameGroup, _selectedNodes);
            _groupMenu.AddItem(_deleteGroup, false, DeleteGroup, _selectedNodes);
            _groupMenu.AddItem(_createGroupItem, false, CreateGroupItem, _selectedNodes);
            _groupItemMenu.AddItem(_createGroupItem, false, CreateGroupItem, _selectedNodes);
            _groupItemMenu.AddItem(_deleteGroupItem, false, DeleteGroupItem, _selectedNodes);
            Reload();
            ExpandAll();
        }
        
        public void OnDestroy()
        {
            _selectedNodes.Clear();
            _selectItems.Clear();
            _expandItems.Clear();
            _controller = default;
            _contextOnItem = default;
            _selectedNodes = default;
            _selectItems = default;
            _expandItems = default;
            _createGroup = default;
            _renameGroup = default;
            _deleteGroup = default;
            _createGroupItem = default;
            _deleteGroupItem = default;
            _emptyMenu = default;
            _groupMenu = default;
            _groupItemMenu = default;
        }
        
        private void DeleteGroupItem(object userdata)
        {
            if (!(userdata is List<TreeViewItem> selectedNodes) || selectedNodes.Count != 1) return;
            _controller.RemoveGroupItem(selectedNodes[0].parent.id,selectedNodes[0].id);
            Reload();
        }

        private void CreateGroupItem(object userdata)
        {
            if (!(userdata is List<TreeViewItem> selectedNodes) || selectedNodes.Count != 1) return;
            int hashCode = 0;
            if (selectedNodes[0].depth == 1)
            {
                ResourceGroupItem resourceGroupItem = _controller.CreateGroupItem(selectedNodes[0].id);
                hashCode = resourceGroupItem.GetHashCode();
                SelectItem(hashCode,selectedNodes[0]);
            }
            else if (selectedNodes[0].depth == 2)
            {
                ResourceGroupItem resourceGroupItem = _controller.CreateGroupItem(selectedNodes[0].parent.id);
                hashCode = resourceGroupItem.GetHashCode();
                SelectItem(hashCode,selectedNodes[0].parent);
            }
        }


        private void CreateGroup(object userdata)
        {
            int count = _groupsRoot.children !=null ? _groupsRoot.children.Count : 0;
            ResourceGroup resourceGroup = _controller.CreateGroup($"{count}");
            if (resourceGroup == null) return;
            int hashCode = resourceGroup.GetHashCode();
            SelectItem(hashCode, _groupsRoot);
            RenameItem(hashCode);
        }
        
        private void RenameGroup(object userdata)
        {
            if (!(userdata is List<TreeViewItem> selectedNodes) || selectedNodes.Count != 1) return;
            BeginRename(selectedNodes[0], 0.25f);
        }
        
        private void DeleteGroup(object userdata)
        {
            if (!(userdata is List<TreeViewItem> selectedNodes) || selectedNodes.Count <= 0) return;
            foreach (TreeViewItem treeViewItem in selectedNodes)
            {
                if (treeViewItem.depth == 1)
                {
                    _controller.RemoveGroup(treeViewItem.id);
                }
            }
            Reload();
        }

        private void SelectItem(int hashCode, TreeViewItem treeViewItem)
        {
            _selectItems.Clear();
            _expandItems.Clear();
            _selectItems.Add(hashCode);
            _expandItems.AddRange(GetExpanded());
            _expandItems.Add(treeViewItem.id);
            Reload();
            SelectItems(_selectItems, _expandItems);
        }
        
        private void SelectItems(List<int> selectItems, List<int> expandItems)
        {
            SetSelection(selectItems, TreeViewSelectionOptions.FireSelectionChanged);
            SetExpanded(expandItems);
        }
        
        private void RenameItem(int hashCode)
        {
            var treeViewItem = FindItem(hashCode, rootItem);
            BeginRename(treeViewItem, 0.25f);
        }
        

        protected override TreeViewItem BuildRoot()
        {
            TreeViewItem root = new TreeViewItem       { id = 0, depth = -1, displayName = "Root" };
            TreeViewItem groupsRoot = new TreeViewItem { id = 0, depth = 0, displayName = "ResourceGroups" , icon = Folder};
            
            Dictionary<int, ResourceGroup> groups = _controller.GetGroups();
            foreach (var group in groups)
            {
                TreeViewItem treeView = new TreeViewItem()
                {
                    id = group.Key, depth = 1, displayName = $"{GroupName}{group.Value.Name}", icon = group.Value.GroupItems.Count > 0 ? Group : EmptyGroup
                };
                foreach (ResourceGroupItem groupItem in group.Value.GroupItems)
                {
                    TreeViewItem treeViewItem = new TreeViewItem()
                    {
                        id = groupItem.GetHashCode(), depth = 2, displayName = $"[{group.Value.Name}]:{groupItem.Path}" , icon = GroupItem
                    };
                    treeView.AddChild(treeViewItem);
                }
                groupsRoot.AddChild(treeView);
            }
            
            _groupsRoot = groupsRoot;
            root.AddChild(groupsRoot);
            SetupDepthsFromParentsAndChildren(root);
            return root;
        }

        protected override void ContextClicked()
        {
            if (_contextOnItem)
            {
                _contextOnItem = false;
                return;
            }
            _selectedNodes.Clear();
            _emptyMenu.ShowAsContext();
        }

        protected override void ContextClickedItem(int id)
        {
            _contextOnItem = true;
            _selectedNodes.Clear();
            foreach (var nodeID in GetSelection())
            {
                _selectedNodes.Add(FindItem(nodeID, rootItem));
            }

            if (_selectedNodes.Count != 1) return;
            if ( _selectedNodes[0].depth == 1)
            {
                _groupMenu.ShowAsContext();
            }
            else if ( _selectedNodes[0].depth == 2)
            {
                _groupItemMenu.ShowAsContext();
            }
            else
            {
                _emptyMenu.ShowAsContext();
            }
        }
        

        protected override bool CanRename(TreeViewItem item)
        {
            return item != null && item.displayName.Length > 0;
        }

        protected override void RenameEnded(RenameEndedArgs args)
        {
            base.RenameEnded(args);
            if (args.newName.Length > 0 && args.newName != args.originalName)
            {
                args.acceptedRename = true;
                TreeViewItem item = FindItem(args.itemID, rootItem);
                string newName = args.newName;
                if (args.newName.Contains(GroupName))
                {
                    newName = newName.Remove(0, GroupName.Length);
                }
                if (item.depth != 1 || !_controller.RenameGroup(item.id, newName)) return;
                Reload();
            }
            else
            {
                args.acceptedRename = false;
            }
        }
        
        protected override void SelectionChanged(IList<int> selectedIds)
        {
            base.SelectionChanged(selectedIds);
            if (selectedIds.Count == 1)
            {
                TreeViewItem item = FindItem(selectedIds[0], rootItem);
                if (item.depth == 1)
                {
                    _controller.SelectionChanged(ChooseType.ResourceGroup,selectedIds[0]);
                }
                else if (item.depth == 2)
                {
                    _controller.SelectionChanged(ChooseType.ResourceGroupItem,selectedIds[0]);
                }
                else 
                {
                    _controller.SelectionChanged(ChooseType.Groups,0);
                }
            }
        }
        
    }
}
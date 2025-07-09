using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Start.Editor
{
    public class ResourceConfigView
    {
        private TreeViewState _groupTreeViewState;
        private TreeViewState _assetTreeViewState;
        private GroupTreeView _groupTreeView;
        private ResourceTreeView _resourceTreeView;
        private ResourceConfigController _controller;
        
        private const float CommonOffsetX = 3f;
        private const float CommonOffsetY = 40f;
        private readonly float ResourceGroupOffset = (typeof(ResourceGroup).GetFields().Length - 1) * 18 + 12;
        private readonly float ResourceGroupItemOffset = typeof(ResourceGroupItem).GetFields().Length * 18 + 12;
        
        private static readonly GUIContent _reload = new GUIContent("Reload All", "重新加载");
        private static readonly GUIContent _expand = new GUIContent("Expand All", "展开所有");
        private static readonly GUIContent _clear = new GUIContent("Clear Empty", "删除空文件夹以及包");
        private static readonly GUIContent _save = new GUIContent("Save All", "保存所有");
        
        public void OnEnable()
        {
            _controller = new ResourceConfigController();
            _groupTreeViewState = new TreeViewState();
            _assetTreeViewState = new TreeViewState();
            _groupTreeView = new GroupTreeView(_groupTreeViewState,_controller);
            _resourceTreeView = new ResourceTreeView(_assetTreeViewState, new MultiColumnHeader(ResourceTreeView.CreateAssetMultiColumnHeaderState()), _controller);
            _controller.OnEnable();
            _groupTreeView.OnEnable();
            _resourceTreeView.OnEnable();
        }

        public void OnDestroy()
        {
            _controller.OnDestroy();
            _groupTreeView.OnDestroy();
            _resourceTreeView.OnDestroy();
            _groupTreeViewState = default;
            _assetTreeViewState = default;
            _groupTreeView = default;
            _resourceTreeView = default;
            _controller = default;
        }
        
        public void OnGUI(Rect position)
        {
            float width = position.width / 2 - CommonOffsetX;
            float height = position.height - 77;
            _groupTreeView?.OnGUI(new Rect(CommonOffsetX,CommonOffsetY ,width,height));
            OnBottomGUI(position, width);
            OnRightGUI(position,width);
        }

        #region Right
        
        private void OnRightGUI(Rect position,float width)
        {
            if (_controller.ChooseHashCode != 0  )
            {
                if (_controller.ChooseType == ChooseType.ResourceGroup && _controller.ChooseResourceGroup != null)
                {
                    GUILayout.BeginArea(new Rect(width + CommonOffsetX , CommonOffsetY , width, ResourceGroupOffset),string.Empty,"HelpBox");
                    ResourceGroup resourceGroup = _controller.ChooseResourceGroup;
                    resourceGroup.Enable = EditorGUILayout.Toggle(nameof(ResourceGroup.Enable),resourceGroup.Enable);
                    EditorGUI.BeginChangeCheck();
                    resourceGroup.Name = EditorGUILayout.TextField(nameof(ResourceGroup.Name),resourceGroup.Name);
                    if (EditorGUI.EndChangeCheck())
                    {
                        _groupTreeView.Reload();
                    }
                    resourceGroup.Tags = EditorGUILayout.TextField(nameof(ResourceGroup.Tags),resourceGroup.Tags);
                    resourceGroup.Desc = EditorGUILayout.TextField(nameof(ResourceGroup.Desc),resourceGroup.Desc);
                    GUILayout.EndArea();
                }
                else if (_controller.ChooseType == ChooseType.ResourceGroupItem &&_controller.ChooseResourceGroupItem != null)
                {
                    GUILayout.BeginArea(new Rect(width + CommonOffsetX , CommonOffsetY , width, ResourceGroupItemOffset),string.Empty,"HelpBox");
                    ResourceGroupItem groupItem = _controller.ChooseResourceGroupItem;
                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(nameof(ResourceGroupItem.Path),GUILayout.Width(150), GUILayout.Height(18));
                    GUI.enabled = false;
                    EditorGUILayout.TextField(groupItem.Path);
                    GUI.enabled = true;
                    if (GUILayout.Button("Choose",GUILayout.Width(60), GUILayout.Height(18)))
                    {
                        string path = UnityEditor.EditorUtility.OpenFolderPanel("Choose Resource",Application.dataPath,string.Empty);
                        if (!string.IsNullOrEmpty(path))
                        {
                            groupItem.Path = path.Replace(Application.dataPath,"Assets");
                            _resourceTreeView.ReloadData();
                            _groupTreeView.Reload();
                        }
                    }
                    EditorGUILayout.EndHorizontal();
                    groupItem.CollectorType = (CollectorType)EditorGUILayout.EnumPopup(nameof(CollectorType),groupItem.CollectorType);
                    groupItem.PackType = (PackType)EditorGUILayout.EnumPopup(nameof(PackType),groupItem.PackType);
                    groupItem.AddressType = (AddressType)EditorGUILayout.EnumPopup(nameof(AddressType),groupItem.AddressType);
                    GUILayout.EndArea();
                    _resourceTreeView?.OnGUI(new Rect(width + CommonOffsetX,CommonOffsetY  + ResourceGroupItemOffset,width,position.height - 77 -ResourceGroupItemOffset));
                }
            }
            
        }
        
        #endregion
        
        
        #region Bottom


        private void OnBottomGUI(Rect position,float width)
        {
            GUILayout.BeginArea(new Rect( CommonOffsetX , position.height - 33 , position.width - 6, 30),"","HelpBox");
            GUILayout.Space(CommonOffsetX);
            EditorGUILayout.BeginHorizontal();
            
            if ( GUILayout.Button(_reload))
            {
                Reload();
            }
            if ( GUILayout.Button(_expand))
            {
                ExpandAll();
            }
            if ( GUILayout.Button(_clear))
            {
                ClearEmpty();
            }
            if ( GUILayout.Button(_save))
            {
                SaveConfig();
            }
           
            EditorGUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

        private void Reload()
        {
            _controller.Reload();
            _groupTreeView.Reload();
            _groupTreeView.ExpandAll();
        }
        
        private void ExpandAll()
        {
            _groupTreeView.ExpandAll();
        }
        
        private void ClearEmpty()
        {
            _controller.ClearEmpty();
            _groupTreeView.Reload();
        }

        private void SaveConfig()
        {
            _controller.SaveConfig();
        }
        
        #endregion
    }
}
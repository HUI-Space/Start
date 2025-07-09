using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Start.Editor
{
    public class ConfigBuildView
    {
        private string _selectAll;
        private string _unSelectAll;
        private string _outputScript;
        private string _outputConfig;
        
        private SearchField _searchField;
        private TreeViewState _treeViewState;
        private ConfigTreeView _configTreeView;
        
        public void OnEnable()
        {
            _selectAll = "选择所有";
            _unSelectAll = "取消选择";
            _outputScript = "1.导出脚本";
            _outputConfig = "2.导出配置";
            _searchField = new SearchField();
            _treeViewState = new TreeViewState();
            _configTreeView = new ConfigTreeView(_treeViewState, new MultiColumnHeader(ConfigTreeView.CreateAssetMultiColumnHeaderState()));
            _configTreeView.OnEnable();
            _searchField.downOrUpArrowKeyPressed += _configTreeView.SetFocusAndEnsureSelectedItem;
        }

        public void OnReload()
        {
            _configTreeView.Reload();
        }
        
        public void OnGUI(Rect position)
        {
            Rect searchFieldRect = new Rect(3f, 40f, position.width - 6f, 20f);
            Rect configTreeViewRect = new Rect(3f, 60f, position.width - 6f, position.height - 166f);
            _configTreeView.searchString = _searchField.OnGUI(searchFieldRect,_configTreeView.searchString);
            _configTreeView?.OnGUI(configTreeViewRect);
            GUILayout.Space(position.height - 136f);
            if (GUILayout.Button(_selectAll))
            {
                _configTreeView?.IsSelectAll(true);
            }
            if (GUILayout.Button(_unSelectAll))
            {
                _configTreeView?.IsSelectAll(false);
            }
            if (GUILayout.Button(_outputScript))
            {
                List<string> selectSheets = _configTreeView?.GetSelectSheets();
                if (selectSheets != null && selectSheets.Count == 0)
                {
                    EditorUtility.DisplayDialog("提示","请选择导出的表格","确定");
                    return;
                }
                ConfigController.Instance.OutputScript(selectSheets);
            }
            if (GUILayout.Button(_outputConfig))
            {
                List<string> selectSheets = _configTreeView?.GetSelectSheets();
                if (selectSheets != null && selectSheets.Count == 0)
                {
                    EditorUtility.DisplayDialog("提示","请选择导出的表格","确定");
                    return;
                }
                ConfigController.Instance.OutputConfig(selectSheets);
            }
        }
        
        public void OnDestroy()
        {

        }
    }
}
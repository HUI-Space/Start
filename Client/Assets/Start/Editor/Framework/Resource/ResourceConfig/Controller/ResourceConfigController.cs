using System;
using System.Collections.Generic;

namespace Start.Editor
{
    public class ResourceConfigController
    {
        public Action OnChooseChange;
        public ChooseType ChooseType { get; private set; }
        public int ChooseHashCode { get; private set; }
        public ResourceGroup ChooseResourceGroup { get; private set; }
        public ResourceGroupItem ChooseResourceGroupItem { get; private set; }

        private ResourceConfigModel _resourceConfigModel;

        public void OnEnable()
        {
            _resourceConfigModel = new ResourceConfigModel();
            _resourceConfigModel.OnEnable();
        }

        public void OnDestroy()
        {
            _resourceConfigModel.OnDestroy();
            OnChooseChange = default;
            ChooseHashCode = default;
            ChooseResourceGroup = default;
            ChooseResourceGroupItem = default;
            _resourceConfigModel = default;
        }

        #region 相关操作

        public Dictionary<int, ResourceGroup> GetGroups()
        {
            return _resourceConfigModel.GetGroups();
        }

        public ResourceGroup GetGroup(int hashcode)
        {
            return _resourceConfigModel.GetGroup(hashcode);
        }

        public void RemoveGroup(int hashcode)
        {
            _resourceConfigModel.RemoveGroup(hashcode);
        }


        public ResourceGroup CreateGroup(string groupName)
        {
            return _resourceConfigModel.CreateGroup(groupName);
        }

        public bool RenameGroup(int hashcode, string newName)
        {
            return _resourceConfigModel.RenameGroup(hashcode, newName);
        }

        public ResourceGroupItem CreateGroupItem(int parentHashcode)
        {
            return _resourceConfigModel.CreateGroupItem(parentHashcode);
        }

        public ResourceGroupItem GetGroupItem(int hashCode)
        {
            return _resourceConfigModel.GetGroupItem(hashCode);
        }

        public void RemoveGroupItem(int parentHashCode, int hashCode)
        {
            _resourceConfigModel.RemoveGroupItem(parentHashCode, hashCode);
        }

        #endregion

        public void Reload()
        {
            _resourceConfigModel.Reload();
        }

        public void SaveConfig()
        {
            _resourceConfigModel.SaveConfig();
        }

        public void ClearEmpty()
        {
            _resourceConfigModel.ClearEmpty();
        }

        public void SelectionChanged(ChooseType chooseType, int hashCode)
        {
            if (ChooseHashCode == hashCode) return;
            ChooseHashCode = hashCode;
            ChooseType = chooseType;
            switch (chooseType)
            {
                case ChooseType.ResourceGroup:
                    ChooseResourceGroup = GetGroup(ChooseHashCode);
                    break;
                case ChooseType.ResourceGroupItem:
                    ChooseResourceGroupItem = GetGroupItem(ChooseHashCode);
                    break;
            }
            OnChooseChange?.Invoke();
        }
    }
}
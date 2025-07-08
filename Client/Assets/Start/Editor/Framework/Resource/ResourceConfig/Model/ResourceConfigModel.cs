using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Start.Editor
{
    public class ResourceConfigModel
    {
        private Dictionary<int, ResourceGroup> _groupConfigs;
        
        public void OnEnable()
        {
            Reload();
        }

        public void OnDestroy()
        {
            _groupConfigs.Clear();
            _groupConfigs = default;
        }

        #region 添加或移除配置

        public Dictionary<int, ResourceGroup> GetGroups()
        {
            return _groupConfigs;
        }

        public ResourceGroup GetGroup(int hashCode)
        {
            return _groupConfigs.TryGetValue(hashCode ,out ResourceGroup resourceGroup ) ? resourceGroup : default;
        }

        public ResourceGroup CreateGroup(string groupName)
        {
            foreach (ResourceGroup config in _groupConfigs.Values)
            {
                if (config.Name == groupName)
                {
                    return default;
                }
            }
            ResourceGroup resourceGroup = new ResourceGroup
            {
                Name = groupName
            };
            _groupConfigs.Add(resourceGroup.GetHashCode(),resourceGroup);
            return resourceGroup;
        }

        public void RemoveGroup(int hashCode)
        {
            if (!_groupConfigs.TryGetValue(hashCode, out ResourceGroup groupConfig)) return;
            groupConfig.GroupItems.Clear();
            _groupConfigs.Remove(hashCode);
        }

        public bool RenameGroup(int hashCode, string newName)
        {
            ResourceGroup resourceGroup = GetGroup(hashCode);
            if (resourceGroup == null) return false;
            resourceGroup.Name = newName;
            return true;
        }

        public ResourceGroupItem CreateGroupItem(int parentHashCode)
        {
            ResourceGroup resourceGroup = GetGroup(parentHashCode);
            if (resourceGroup == null) return default;
            ResourceGroupItem resourceGroupItem = new ResourceGroupItem();
            resourceGroup.GroupItems.Add(resourceGroupItem);
            return resourceGroupItem;
        }

        public ResourceGroupItem GetGroupItem(int hashCode)
        {
            foreach (ResourceGroup resourceGroup in _groupConfigs.Values)
            {
                foreach (ResourceGroupItem groupGroupItem in resourceGroup.GroupItems)
                {
                    if (groupGroupItem.GetHashCode() == hashCode)
                    {
                        return groupGroupItem;
                    }
                }
            }
            return default;
        }

        public void RemoveGroupItem(int parentHashCode,int hashCode)
        {
            ResourceGroup resourceGroup = GetGroup(parentHashCode);
            if (resourceGroup == null) return;
            ResourceGroupItem resourceGroupItem = resourceGroup.GroupItems.Find(item => item.GetHashCode() == hashCode);
            if (resourceGroupItem == null) return;
            resourceGroup.GroupItems.Remove(resourceGroupItem);
        }

        public void ClearEmpty()
        {
            List<int> groups = new List<int>();
            foreach (var item in _groupConfigs)
            {
                if (item.Value.GroupItems.Count == 0)
                {
                    groups.Add(item.Key);
                }
            }

            foreach (var hashCode in groups)
            {
                _groupConfigs.Remove(hashCode);
            }
        }
        
        #endregion

        #region 加载 保存 配置

        public void Reload()
        {
            _groupConfigs = null;
            ResourceGroupConfig resourceGroupConfig = Utility.LoadJsonConfig<ResourceGroupConfig>(ResourcePath.ResourceGroupConfigPath);
            _groupConfigs = resourceGroupConfig != null
                ? resourceGroupConfig.Groups.ToDictionary(group => group.GetHashCode())
                : new Dictionary<int, ResourceGroup>();
        }
        

        public void SaveConfig()
        {
            ResourceGroupConfig groupConfig = new ResourceGroupConfig();
            groupConfig.Groups.AddRange(_groupConfigs.Values);
            FileUtility.WriteAllBytes(ResourcePath.ResourceGroupConfigPath, JsonUtility.ToJson(groupConfig,true));
            AssetDatabase.Refresh();
        }

        #endregion
        
    }
}
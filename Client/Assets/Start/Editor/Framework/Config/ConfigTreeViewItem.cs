using UnityEditor.IMGUI.Controls;

namespace Start.Editor
{
    public class ConfigTreeViewItem : TreeViewItem
    {
        public bool Enable;
        public string FullName;
        
        public ConfigTreeViewItem(int id, int depth, bool enable, string name, string fullName) : base(id, depth, name)
        {
            Enable = enable;
            FullName = fullName;
        }
    }
}
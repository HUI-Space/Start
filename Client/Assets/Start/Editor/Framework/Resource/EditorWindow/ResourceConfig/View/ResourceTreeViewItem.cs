using System.IO;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace Start.Editor
{
    public class ResourceTreeViewItem : TreeViewItem
    {
        public string Name { get; private set; }
        public string FullName { get; private set; }
        public string[] Dependencies { get; private set; }
        public Texture2D Icon { get; private set; }
        
        public void OnEnable(int Id, string fullName)
        {
            id = Id;
            Name = Path.GetFileName(fullName);
            FullName = fullName;
            Dependencies = Utility.GetAllDependencies(fullName).ToArray();
            Icon = (Texture2D)AssetDatabase.GetCachedIcon(fullName);
        }
    }
}
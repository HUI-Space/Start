using UnityEditor;
using UnityEngine;

namespace Start.Editor
{
    public class BattleEditor : UnityEditor.Editor
    {

        [MenuItem("Start/Battle/GenerateComponent")]
        public static void GenerateComponent()
        {
            if (EditorApplication.isCompiling)
            {
                Debug.LogError("脚本真正编译，请等待编译完成在生成组件代码");
                return;
            }
            
            
            
        }
        
    }
}
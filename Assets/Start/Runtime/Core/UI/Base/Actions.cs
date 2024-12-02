using System.Threading.Tasks;
using Start.Framework;

namespace Start.Runtime
{
    
    /// <summary>
    /// 常用API
    /// </summary>
    public static partial class Actions
    {
        public static Task OpenUI(string name)
        {
            return UIAction.Create(name, ActionType.OpenUI).Dispatch();
        }

        public static Task CloseUI(string name)
        {
            return UIAction.Create(name, ActionType.CloseUI).Dispatch();
        }
        
        public static Task GoBackUI(string name)
        {
            return UIAction.Create(name, ActionType.GoBackUI).Dispatch();
        }
    }
}
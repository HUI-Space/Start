using System.Threading.Tasks;

namespace Start
{
    public static partial class UIActionTypes
    {
        
    }
    public static partial class UIActions
    {
        public static Task ScenePanel_Open(int state)
        {
            return UIAction.Create(nameof(ScenePanel), UIActionTypes.Open).SetData(state).Dispatch();
        }
    }
}
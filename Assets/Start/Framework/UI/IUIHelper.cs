using System.Collections.Generic;
using System.Threading.Tasks;

namespace Start.Framework
{
    public interface IUIHelper
    {
        List<Middleware> GetMiddlewares();
        Task<GenericData<bool,EUIType,IUIBase, IUIData>> OpenUI(UIAction uiAction);
        void CloseUI(IUIBase uiBase, IUIData uiData);
    }
}
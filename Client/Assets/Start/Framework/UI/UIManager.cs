using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Start
{
    /// <summary>
    /// UI中间件（单项数据流）
    /// </summary>
    public delegate Task UIMiddleware(UIAction uiAction);
    
    public partial class UIManager : ManagerBase<UIManager>
    {
        public override int Priority => 12;
        private IUIHelper _iUIHelper;
        private List<UIMiddleware> _middlewares;
        
        public override Task Initialize()
        {
            _middlewares = new List<UIMiddleware>();
            _iUIHelper = Helper.CreateHelper<IUIHelper>();
            _middlewares.AddRange(_iUIHelper.GetMiddlewares());
            return base.Initialize();
        }
        
        public async Task Dispatch(UIAction uiAction)
        {
            try
            {
                foreach (UIMiddleware middleware in _middlewares)
                {
                    var task = middleware(uiAction);
                    if (task != null)
                        await task;
                }
            }
            catch (Exception e)
            {
                Logger.Error($"UI中间件 :{e}");
            }
        }

        public override Task DeInitialize()
        {
            _middlewares.Clear();
            _middlewares = null;
            _iUIHelper = null;
            return base.DeInitialize();
        }
    }
}
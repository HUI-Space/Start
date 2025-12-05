using System.Collections.Generic;
using System.Threading.Tasks;

namespace Start
{
    public class UIHelper : IUIHelper
    {
        public List<UIMiddleware> GetMiddlewares()
        {
            List<UIMiddleware> middlewares = new List<UIMiddleware>()
            {
                //按照需求顺序添加中间件
                UIWindow.Instance.UIMiddleware,
                UILoading.Instance.UIMiddleware,
                UITips.Instance.UIMiddleware,
                DefaultDispatch,
            };

            return middlewares;
        }
        
        private Task DefaultDispatch(UIAction uiAction)
        {
            //回收UIAction
            Logger.Info($"回收UIAction UIName:{uiAction.UIName} 事件类型：{uiAction.ActionType}");
            RecyclableObjectPool.Recycle(uiAction);
            return Task.CompletedTask;
        }
    }
}
using System.Threading.Tasks;

namespace Start
{
    public static partial class UIActions
    {
        /// <summary>
        /// 打开UI
        /// </summary>
        /// <param name="name">UI名称</param>
        /// <returns></returns>
        public static Task OpenUI(string name)
        {
            return UIAction.Create(name, UIActionTypes.Open).Dispatch();
        }

        /// <summary>
        /// 刷新UI
        /// </summary>
        /// <param name="name">UI名称</param>
        /// <returns></returns>
        public static Task RenderUI(string name)
        {
            return UIAction.Create(name, UIActionTypes.Render).Dispatch();
        }

        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <param name="name">UI名称</param>
        /// <returns></returns>
        public static Task CloseUI(string name)
        {
            return UIAction.Create(name, UIActionTypes.Close).Dispatch();
        }
        
        /// <summary>
        /// 关闭UI
        /// </summary>
        /// <returns></returns>
        public static Task CloseAllUI()
        {
            return UIAction.Create(UIActionTypes.CloseAll).Dispatch();
        }

        /// <summary>
        /// 回退到UI
        /// </summary>
        /// <param name="name">UI名称</param>
        /// <returns></returns>
        public static Task GoBackUI(string name)
        {
            return UIAction.Create(name, UIActionTypes.Back).Dispatch();
        }

        /// <summary>
        /// 显示提示
        /// </summary>
        /// <param name="message">提示信息</param>
        /// <param name="durationTime">持续时间</param>
        /// <param name="isThroughAll">射线是否穿透下方所有物体</param>
        /// <param name="throughCount">射线穿透下方物体个数</param>
        /// <returns></returns>
        public static Task ShowTips(string message, float durationTime = 2f, bool isThroughAll = false, int throughCount = 0)
        {
            return UIAction.Create(UIActionTypes.ShowTips).SetData(message, durationTime, isThroughAll, throughCount)
                .Dispatch();
        }

        public static Task ShowLoading(string path = "")
        {
            return UIAction.Create(UIActionTypes.ShowLoading).SetData(path).Dispatch();
        }
        
        public static Task HideLoading()
        {
            return UIAction.Create(UIActionTypes.HideLoading).Dispatch();
        }

        public static Task UpdateLoadingProgress(float progress)
        {
            return UIAction.Create(UIActionTypes.UpdateLoadingProgress).SetData(progress).Dispatch();
        }
    }
}
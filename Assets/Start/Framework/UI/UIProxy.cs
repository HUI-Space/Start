namespace Start.Framework
{
    public class UIProxy:IReference
    {
        public string UIName { get; private set; }
        public EUIType UIType { get; private set; }
        public IUIData UIData { get; private set; }
        public IUIBase UIBase { get; private set; }
        
        public static UIProxy Create(string uiName,EUIType euiType,IUIBase uiBase,IUIData uiData)
        {
            UIProxy uiProxy = ReferencePool.Acquire<UIProxy>();
            uiProxy.UIName = uiName;
            uiProxy.UIType = euiType;
            uiProxy.UIBase = uiBase;
            uiProxy.UIData = uiData;
            return uiProxy;
        }
        
        public void Clear()
        {
            UIName = default;
            UIBase = default;
            UIData = default;
        }
    }
}
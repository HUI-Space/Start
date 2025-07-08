namespace Start
{
    public class UIProxy : IReference
    {
        public string UIName { get; private set; }
        public string UIDataName { get; private set; }
        public EUIType UIType { get; private set; }
        public IUIData UIData { get; private set; }
        public IUIBase UIBase { get; private set; }//父节点
        public IUIBase[] AllUIBases { get; private set; }//包含父节点
        
        public static UIProxy Create(string uiName, EUIType euiType, IUIBase uiBase, IUIData uiData, IUIBase[] allUIBases)
        {
            UIProxy uiProxy = ReferencePool.Acquire<UIProxy>();
            uiProxy.UIName = uiName;
            uiProxy.UIType = euiType;
            uiProxy.UIBase = uiBase;
            uiProxy.UIData = uiData;
            uiProxy.AllUIBases = allUIBases;
            uiProxy.UIDataName = uiData.GetType().Name;
            return uiProxy;
        }

        public void Clear()
        {
            UIName = default;
            UIType = default;
            UIBase = default;
            UIData = default;
            UIDataName = default;
            AllUIBases = default;
        }
    }
}
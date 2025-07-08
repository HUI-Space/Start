namespace Start
{
    public interface IUIData
    {
        /// <summary>
        /// 是否脏数据
        /// </summary>
        bool IsDirty { get; }
        
        /// <summary>
        /// UI名称
        /// </summary>
        string UIName { get; }
        
        /// <summary>
        /// 排序
        /// </summary>
        int Order { get; set; }
        
        void Initialize();
        
        void ReceiveAction(UIAction uiAction);
        
        void BeforeClearDirty();
        
        void ClearDirty();
        
        void DeInitialize();
    }
}
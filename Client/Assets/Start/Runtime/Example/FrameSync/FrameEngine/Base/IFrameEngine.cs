namespace Start
{
    public interface IFrameEngine
    {
        /// <summary>
        /// 运行中
        /// </summary>
        bool Running { get; }
        
        /// <summary>
        /// 启动引擎
        /// </summary>
        void StartEngine();
        
        /// <summary>
        /// 停止引擎
        /// </summary>
        void StopEngine();
        
        /// <summary>
        /// 暂停
        /// </summary>
        void Pause();
        
        /// <summary>
        /// 恢复
        /// </summary>
        void Resume();

        /// <summary>
        /// 渲染更新
        /// </summary>
        void RenderEngineUpdate();
    }
}
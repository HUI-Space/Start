namespace Start
{
    /// <summary>
    /// 响应接口
    /// </summary>
    public interface IResponse : IMessage
    {
        /// <summary>
        /// 序列号
        /// </summary>
        int SN { get; set; }
        
        /// <summary>
        /// 消息码
        /// </summary>
        int Code { get; }
        
        /// <summary>
        /// 错误信息
        /// </summary>
        string Error { get; }
    }
}
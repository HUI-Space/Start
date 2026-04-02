namespace Start
{
    /// <summary>
    /// 请求接口
    /// </summary>
    public interface IRequest : IMessage
    {
        /// <summary>
        /// 序列号
        /// </summary>
        int SN { get; set; }
    }
}
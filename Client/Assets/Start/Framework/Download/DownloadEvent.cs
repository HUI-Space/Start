

namespace Start
{
    public class DownloadEvent : IReference
    {
        /// <summary>
        /// 任务的序列编号
        /// </summary>
        public int SerialId { get; private set; }

        /// <summary>
        /// 下载任务的状态
        /// </summary>
        public EDownloadStatusType StatusType { get; private set; }

        /// <summary>
        /// 获取当前大小
        /// </summary>
        public long CurrentLength { get; private set; }

        /// <summary>
        /// 获取下载后存放路径
        /// </summary>
        public string DownloadPath { get; private set; }

        /// <summary>
        /// 获取下载地址
        /// </summary>
        public string DownloadUri { get; private set; }

        /// <summary>
        /// 获取错误信息
        /// </summary>
        public string ErrorMessage { get; private set; }

        /// <summary>
        /// 获取用户自定义数据
        /// </summary>
        public object UserData { get; private set; }

        /// <summary>
        /// 创建下载事件
        /// </summary>
        /// <param name="serialId">任务的序列编号</param>
        /// <param name="statusType">下载状态</param>
        /// <param name="currentLength">获取当前大小</param>
        /// <param name="downloadPath">获取下载后存放路径</param>
        /// <param name="downloadUri">获取下载地址</param>
        /// <param name="ErrorMessage">获取错误信息</param>
        /// <param name="userData">获取用户自定义数据</param>
        /// <returns></returns>
        public static DownloadEvent Create(int serialId, EDownloadStatusType statusType, long currentLength, string downloadPath,
            string downloadUri,
            string ErrorMessage, 
            object userData)
        {
            DownloadEvent @event = ReferencePool.Acquire<DownloadEvent>();
            @event.SerialId = serialId;
            @event.StatusType = statusType;
            @event.CurrentLength = currentLength;
            @event.DownloadPath = downloadPath;
            @event.DownloadUri = downloadUri;
            @event.ErrorMessage = ErrorMessage;
            @event.UserData = userData;
            return @event;
        }

        public void Clear()
        {
            SerialId = default;
            StatusType = default;
            CurrentLength = default;
            DownloadPath = default;
            DownloadUri = default;
            ErrorMessage = default;
            UserData = default;
        }
    }
}
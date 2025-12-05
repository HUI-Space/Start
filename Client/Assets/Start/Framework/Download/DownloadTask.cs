

namespace Start
{
    public class DownloadTask:TaskBase
    {
        private static int s_Serial = 0;
        public EDownloadStatusType Status { get; set; }
        public string DownloadPath { get; set; }
        public string DownloadUri { get; set; }
        public int FlushSize { get; set; }
        public float Timeout { get; set; }
        
        /// <summary>
        /// 创建下载任务。
        /// </summary>
        /// <param name="downloadSavePath">下载后存放路径。</param>
        /// <param name="downloadUri">原始下载地址。</param>
        /// <param name="tag">下载任务的标签。</param>
        /// <param name="priority">下载任务的优先级。</param>
        /// <param name="flushSize">将缓冲区写入磁盘的临界大小。</param>
        /// <param name="timeout">下载超时时长，以秒为单位。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns></returns>
        public static DownloadTask Create(string downloadSavePath, string downloadUri, string tag, int priority, int flushSize, float timeout, object userData)
        {
            DownloadTask downloadTask = RecyclableObjectPool.Acquire<DownloadTask>();
            downloadTask.Initialize(++s_Serial, priority,tag, userData);
            downloadTask.DownloadPath = downloadSavePath;
            downloadTask.DownloadUri = downloadUri;
            downloadTask.FlushSize = flushSize;
            downloadTask.Timeout = timeout;
            return downloadTask;
        }

        public override void Reset()
        {
            base.Reset();
            Status = EDownloadStatusType.Todo;
            DownloadPath = null;
            DownloadUri = null;
            FlushSize = 0;
            Timeout = 0f;
        }
    }
}
using System;
using System.Threading.Tasks;

namespace Start
{
    public class DownloadManager : ManagerBase<DownloadManager>, IUpdateManger
    {
        private const int OneMegaBytes = 1024 * 1024;
        public override int Priority => 5;

        /// <summary>
        /// 获取或设置将缓冲区写入磁盘的临界大小。
        /// </summary>
        public int FlushSize;

        /// <summary>
        /// 获取或设置下载超时时长，以秒为单位。
        /// </summary>
        public float Timeout;

        /// <summary>S
        /// 获取或设置下载是否被暂停。
        /// </summary>
        public bool Paused
        {
            get => _taskPool.Paused;
            set => _taskPool.Paused = value;
        }

        /// <summary>
        /// 获取当前下载速度。
        /// </summary>
        public float CurrentSpeed => _downloadCounter.CurrentSpeed;

        /// <summary>
        /// 获取下载代理总数量。
        /// </summary>
        public int TotalAgentCount => _taskPool.TotalAgentCount;

        /// <summary>
        /// 获取等待下载任务数量。
        /// </summary>
        public int WaitingTaskCount => _taskPool.WaitingTaskCount;

        /// <summary>
        /// 获取工作中下载代理数量。
        /// </summary>
        public int WorkingAgentCount => _taskPool.WorkingAgentCount;

        /// <summary>
        /// 获取可用下载代理数量。
        /// </summary>
        public int FreeAgentCount => _taskPool.FreeAgentCount;

        public Action<DownloadEvent> DownloadEventHandler;

        private TaskPool<DownloadTask> _taskPool;
        private DownloadCounter _downloadCounter;

        public override Task Initialize()
        {
            _downloadCounter = new DownloadCounter(1f, 10f);
            _taskPool = new TaskPool<DownloadTask>();
            FlushSize = OneMegaBytes;
            Timeout = 30f;
            
            IDownloadHelper downloadHelper = Helper.CreateHelper<IDownloadHelper>();
            for (int i = 0; i < downloadHelper.DownloadAgentHelperCount; i++)
            {
                IDownloadAgentHelper downloadAgentHelper =
                    (IDownloadAgentHelper)Activator.CreateInstance(downloadHelper.DownloadAgentHelperType);
                Instance.AddDownloadAgentHelper(downloadAgentHelper);
            }
            return Task.CompletedTask;
        }

        public override Task DeInitialize()
        {
            _taskPool.DeInitialize();
            _downloadCounter.DeInitialize();
            _downloadCounter = default;
            _taskPool = default;
            FlushSize = default;
            Timeout = default;
            return Task.CompletedTask;
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            _taskPool.Update(elapseSeconds, realElapseSeconds);
            _downloadCounter.Update(elapseSeconds, realElapseSeconds);
        }

        /// <summary>
        /// 增加下载代理辅助器。
        /// </summary>
        /// <param name="downloadAgentHelper">要增加的下载代理辅助器。</param>
        private void AddDownloadAgentHelper(IDownloadAgentHelper downloadAgentHelper)
        {
            DownloadAgent agent = new DownloadAgent(downloadAgentHelper);
            agent.DownloadAgentStart += OnDownloadAgentStart;
            agent.DownloadAgentUpdate += OnDownloadAgentUpdate;
            agent.DownloadAgentSuccess += OnDownloadAgentSuccess;
            agent.DownloadAgentFailure += OnDownloadAgentFailure;
            _taskPool.AddAgent(agent);
        }

        #region 任务接口

        /// <summary>
        /// 增加下载任务。
        /// </summary>
        /// <param name="downloadPath">下载后存放路径。</param>
        /// <param name="downloadUri">原始下载地址。</param>
        /// <param name="tag">下载任务的标签。</param>
        /// <param name="priority">下载任务的优先级。</param>
        /// <param name="userData">用户自定义数据。</param>
        /// <returns>新增下载任务的序列编号。</returns>
        public int AddDownload(string downloadPath, string downloadUri, string tag, int priority, object userData)
        {
            if (string.IsNullOrEmpty(downloadPath))
            {
                throw new Exception("下载路径为空.");
            }

            if (string.IsNullOrEmpty(downloadUri))
            {
                throw new Exception("下载 uri 为空.");
            }

            if (_taskPool.TotalAgentCount <= 0)
            {
                throw new Exception("您必须先添加下载代理.");
            }

            DownloadTask downloadTask =
                DownloadTask.Create(downloadPath, downloadUri, tag, priority, FlushSize, Timeout, userData);
            _taskPool.AddTask(downloadTask);
            return downloadTask.SerialId;
        }

        /// <summary>
        /// 根据下载任务的序列编号移除下载任务。
        /// </summary>
        /// <param name="serialId">要移除下载任务的序列编号。</param>
        /// <returns>是否移除下载任务成功。</returns>
        public bool RemoveDownload(int serialId)
        {
            return _taskPool.RemoveTask(serialId);
        }

        /// <summary>
        /// 根据下载任务的标签移除下载任务。
        /// </summary>
        /// <param name="tag">要移除下载任务的标签。</param>
        /// <returns>移除下载任务的数量。</returns>
        public int RemoveDownloads(string tag)
        {
            return _taskPool.RemoveTasks(tag);
        }

        /// <summary>
        /// 移除所有下载任务。
        /// </summary>
        /// <returns>移除下载任务的数量。</returns>
        public int RemoveAllDownloads()
        {
            return _taskPool.RemoveAllTasks();
        }

        #endregion

        #region 任务回调事件
        
        private void OnDownloadAgentStart(DownloadAgent downloadAgent)
        {
            DownloadEvent downloadEvent = DownloadEvent.Create(downloadAgent.Task.SerialId, EDownloadStatusType.Todo,
                downloadAgent.StartLength, downloadAgent.Task.DownloadPath, downloadAgent.Task.DownloadUri, null,
                downloadAgent.Task.UserData);
            DownloadEventHandler?.Invoke(downloadEvent);
            RecyclableObjectPool.Recycle(downloadEvent);
        }

        private void OnDownloadAgentUpdate(DownloadAgent downloadAgent, int deltaLength)
        {
            _downloadCounter.RecordDeltaLength(deltaLength);
            DownloadEvent downloadEvent = DownloadEvent.Create(downloadAgent.Task.SerialId, EDownloadStatusType.Doing,
                downloadAgent.StartLength, downloadAgent.Task.DownloadPath, downloadAgent.Task.DownloadUri, null,
                downloadAgent.Task.UserData);
            DownloadEventHandler?.Invoke(downloadEvent);
            RecyclableObjectPool.Recycle(downloadEvent);
        }

        private void OnDownloadAgentSuccess(DownloadAgent downloadAgent, long length)
        {
            DownloadEvent downloadEvent = DownloadEvent.Create(downloadAgent.Task.SerialId, EDownloadStatusType.Done,
                downloadAgent.StartLength, downloadAgent.Task.DownloadPath, downloadAgent.Task.DownloadUri, null,
                downloadAgent.Task.UserData);
            DownloadEventHandler?.Invoke(downloadEvent);
            RecyclableObjectPool.Recycle(downloadEvent);
        }

        private void OnDownloadAgentFailure(DownloadAgent downloadAgent, string errorMessage)
        {
            DownloadEvent downloadEvent = DownloadEvent.Create(downloadAgent.Task.SerialId, EDownloadStatusType.Error,
                downloadAgent.StartLength, downloadAgent.Task.DownloadPath, downloadAgent.Task.DownloadUri, errorMessage, 
                downloadAgent.Task.UserData);
            DownloadEventHandler?.Invoke(downloadEvent);
            RecyclableObjectPool.Recycle(downloadEvent);
        }
        
        #endregion
    }
}
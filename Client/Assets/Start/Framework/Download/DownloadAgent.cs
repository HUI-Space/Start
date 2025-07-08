using System;
using System.IO;


namespace Start
{
    public class DownloadAgent: ITaskAgent<DownloadTask>, IDisposable
    {
        private const string DownloadExtension = ".download";

        public Action<DownloadAgent> DownloadAgentStart;
        public Action<DownloadAgent, int> DownloadAgentUpdate;
        public Action<DownloadAgent, long> DownloadAgentSuccess;
        public Action<DownloadAgent, string> DownloadAgentFailure;
        
        public DownloadTask Task { get; private set; }
        
        /// <summary>
        /// 获取已经等待时间。
        /// </summary>
        public float WaitTime { get; private set; }
        /// <summary>
        /// 获取开始下载时已经存在的大小。
        /// </summary>
        public long StartLength { get; private set; }
        /// <summary>
        /// 获取本次已经下载的大小。
        /// </summary>
        public long DownloadedLength { get; private set; }
        /// <summary>
        /// 获取已经存盘的大小。
        /// </summary>
        public long SavedLength { get; private set; }

        private readonly IDownloadAgentHelper _iDownloadAgentHelper;
        private FileStream _fileStream;
        private int WaitFlushSize;
        private bool _disposed;
        
        public DownloadAgent(IDownloadAgentHelper iDownloadAgentHelper)
        {
            _iDownloadAgentHelper = iDownloadAgentHelper;
        }
        
        public void Initialize()
        {
            _iDownloadAgentHelper.DownloadUpdateBytes += OnDownloadUpdateBytes;
            _iDownloadAgentHelper.DownloadProgress += OnDownloadProgress;
            _iDownloadAgentHelper.DownloadComplete += OnDownloadComplete;
            _iDownloadAgentHelper.DownloadError += OnDownloadError;
        }
        
        public EStartTaskStatus Start(DownloadTask task)
        {
            if (task == null)
            {
                throw new Exception("任务为空.");
            }

            Task = task;
            Task.Status = EDownloadStatusType.Doing;
            string downloadFile = Path.ChangeExtension(task.DownloadPath, DownloadExtension);
            try
            {
                if (File.Exists(downloadFile))
                {
                    _fileStream = File.OpenWrite(downloadFile);
                    _fileStream.Seek(0L, SeekOrigin.End);
                    StartLength = SavedLength = _fileStream.Length;
                    DownloadedLength = 0L;
                }
                else
                {
                    string directory = Path.GetDirectoryName(Task.DownloadPath);
                    if (!Directory.Exists(directory))
                    {
                        if (directory != null) Directory.CreateDirectory(directory);
                    }

                    _fileStream = new FileStream(downloadFile, FileMode.Create, FileAccess.Write);
                    StartLength = SavedLength = DownloadedLength = 0L;
                }
                
                if (DownloadAgentStart != null)
                {
                    DownloadAgentStart(this);
                }
                
                if (StartLength > 0L)
                {
                    _iDownloadAgentHelper.Download(Task.DownloadUri, StartLength, Task.UserData);
                }
                else
                {
                    _iDownloadAgentHelper.Download(Task.DownloadUri, Task.UserData);
                }

                return EStartTaskStatus.CanResume;
            }
            catch (Exception e)
            {
                OnDownloadError(e.ToString());
                return EStartTaskStatus.UnknownError;
            }
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (Task.Status == EDownloadStatusType.Doing)
            {
                WaitTime += realElapseSeconds;
                _iDownloadAgentHelper.Update(elapseSeconds,realElapseSeconds);
                if (WaitTime >= Task.Timeout)
                {
                    OnDownloadError("Download timeout.");
                }
            }
        }

        public void Reset()
        {
            _iDownloadAgentHelper.Reset();
            
            if (_fileStream != null)
            {
                _fileStream.Close();
                _fileStream = null;
            }

            Task = null;
            WaitFlushSize = 0;
            WaitTime = 0f;
            StartLength = 0L;
            DownloadedLength = 0L;
            SavedLength = 0L;
        }

        public void DeInitialize()
        {
            Dispose();
            _iDownloadAgentHelper.DownloadUpdateBytes -= OnDownloadUpdateBytes;
            _iDownloadAgentHelper.DownloadProgress -= OnDownloadProgress;
            _iDownloadAgentHelper.DownloadComplete -= OnDownloadComplete;
            _iDownloadAgentHelper.DownloadError -= OnDownloadError;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        
        /// <summary>
        /// 释放资源。
        /// </summary>
        /// <param name="disposing">释放资源标记。</param>
        private void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (_fileStream != null)
                {
                    _fileStream.Dispose();
                    _fileStream = null;
                }
            }

            _disposed = true;
        }

        private void OnDownloadUpdateBytes(byte[] bytes, int offset, int length)
        {
            WaitTime = 0f;
            try
            {
                _fileStream.Write(bytes, offset, length);
                WaitFlushSize += length;
                SavedLength += length;

                if (WaitFlushSize >= Task.FlushSize)
                {
                    _fileStream.Flush();
                    WaitFlushSize = 0;
                }
            }
            catch (Exception exception)
            {
                OnDownloadError(exception.ToString());
            }
        }

        private void OnDownloadProgress(int e)
        {
            WaitTime = 0f;
            DownloadedLength += e;
            if (DownloadAgentUpdate != null)
            {
                DownloadAgentUpdate(this, e);
            }
        }

        private void OnDownloadComplete(long e)
        {
            WaitTime = 0f;
            DownloadedLength = e;
            if (SavedLength != StartLength + DownloadedLength)
            {
                throw new Exception("内部下载错误.");
            }

            _iDownloadAgentHelper.Reset();
            _fileStream.Close();
            _fileStream = null;
            
            if (File.Exists(Task.DownloadPath))
            {
                File.Delete(Task.DownloadPath);
            }
            
            string downloadFile = Path.ChangeExtension(Task.DownloadPath, DownloadExtension);
            if (downloadFile != null) 
                File.Move(downloadFile, Task.DownloadPath);

            Task.Status = EDownloadStatusType.Done;
            
            if (DownloadAgentSuccess != null)
            {
                DownloadAgentSuccess(this, e);
            }
            
            Task.Done = true;
        }

        private void OnDownloadError(string e)
        {
            _iDownloadAgentHelper.Reset();
            if (_fileStream != null)
            {
                _fileStream.Close();
                _fileStream = null;
            }
            
            Task.Status = EDownloadStatusType.Error;
            
            if (DownloadAgentFailure != null)
            {
                DownloadAgentFailure(this, e);
            }
            
            Task.Done = true;
        }
    }
}
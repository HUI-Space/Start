using System;

using UnityEngine.Networking;

namespace Start
{
    public class DownloadAgentHelper:IDownloadAgentHelper,IDisposable
    {
        public event Action<byte[], int, int> DownloadUpdateBytes
        {
            add => _downloadUpdateBytes += value;
            remove => _downloadUpdateBytes -= value;
        }
        public event Action<int> DownloadProgress 
        {
            add => _downloadProgress += value;
            remove => _downloadProgress -= value;
        }
        public event Action<long> DownloadComplete
        {
            add => _downloadComplete += value;
            remove => _downloadComplete -= value;
        }
        public event Action<string> DownloadError
        {
            add => _downloadError += value;
            remove => _downloadError -= value;
        }
        
        private const int CachedBytesLength = 0x1000;
        private readonly byte[] _cachedBytes = new byte[CachedBytesLength];
        private UnityWebRequest _unityWebRequest;
        private bool _disposed;
        private event Action<byte[], int, int> _downloadUpdateBytes;
        private event Action<int> _downloadProgress;
        private event Action<long> _downloadComplete;
        private event Action<string> _downloadError;
        
        public void Download(string downloadUrl, object userData)
        {
            if (_downloadUpdateBytes == null || _downloadProgress == null || _downloadComplete == null || _downloadError == null)
            {
                return;
            }
            _unityWebRequest = new UnityWebRequest(downloadUrl);
            _unityWebRequest.downloadHandler = new DownloadHandler(this);
            _unityWebRequest.SendWebRequest();
        }
        
        public void Download(string downloadUrl, long fromPosition, object userData)
        {
            if (_downloadUpdateBytes == null || _downloadProgress == null || _downloadComplete == null || _downloadError == null)
            {
                return;
            }

            _unityWebRequest = new UnityWebRequest(downloadUrl);
            _unityWebRequest.SetRequestHeader("Range", $"bytes={fromPosition}-");
            _unityWebRequest.downloadHandler = new DownloadHandler(this);
            _unityWebRequest.SendWebRequest();
        }

        public void Reset()
        {
            if (_unityWebRequest != null)
            {
                _unityWebRequest.Abort();
                _unityWebRequest.Dispose();
                _unityWebRequest = null;
            }

            Array.Clear(_cachedBytes, 0, CachedBytesLength);
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
        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                if (_unityWebRequest != null)
                {
                    _unityWebRequest.Dispose();
                    _unityWebRequest = null;
                }
            }

            _disposed = true;
        }

        public void Update(float elapseSeconds, float realElapseSeconds)
        {
            if (_unityWebRequest == null)
            {
                return;
            }

            if (!_unityWebRequest.isDone)
            {
                return;
            }

            bool isError = false;
#if UNITY_2020_2_OR_NEWER
            isError = _unityWebRequest.result != UnityWebRequest.Result.Success;
#elif UNITY_2017_1_OR_NEWER
            isError = _unityWebRequest.isNetworkError || _unityWebRequest.isHttpError;
#else
            isError = _unityWebRequest.isError;
#endif
            if (isError)
            {
                _downloadError?.Invoke(_unityWebRequest.error);
            }
            else
            {
                _downloadComplete?.Invoke((long)_unityWebRequest.downloadedBytes);
            }
        }

        private sealed class DownloadHandler : DownloadHandlerScript
        {
            private readonly DownloadAgentHelper _owner;

            public DownloadHandler(DownloadAgentHelper owner)
                : base(owner._cachedBytes)
            {
                _owner = owner;
            }

            protected override bool ReceiveData(byte[] datas, int dataLength)
            {
                if (_owner?._unityWebRequest != null && dataLength > 0)
                {
                    _owner._downloadUpdateBytes?.Invoke(datas,0,dataLength);
                    _owner._downloadProgress?.Invoke(dataLength);
                }

                return base.ReceiveData(datas, dataLength);
            } 
        }
    }
}
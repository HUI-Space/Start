using System;

namespace Start
{
    public interface IDownloadAgentHelper
    {
        /// <summary>
        /// 返回这次请求时需要下载的大小（即剩余文件大小）
        /// byte[]下载的数据流。
        /// int 数据流的偏移。
        /// int 数据流的长度。
        /// </summary>
        event Action<byte[],int,int> DownloadUpdateBytes;
        
        /// <summary>
        /// 每次下载到数据后回调进度
        /// </summary>
        event Action<int> DownloadProgress;
        
        /// <summary>
        /// 当下载完成后回调下载的文件总大小
        /// </summary>
        event Action<long> DownloadComplete;
        
        /// <summary>
        /// 当下载中错误信息
        /// </summary>
        event Action<string> DownloadError;

        /// <summary>
        /// 通过下载代理辅助器下载指定地址的数据。
        /// </summary>
        /// <param name="downloadUrl">下载地址</param>
        /// <param name="userData">下载数据起始位置</param>
        void Download(string downloadUrl, object userData);
        
        /// <summary>
        /// 通过下载代理辅助器下载指定地址的数据。
        /// </summary>
        /// <param name="downloadUrl">下载地址。</param>
        /// <param name="fromPosition">下载数据起始位置。</param>
        /// <param name="userData">用户自定义数据。</param>
        void Download(string downloadUrl, long fromPosition, object userData);

        /// <summary>
        /// 更新下载代理辅助器。
        /// </summary>
        /// <param name="elapseSeconds">逻辑流逝时间，以秒为单位。</param>
        /// <param name="realElapseSeconds">真实流逝时间，以秒为单位。</param>
        void Update(float elapseSeconds, float realElapseSeconds);
        
        /// <summary>
        /// 重置下载辅助器
        /// </summary>
        void Reset();
    }
}
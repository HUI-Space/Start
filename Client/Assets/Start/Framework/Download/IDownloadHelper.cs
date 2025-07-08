using System;

namespace Start
{
    public interface IDownloadHelper
    {
        int DownloadAgentHelperCount { get; }
        Type DownloadAgentHelperType { get; }
    }
}
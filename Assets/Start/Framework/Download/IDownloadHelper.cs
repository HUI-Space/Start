using System;

namespace Start.Framework
{
    public interface IDownloadHelper
    {
        int DownloadAgentHelperCount { get; }
        Type DownloadAgentHelperType { get; }
    }
}
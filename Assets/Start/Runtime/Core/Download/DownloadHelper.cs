using System;
using Start.Framework;

namespace Start.Runtime
{
    public class DownloadHelper:IDownloadHelper
    {
        public int DownloadAgentHelperCount => 5;
        public Type DownloadAgentHelperType => typeof(DownloadAgentHelper);
    }
}
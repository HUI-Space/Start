using System;


namespace Start
{
    public class DownloadHelper : IDownloadHelper
    {
        public int DownloadAgentHelperCount => 5;
        public Type DownloadAgentHelperType => typeof(DownloadAgentHelper);
    }
}
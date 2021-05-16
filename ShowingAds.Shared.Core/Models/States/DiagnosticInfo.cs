using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.Shared.Core.Models.States
{
    public class DiagnosticInfo : ICloneable
    {
        public string Version { get; private set; }
        public int ReadyVideosCount { get; private set; }
        public int DownloadType { get; private set; }
        public int DownloadProgress { get; private set; }
        public double DownloadSpeed { get; private set; }

        public DiagnosticInfo(string version, int readyVideosCount, int downloadType, int downloadProgress, double downloadSpeed)
        {
            Version = version;
            ReadyVideosCount = readyVideosCount;
            DownloadType = downloadType;
            DownloadProgress = downloadProgress;
            DownloadSpeed = downloadSpeed;
        }

        public object Clone() => MemberwiseClone();
    }
}

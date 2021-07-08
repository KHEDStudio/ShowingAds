using Newtonsoft.Json;
using ShowingAds.Shared.Core.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.Shared.Core.Models.States
{
    public class DiagnosticInfo : ICloneable, IModel<Guid>
    {
        public Guid Id { get; set; }

        public DeviceStatus DeviceStatus { get; set; }

        public string Version { get; private set; }

        public int ReadyVideosCount { get; private set; }
        public Dictionary<string, int> ClientVideos { get; private set; }

        public Dictionary<string, int> ClientVideosShowed { get; private set; }
        public Dictionary<string, DateTime> ClientVideosWillShow { get; private set; }

        public int DownloadType { get; private set; }
        public int DownloadProgress { get; private set; }
        public double DownloadSpeed { get; private set; }

        public string CurrentVideo { get; private set; }
        public string Screenshot { get; set; }
        public double FreeSpaceDisk { get; private set; }

        public IEnumerable<string> Logs { get; private set; }

        [JsonConstructor]
        public DiagnosticInfo(Guid id, DeviceStatus deviceStatus, string version, int readyVideosCount, Dictionary<string, int> clientVideos, 
            Dictionary<string, int> clientVideosShowed, Dictionary<string, DateTime> clientVideosWillShow, int downloadType, int downloadProgress,
            double downloadSpeed, string currentVideo, string screenshot, double freeSpaceDisk, IEnumerable<string> logs)
        {
            Id = id;
            DeviceStatus = deviceStatus;
            Version = version ?? throw new ArgumentNullException(nameof(version));
            ReadyVideosCount = readyVideosCount;
            ClientVideos = clientVideos ?? new Dictionary<string, int>();
            ClientVideosShowed = clientVideosShowed ?? new Dictionary<string, int>();
            ClientVideosWillShow = clientVideosWillShow ?? new Dictionary<string, DateTime>();
            DownloadType = downloadType;
            DownloadProgress = downloadProgress;
            DownloadSpeed = downloadSpeed;
            CurrentVideo = currentVideo ?? string.Empty;
            Screenshot = screenshot ?? Guid.Empty.ToString();
            FreeSpaceDisk = freeSpaceDisk;
            Logs = logs ?? new List<string>();
        }

        public DiagnosticInfo(Guid id)
        {
            Id = id;
            DeviceStatus = DeviceStatus.Offline;
            Version = "1.0";
            ReadyVideosCount = 0;
            ClientVideos = new Dictionary<string, int>();
            ClientVideosShowed = new Dictionary<string, int>();
            ClientVideosWillShow = new Dictionary<string, DateTime>();
            DownloadType = 0;
            DownloadProgress = 0;
            DownloadSpeed = 0;
            CurrentVideo = string.Empty;
            Screenshot = Guid.Empty.ToString();
            FreeSpaceDisk = 0;
            Logs = new List<string>();
        }

        public object Clone()
        {
            var json = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject<DiagnosticInfo>(json);
        }

        public Guid GetKey() => Id;
    }
}

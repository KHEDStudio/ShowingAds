using Newtonsoft.Json;
using ShowingAds.CoreLibrary.Converters;
using ShowingAds.CoreLibrary.Enums;
using ShowingAds.CoreLibrary.Interfaces;
using ShowingAds.CoreLibrary.Models.Database;
using ShowingAds.CoreLibrary.Models.Login;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.CoreLibrary.Models.States
{
    public class DeviceState : Device, IModel<Guid>
    {
        [JsonProperty("status")]
        public DeviceStatus DeviceStatus { get; set; }
        [JsonProperty("total_contents")]
        public int TotalContents { get; set; }
        [JsonProperty("current_video"), JsonConverter(typeof(GuidConverter))]
        public Guid CurrentVideo { get; set; }
        [JsonProperty("clients")]
        public Dictionary<Guid, int> VideoDownloaded { get; set; }
        [JsonProperty("priority"), JsonConverter(typeof(GuidConverter))]
        public Guid PriorityAdvertisingClient { get; set; }

        [JsonConstructor]
        public DeviceState(Guid id, string name, string address, double latitude, double longitude, DateTime last_online, int account, Guid channel, 
            DeviceStatus status, int total_contents, Guid current_video, Dictionary<Guid, int> clients, Guid priority)
            : base(id, name, address, latitude, longitude, last_online, account, channel)
        {
            DeviceStatus = status;
            TotalContents = total_contents;
            CurrentVideo = current_video;
            VideoDownloaded = clients ?? new Dictionary<Guid, int>();
            PriorityAdvertisingClient = priority;
        }

        public DeviceState(Device device) : base(device) =>
            InitializeEmptyDeviceState();

        public DeviceState(Device device, DeviceState deviceState) : base(device)
        {
            DeviceStatus = deviceState.DeviceStatus;
            TotalContents = deviceState.TotalContents;
            CurrentVideo = deviceState.CurrentVideo;
            VideoDownloaded = deviceState.VideoDownloaded;
            PriorityAdvertisingClient = deviceState.PriorityAdvertisingClient;
        }

        public DeviceState(LoginDevice login, int ownerId) : base(login, ownerId) =>
            InitializeEmptyDeviceState();

        private void InitializeEmptyDeviceState()
        {
            DeviceStatus = DeviceStatus.Offline;
            TotalContents = int.MinValue;
            CurrentVideo = Guid.Empty;
            VideoDownloaded = new Dictionary<Guid, int>();
            PriorityAdvertisingClient = Guid.Empty;
        }
    }
}

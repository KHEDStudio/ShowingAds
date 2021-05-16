using Newtonsoft.Json;
using ShowingAds.Shared.Backend.Models.Database;
using ShowingAds.Shared.Core.Converters;
using ShowingAds.Shared.Core.Enums;
using ShowingAds.Shared.Core.Models;
using ShowingAds.Shared.Core.Models.Login;
using ShowingAds.Shared.Core.Models.States;
using System;

namespace ShowingAds.Shared.Backend.Models.States
{
    public class DeviceState : Device, IModel<Guid>
    {
        [JsonProperty("status")]
        public DeviceStatus DeviceStatus { get; set; }
        [JsonProperty("priority"), JsonConverter(typeof(GuidConverter))]
        public Guid PriorityAdvertisingClient { get; set; }
        [JsonProperty("info")]
        public DiagnosticInfo DiagnosticInfo { get; set; }

        [JsonConstructor]
        public DeviceState(Guid id, string name, string address, double latitude, double longitude, DateTime last_online, int account, Guid channel, 
            DeviceStatus status, DiagnosticInfo info, Guid priority)
            : base(id, name, address, latitude, longitude, last_online, account, channel)
        {
            DeviceStatus = status;
            DiagnosticInfo = info ?? new DiagnosticInfo("1.0", 0, 0, 0, 0);
            PriorityAdvertisingClient = priority;
        }

        public DeviceState(Device device) : base(device) =>
            InitializeEmptyDeviceState();

        public DeviceState(Device device, DeviceState deviceState) : base(device)
        {
            DeviceStatus = deviceState.DeviceStatus;
            DiagnosticInfo = deviceState.DiagnosticInfo;
            PriorityAdvertisingClient = deviceState.PriorityAdvertisingClient;
        }

        public DeviceState(LoginDevice login, int ownerId) : base(login, ownerId) =>
            InitializeEmptyDeviceState();

        private void InitializeEmptyDeviceState()
        {
            DeviceStatus = DeviceStatus.Offline;
            PriorityAdvertisingClient = Guid.Empty;
            DiagnosticInfo = new DiagnosticInfo("1.0", 0, 0, 0, 0);
        }

        public new object Clone()
        {
            var deviceState = MemberwiseClone() as DeviceState;
            deviceState.DiagnosticInfo = DiagnosticInfo.Clone() as DiagnosticInfo;
            return deviceState;
        }
    }
}

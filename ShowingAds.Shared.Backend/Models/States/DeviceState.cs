using Newtonsoft.Json;
using ShowingAds.Shared.Backend.Models.Database;
using ShowingAds.Shared.Core.Converters;
using ShowingAds.Shared.Core.Enums;
using ShowingAds.Shared.Core.Models;
using ShowingAds.Shared.Core.Models.Login;
using ShowingAds.Shared.Core.Models.States;
using System;
using System.Collections.Generic;

namespace ShowingAds.Shared.Backend.Models.States
{
    public class DeviceState : Device, IModel<Guid>
    {
        [JsonProperty("info")]
        public DiagnosticInfo DiagnosticInfo { get; set; }
        [JsonProperty("tasks")]
        public DeviceTasks Tasks { get; set; }

        [JsonConstructor]
        public DeviceState(Guid id, string name, string address, double latitude, double longitude, DateTime last_online, int account, 
            Guid channel, DiagnosticInfo info, Guid priority, bool is_updated, bool take_screenshot, bool reboot)
            : base(id, name, address, latitude, longitude, last_online, account, channel)
        {
            DiagnosticInfo = info ?? new DiagnosticInfo(id);
            Tasks = new DeviceTasks(id, priority, is_updated, take_screenshot, reboot);
        }

        public DeviceState(Device device) : base(device) =>
            InitializeEmptyDeviceState();

        public DeviceState(Device device, DiagnosticInfo info, DeviceTasks tasks) : base(device)
        {
            DiagnosticInfo = info;
            Tasks = tasks;
        }

        public DeviceState(Device device, DeviceState deviceState) : base(device)
        {
            DiagnosticInfo = deviceState.DiagnosticInfo;
            Tasks = deviceState.Tasks;
        }

        public DeviceState(LoginDevice login, int ownerId) : base(login, ownerId) =>
            InitializeEmptyDeviceState();

        private void InitializeEmptyDeviceState()
        {
            DiagnosticInfo = new DiagnosticInfo(Id);
            Tasks = new DeviceTasks(Id);
        }

        public new object Clone()
        {
            var json = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject<DeviceState>(json);
        }
    }
}

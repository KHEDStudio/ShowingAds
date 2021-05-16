using ShowingAds.CoreLibrary.Enums;
using ShowingAds.CoreLibrary.Models.Database;
using ShowingAds.CoreLibrary.Models.Json;
using ShowingAds.CoreLibrary.Models.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.DevicesService.BusinessLayer.Interfaces
{
    public interface IDeviceManager
    {
        public Task<ChannelJson> GetChannelJsonAsync(Guid channelId);
        public Task<bool> SetDeviceStatusAsync(Guid deviceId, DeviceStatus status);
        public Task<bool> SetDiagnosticInfoAsync(Guid deviceId, DiagnosticInfo info);
        public Task<bool> TryChoosePriorityClientAsync(Guid deviceId, Guid clientId);
        public Task<bool> TryUpdateDeviceAsync(Device device);
        public Task<(bool, DeviceState)> TryGetAsync(Guid deviceId);
        public Task NotifyClientsServiceAsync(DeviceState device);
    }
}

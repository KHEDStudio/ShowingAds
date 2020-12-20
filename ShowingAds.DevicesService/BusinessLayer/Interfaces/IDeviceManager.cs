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
        public Task<ChannelJson> GetChannelJson(Guid channelId);
        public Task<bool> SetDeviceStatus(Guid deviceId, DeviceStatus status);
        public Task<bool> SetTotalContentVideos(Guid deviceId, int count);
        public Task<bool> TryChoosePriorityClient(Guid deviceId, Guid clientId);
        public Task<bool> TryUpdateDevice(Device device);
        public Task<(bool, DeviceState)> TryGet(Guid deviceId);
        public Task NotifyServer(DeviceState device);
    }
}

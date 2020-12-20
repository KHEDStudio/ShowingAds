using Newtonsoft.Json;
using Nito.AsyncEx;
using NLog;
using ShowingAds.CoreLibrary.Abstract;
using ShowingAds.CoreLibrary.DataProviders;
using ShowingAds.CoreLibrary.Enums;
using ShowingAds.CoreLibrary.Managers;
using ShowingAds.CoreLibrary.Models.Database;
using ShowingAds.CoreLibrary.Models.Json;
using ShowingAds.CoreLibrary.Models.States;
using ShowingAds.DevicesService.BusinessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace ShowingAds.DevicesService.BusinessLayer
{
    public sealed class DeviceManager : ModelManager<Guid, DeviceState, DeviceManager>, IDeviceManager
    {
        private Logger _logger { get; }
        private AsyncLock _syncLock { get; set; }

        private DeviceManager() : base(new WebProvider<Guid, DeviceState>(Settings.DevicesPath))
        {
            _logger = LogManager.GetCurrentClassLogger();
            _syncLock = new AsyncLock();
            UpdateOrInitializeModels(default, default);
        }

        protected async override void UpdateOrInitializeModels(object sender, ElapsedEventArgs e)
        {
            _logger.Info("Initialize devices...");
            using (await _mutex.LockAsync())
            {
                var devices = await _provider.GetModels();
                foreach (var model in devices)
                {
                    model.DeviceStatus = DeviceStatus.Offline;
                    _models.Add(model.GetKey(), model);
                }
            }
        }

        public async Task<ChannelJson> GetChannelJson(Guid channelId)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var requestUri = new Uri(Settings.ChannelPath, $"?channel={channelId}");
                    var response = await httpClient.GetAsync(requestUri);
                    if (response.IsSuccessStatusCode)
                    {
                        var json = await response.Content.ReadAsStringAsync();
                        return JsonConvert.DeserializeObject<ChannelJson>(json);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
            return default;
        }

        public async Task<bool> SetDeviceStatus(Guid deviceId, DeviceStatus status)
        {
            using (await _syncLock.LockAsync())
            {
                var (isExists, deviceState) = await TryGet(deviceId);
                if (isExists)
                {
                    deviceState.DeviceStatus = status;
                    deviceState.LastOnline = DateTime.Now;
                    var isSuccess = await TryAddOrUpdate(deviceState.Id, deviceState);
                    if (isSuccess)
                    {
                        NotifyServer(deviceState);
                        return isSuccess;
                    }
                }
                return false;
            }
        }

        public async Task<bool> SetTotalContentVideos(Guid deviceId, int count)
        {
            using (await _syncLock.LockAsync())
            {
                var (isExists, deviceState) = await TryGet(deviceId);
                if (isExists)
                {
                    deviceState.TotalContents = count;
                    var isSuccess = await TryAddOrUpdate(deviceState.Id, deviceState);
                    if (isSuccess)
                    {
                        NotifyServer(deviceState);
                        return isSuccess;
                    }
                }
                return false;
            }
        }

        public async Task<bool> TryUpdateDevice(Device device)
        {
            using (await _syncLock.LockAsync())
            {
                (var isExists, var deviceState) = await TryGet(device.Id);
                if (isExists)
                {
                    var newDeviceState = new DeviceState(device, deviceState);
                    return await TryAddOrUpdate(newDeviceState.Id, newDeviceState);
                }
                return false;
            }
        }

        public async Task<bool> TryChoosePriorityClient(Guid deviceId, Guid clientId)
        {
            using (await _syncLock.LockAsync())
            {
                (var isExists, var deviceState) = await TryGet(deviceId);
                if (isExists)
                {
                    deviceState.PriorityAdvertisingClient = clientId;
                    return await TryAddOrUpdate(deviceState.Id, deviceState);
                }
                return false;
            }
        }

        public async Task NotifyServer(DeviceState device)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    var json = JsonConvert.SerializeObject(device);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    await httpClient.PostAsync(Settings.UpdatePath, content);
                }
            } 
            catch (Exception ex)
            {
                _logger.Error(ex);
            }
        }
    }
}

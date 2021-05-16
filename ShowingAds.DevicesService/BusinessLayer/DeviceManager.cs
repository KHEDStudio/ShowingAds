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
    public sealed class DeviceManager : SaveModelManager<Guid, DeviceState, DeviceManager>, IDeviceManager
    {
        private Logger _logger { get; }
        private AsyncLock _syncLock { get; set; }

        private DeviceManager() : base(new WebProvider<Guid, DeviceState>(Settings.DevicesPath))
        {
            _logger = LogManager.GetCurrentClassLogger();
            _syncLock = new AsyncLock();
            UpdateOrInitializeModelsAsync().Wait();
        }

        protected async override Task UpdateOrInitializeModelsAsync()
        {
            _logger.Info("Initialize devices...");
            try
            {
                _mutex.WaitOne();
                var devices = await _provider.GetModelsAsync();
                foreach (var model in devices)
                {
                    model.DeviceStatus = DeviceStatus.Offline;
                    _models.Add(model.GetKey(), model);
                }
            }
            catch
            {
                throw;
            }
            finally
            {
                _mutex.Set();
            }
        }

        public async Task<ChannelJson> GetChannelJsonAsync(Guid channelId)
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

        public async Task<bool> SetDeviceStatusAsync(Guid deviceId, DeviceStatus status)
        {
            using (await _syncLock.LockAsync())
            {
                var (isExists, deviceState) = await TryGetAsync(deviceId);
                if (isExists)
                {
                    deviceState.DeviceStatus = status;
                    deviceState.LastOnline = DateTime.Now;
                    var isSuccess = await TryAddOrUpdateAsync(deviceState.Id, deviceState);
                    if (isSuccess)
                    {
                        await NotifyClientsServiceAsync(deviceState);
                        return isSuccess;
                    }
                }
                return false;
            }
        }

        public async Task<bool> SetDiagnosticInfoAsync(Guid deviceId, DiagnosticInfo info)
        {
            using (await _syncLock.LockAsync())
            {
                var (isExists, deviceState) = await TryGetAsync(deviceId);
                if (isExists)
                {
                    deviceState.DiagnosticInfo = info;
                    var isSuccess = await TryAddOrUpdateAsync(deviceState.Id, deviceState);
                    if (isSuccess)
                    {
                        await NotifyClientsServiceAsync(deviceState);
                        return isSuccess;
                    }
                }
                return false;
            }
        }

        public async Task<bool> TryUpdateDeviceAsync(Device device)
        {
            using (await _syncLock.LockAsync())
            {
                (var isExists, var deviceState) = await TryGetAsync(device.Id);
                if (isExists)
                {
                    var newDeviceState = new DeviceState(device, deviceState);
                    return await TryAddOrUpdateAsync(newDeviceState.Id, newDeviceState);
                }
                return false;
            }
        }

        public async Task<bool> TryChoosePriorityClientAsync(Guid deviceId, Guid clientId)
        {
            using (await _syncLock.LockAsync())
            {
                (var isExists, var deviceState) = await TryGetAsync(deviceId);
                if (isExists)
                {
                    deviceState.PriorityAdvertisingClient = clientId;
                    return await TryAddOrUpdateAsync(deviceState.Id, deviceState);
                }
                return false;
            }
        }

        public async Task NotifyClientsServiceAsync(DeviceState device)
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

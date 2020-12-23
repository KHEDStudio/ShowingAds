using Nito.AsyncEx;
using NLog;
using ShowingAds.CoreLibrary;
using ShowingAds.CoreLibrary.DataProviders;
using ShowingAds.CoreLibrary.Managers;
using ShowingAds.CoreLibrary.Models.States;
using ShowingAds.WebAssembly.Server.BusinessLayer.Managers.Abstract;
using ShowingAds.WebAssembly.Server.BusinessLayer.Managers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace ShowingAds.WebAssembly.Server.BusinessLayer.Managers
{
    public class DeviceManager : NotifyModelManager<Guid, DeviceState, DeviceManager>, IModelManager<Guid, DeviceState>
    {
        private Logger _logger { get; }

        private DeviceManager() : base(new WebProvider<Guid, DeviceState>(Settings.DevicesPath))
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
            EventBus.GetInstance().StartManagersUpdate += UpdateOrInitializeModels;
            UpdateOrInitializeModels();
        }

        protected override void UpdateOrInitializeModels()
        {
            _logger.Info("Update or initialize Devices...");
            base.UpdateOrInitializeModels();
        }

        public async Task<IEnumerable<DeviceState>> GetPermittedModels(List<int> users) =>
            await GetCollection(x => users.Contains(x.OwnerId));

        public async Task<bool> TryAddOrUpdate(DeviceState model) =>
            await TryAddOrUpdate(model.Id, model);

        public async Task UpdateModel(DeviceState device)
        {
            using (await _mutex.LockAsync())
            {
                var newDevice = (DeviceState)device.Clone();
                if (_models.ContainsKey(device.Id))
                {
                    _models.Remove(device.Id);
                    _models.Add(newDevice.Id, newDevice);
                }
                else
                {
                    _models.Add(newDevice.Id, newDevice);
                }
                NotifyUsers(device);
            }
        }
    }
}

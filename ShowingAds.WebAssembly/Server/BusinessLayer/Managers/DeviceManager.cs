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

        private DeviceManager() : base(new WebProvider<Guid, DeviceState>(Settings.DevicesPath), Settings.NotifyDevicePath)
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
        }

        public async Task<IEnumerable<DeviceState>> GetPermittedModelsAsync(IEnumerable<int> users) =>
            await GetCollectionAsync(x => users.Contains(x.OwnerId));

        public async Task<bool> TryAddOrUpdateAsync(DeviceState model) =>
            await TryAddOrUpdateAsync(model.Id, model);

        public async Task UpdateModelAsync(DeviceState device) => await NotifySubscribersAsync(device);

        public override async Task<IEnumerable<Guid>> GetSubscribersAsync(DeviceState model)
        {
            var userManager = UserManager.GetInstance();
            var employerUsers = await userManager.GetEmployerUsers(model.OwnerId);
            return employerUsers.Select(x => x.ToGuid());
        }
    }
}

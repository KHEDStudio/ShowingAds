using ShowingAds.Backend.DeviceService.Providers;
using ShowingAds.Shared.Backend.DataProviders;
using ShowingAds.Shared.Backend.Managers;
using ShowingAds.Shared.Backend.Models.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShowingAds.Shared.Core;
using ShowingAds.Shared.Core.Models.States;
using ShowingAds.Shared.Core.Enums;

namespace ShowingAds.Backend.DeviceService.Managers
{
    public class DeviceManager : NotifyModelManager<Guid, DeviceState, DeviceManager>, IModelManager<Guid, DeviceState>
    {
        public DeviceManager() : base(new DeviceProvider(Settings.DevicesPath))
        {
            _ = UpdateOrInitializeModelsAsync();
        }

        public async Task<IEnumerable<DeviceState>> GetPermittedModelsAsync(IEnumerable<int> users) =>
            await GetCollectionOrNullAsync(x => users.Contains(x.OwnerId));

        public async Task<bool> SetDiagnosticInfoAsync(Guid key, DiagnosticInfo info)
        {
            var device = await GetOrDefaultAsync(key);
            if (device != default)
            {
                device.LastOnline = DateTime.UtcNow;
                device.DeviceStatus &= ~DeviceStatus.Offline;
                device.DiagnosticInfo = info;
                await TryAddOrUpdateAsync(device.Id, device);
                return true;
            }
            return false;
        }

        public override async Task<IEnumerable<Guid>> GetSubscribersAsync(DeviceState model)
        {
            var userManager = UserManager.GetInstance();
            var employerUsers = await userManager.GetEmployerUsers(model.OwnerId);
            return employerUsers.Select(x => x.ToGuid());
        }
    }
}

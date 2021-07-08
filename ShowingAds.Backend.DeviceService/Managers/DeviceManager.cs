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
using ShowingAds.Shared.Backend.Models.NotifyService;
using ShowingAds.Shared.Backend.Enums;
using Newtonsoft.Json;
using ShowingAds.Shared.Backend.Models.Database;

namespace ShowingAds.Backend.DeviceService.Managers
{
    public class DeviceManager : NotifyModelManager<Guid, Device, DeviceManager>, IModelManager<Guid, Device>
    {
        public DeviceManager() : base(new DeviceProvider(Settings.DevicesPath))
        {
            _ = UpdateOrInitializeModelsAsync();
        }

        public async Task<IEnumerable<Device>> GetPermittedModelsAsync(IEnumerable<int> users) =>
            await GetCollectionOrNullAsync(x => users.Contains(x.OwnerId));

        public override async Task<IEnumerable<Guid>> GetSubscribersAsync(Guid key, Device model)
        {
            var userManager = UserManager.GetInstance();
            var employerUsers = await userManager.GetEmployerUsers(model.OwnerId);
            return employerUsers.Select(x => x.ToGuid());
        }
    }
}

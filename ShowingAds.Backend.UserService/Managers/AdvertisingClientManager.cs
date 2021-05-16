using ShowingAds.Shared.Backend.DataProviders;
using ShowingAds.Shared.Backend.Models.Database;
using ShowingAds.Shared.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.UserService.Managers
{
    public sealed class AdvertisingClientManager : NotifyModelManager<Guid, AdvertisingClient, AdvertisingClientManager>, IModelManager<Guid, AdvertisingClient>
    {
        private AdvertisingClientManager() : base(new WebProvider<Guid, AdvertisingClient>(Settings.AdvertisingClientsPath)) { }

        public async Task<IEnumerable<AdvertisingClient>> GetPermittedModelsAsync(IEnumerable<int> users) =>
            await GetCollectionOrNullAsync(x => users.Contains(x.OwnerId));

        public async Task<bool> TryAddOrUpdateAsync(AdvertisingClient model) =>
            await TryAddOrUpdateAsync(model.Id, model);

        public async Task<bool> TryDeleteAsync(AdvertisingClient model) =>
            await TryDeleteAsync(model.Id, model);

        public override async Task<IEnumerable<Guid>> GetSubscribersAsync(AdvertisingClient model)
        {
            var userManager = UserManager.GetInstance();
            var users = await userManager.GetEmployerUsers(model.OwnerId);
            return users.Select(x => x.ToGuid());
        }
    }
}

using ShowingAds.Shared.Backend.DataProviders;
using ShowingAds.Shared.Backend.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.UserService.Managers
{
    public sealed class AdvertisingVideoManager : NotifyModelManager<Guid, AdvertisingVideo, AdvertisingVideoManager>, IModelManager<Guid, AdvertisingVideo>
    {
        private AdvertisingVideoManager() : base(new WebProvider<Guid, AdvertisingVideo>(Settings.AdvertisingVideosPath)) { }

        public async Task<IEnumerable<AdvertisingVideo>> GetPermittedModelsAsync(IEnumerable<int> users)
        {
            var manager = AdvertisingClientManager.GetInstance();
            var clients = await manager.GetCollectionOrNullAsync(x => users.Contains(x.OwnerId));
            return await GetCollectionOrNullAsync(x => clients.Any(y => y.Id == x.AdvertisingClientId));
        }

        public async Task<bool> TryAddOrUpdateAsync(AdvertisingVideo model) =>
            await TryAddOrUpdateAsync(model.Id, model);

        public async Task<bool> TryDeleteAsync(AdvertisingVideo model) =>
            await TryDeleteAsync(model.Id, model);

        public override async Task<IEnumerable<Guid>> GetSubscribersAsync(AdvertisingVideo model)
        {
            var advertisingClientManager = AdvertisingClientManager.GetInstance();
            var advertisingClient = await advertisingClientManager.GetOrDefaultAsync(model.AdvertisingClientId);
            if (advertisingClient != default)
                return await advertisingClientManager.GetSubscribersAsync(advertisingClient);
            return new List<Guid>();
        }
    }
}

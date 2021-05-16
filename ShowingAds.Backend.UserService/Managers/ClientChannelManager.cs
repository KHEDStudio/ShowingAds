using ShowingAds.Shared.Backend.DataProviders;
using ShowingAds.Shared.Backend.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.UserService.Managers
{
    public sealed class ClientChannelManager : NotifyModelManager<Guid, ClientChannel, ClientChannelManager>, IModelManager<Guid, ClientChannel>
    {
        private ClientChannelManager() : base(new WebProvider<Guid, ClientChannel>(Settings.ClientChannelsPath)) { }

        public async Task<IEnumerable<ClientChannel>> GetPermittedModelsAsync(IEnumerable<int> users)
        {
            var manager = ChannelManager.GetInstance();
            var channels = await manager.GetCollectionOrNullAsync(x => users.Contains(x.OwnerId));
            return await GetCollectionOrNullAsync(x => channels.Any(y => y.Id == x.ChannelId));
        }

        public async Task<bool> TryAddOrUpdateAsync(ClientChannel model) =>
            await TryAddOrUpdateAsync(model.Id, model);

        public async Task<bool> TryDeleteAsync(ClientChannel model) =>
            await TryDeleteAsync(model.Id, model);

        public override async Task<IEnumerable<Guid>> GetSubscribersAsync(ClientChannel model)
        {
            var channelManager = ChannelManager.GetInstance();
            var channel = await channelManager.GetOrDefaultAsync(model.ChannelId);
            if (channel != default)
                return await channelManager.GetSubscribersAsync(channel);
            return new List<Guid>();
        }
    }
}

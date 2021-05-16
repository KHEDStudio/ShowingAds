using ShowingAds.Shared.Backend.DataProviders;
using ShowingAds.Shared.Backend.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.UserService.Managers
{
    public sealed class OrderManager : NotifyModelManager<Guid, Order, OrderManager>, IModelManager<Guid, Order>
    {
        private OrderManager() : base(new WebProvider<Guid, Order>(Settings.OrdersPath)) { }

        public async Task<IEnumerable<Order>> GetPermittedModelsAsync(IEnumerable<int> users)
        {
            var manager = ChannelManager.GetInstance();
            var channels = await manager.GetCollectionOrNullAsync(x => users.Contains(x.OwnerId));
            return await GetCollectionOrNullAsync(x => channels.Any(y => y.Id == x.ChannelId));
        }

        public async Task<bool> TryAddOrUpdateAsync(Order model) =>
            await TryAddOrUpdateAsync(model.Id, model);

        public async Task<bool> TryDeleteAsync(Order model) =>
            await TryDeleteAsync(model.Id, model);

        public override async Task<IEnumerable<Guid>> GetSubscribersAsync(Order model)
        {
            var channelManager = ChannelManager.GetInstance();
            var channel = await channelManager.GetOrDefaultAsync(model.ChannelId);
            if (channel != default)
                return await channelManager.GetSubscribersAsync(channel);
            return new List<Guid>();
        }
    }
}

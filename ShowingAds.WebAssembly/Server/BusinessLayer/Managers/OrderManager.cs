using Nito.AsyncEx;
using NLog;
using ShowingAds.CoreLibrary;
using ShowingAds.CoreLibrary.DataProviders;
using ShowingAds.CoreLibrary.Managers;
using ShowingAds.CoreLibrary.Models.Database;
using ShowingAds.WebAssembly.Server.BusinessLayer.Managers.Abstract;
using ShowingAds.WebAssembly.Server.BusinessLayer.Managers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace ShowingAds.WebAssembly.Server.BusinessLayer.Managers
{
    public sealed class OrderManager : NotifyModelManager<Guid, Order, OrderManager>, IModelManager<Guid, Order>
    {
        private Logger _logger { get; }

        private OrderManager() : base(new WebProvider<Guid, Order>(Settings.OrdersPath), Settings.NotifyChannelPath)
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
        }

        public async Task<IEnumerable<Order>> GetPermittedModelsAsync(IEnumerable<int> users)
        {
            var manager = ChannelManager.GetInstance();
            var channels = await manager.GetCollectionAsync(x => users.Contains(x.OwnerId));
            return await GetCollectionAsync(x => channels.Any(y => y.Id == x.ChannelId));
        }

        public async Task<bool> TryAddOrUpdateAsync(Order model) =>
            await TryAddOrUpdateAsync(model.Id, model);

        public override async Task<IEnumerable<Guid>> GetSubscribersAsync(Order model)
        {
            var channelManager = ChannelManager.GetInstance();
            var (isSuccess, channel) = await channelManager.TryGetAsync(model.ChannelId);
            if (isSuccess)
                return await channelManager.GetSubscribersAsync(channel);
            return new List<Guid>();
        }
    }
}

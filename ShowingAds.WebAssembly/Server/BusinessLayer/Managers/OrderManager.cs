using Nito.AsyncEx;
using NLog;
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

        private OrderManager() : base(new WebProvider<Guid, Order>(Settings.OrdersPath))
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
            UpdateOrInitializeModels(default, default);
        }

        protected override void UpdateOrInitializeModels(object sender, ElapsedEventArgs e)
        {
            _logger.Info("Initialize Orders...");
            base.UpdateOrInitializeModels(sender, e);
            _syncTimer.Start();
        }

        public async Task<IEnumerable<Order>> GetPermittedModels(List<int> users)
        {
            var manager = ChannelManager.GetInstance();
            var channels = await manager.GetCollection(x => users.Contains(x.OwnerId));
            return await GetCollection(x => channels.Any(y => y.Id == x.ChannelId));
        }

        public async Task<bool> TryAddOrUpdate(Order model) =>
            await TryAddOrUpdate(model.Id, model);
    }
}

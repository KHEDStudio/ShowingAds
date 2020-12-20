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
    public sealed class ChannelManager : NotifyModelManager<Guid, Channel, ChannelManager>, IModelManager<Guid, Channel>
    {
        private Logger _logger { get; }

        private ChannelManager() : base(new WebProvider<Guid, Channel>(Settings.ChannelsPath))
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
            UpdateOrInitializeModels(default, default);
        }

        protected override void UpdateOrInitializeModels(object sender, ElapsedEventArgs e)
        {
            _logger.Info("Initialize channels...");
            base.UpdateOrInitializeModels(sender, e);
            _syncTimer.Start();
        }

        public async Task<IEnumerable<Channel>> GetPermittedModels(List<int> users) =>
            await GetCollection(x => users.Contains(x.OwnerId));

        public async Task<bool> TryAddOrUpdate(Channel model) =>
            await TryAddOrUpdate(model.Id, model);
    }
}

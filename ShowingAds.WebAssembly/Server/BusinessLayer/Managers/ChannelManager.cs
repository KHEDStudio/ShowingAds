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
    public sealed class ChannelManager : NotifyModelManager<Guid, Channel, ChannelManager>, IModelManager<Guid, Channel>
    {
        private Logger _logger { get; }

        private ChannelManager() : base(new WebProvider<Guid, Channel>(Settings.ChannelsPath))
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
            EventBus.GetInstance().StartManagersUpdate += UpdateOrInitializeModels;
            UpdateOrInitializeModels();
        }

        protected override void UpdateOrInitializeModels()
        {
            _logger.Info("Update or initialize channels...");
            base.UpdateOrInitializeModels();
        }

        public async Task<IEnumerable<Channel>> GetPermittedModels(List<int> users) =>
            await GetCollection(x => users.Contains(x.OwnerId));

        public async Task<bool> TryAddOrUpdate(Channel model) =>
            await TryAddOrUpdate(model.Id, model);
    }
}

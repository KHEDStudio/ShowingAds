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
    public sealed class ClientChannelManager : NotifyModelManager<Guid, ClientChannel, ClientChannelManager>, IModelManager<Guid, ClientChannel>
    {
        private Logger _logger { get; }

        private ClientChannelManager() : base(new WebProvider<Guid, ClientChannel>(Settings.ClientChannelsPath))
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
            EventBus.GetInstance().StartManagersUpdate += UpdateOrInitializeModels;
            UpdateOrInitializeModels();
        }

        protected override void UpdateOrInitializeModels()
        {
            _logger.Info("Update or initialize ClientChannels...");
            base.UpdateOrInitializeModels();
        }

        public async Task<IEnumerable<ClientChannel>> GetPermittedModels(List<int> users)
        {
            var manager = ChannelManager.GetInstance();
            var channels = await manager.GetCollection(x => users.Contains(x.OwnerId));
            return await GetCollection(x => channels.Any(y => y.Id == x.ChannelId));
        }

        public async Task<bool> TryAddOrUpdate(ClientChannel model) =>
            await TryAddOrUpdate(model.Id, model);
    }
}

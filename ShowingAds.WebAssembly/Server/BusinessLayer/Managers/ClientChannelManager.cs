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

        private ClientChannelManager() : base(new WebProvider<Guid, ClientChannel>(Settings.ClientChannelsPath), Settings.NotifyChannelPath)
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
        }

        public async Task<IEnumerable<ClientChannel>> GetPermittedModelsAsync(IEnumerable<int> users)
        {
            var manager = ChannelManager.GetInstance();
            var channels = await manager.GetCollectionAsync(x => users.Contains(x.OwnerId));
            return await GetCollectionAsync(x => channels.Any(y => y.Id == x.ChannelId));
        }

        public async Task<bool> TryAddOrUpdateAsync(ClientChannel model) =>
            await TryAddOrUpdateAsync(model.Id, model);

        public override async Task<IEnumerable<Guid>> GetSubscribersAsync(ClientChannel model)
        {
            var channelManager = ChannelManager.GetInstance();
            var (isSuccess, channel) = await channelManager.TryGetAsync(model.ChannelId);
            if (isSuccess)
                return await channelManager.GetSubscribersAsync(channel);
            return new List<Guid>();
        }
    }
}

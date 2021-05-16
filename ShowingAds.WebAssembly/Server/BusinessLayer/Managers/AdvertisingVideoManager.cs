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
    public sealed class AdvertisingVideoManager : NotifyModelManager<Guid, AdvertisingVideo, AdvertisingVideoManager>, IModelManager<Guid, AdvertisingVideo>
    {
        private Logger _logger { get; }

        private AdvertisingVideoManager() : base(new WebProvider<Guid, AdvertisingVideo>(Settings.AdvertisingVideosPath), Settings.NotifyChannelPath)
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
        }

        public async Task<IEnumerable<AdvertisingVideo>> GetPermittedModelsAsync(IEnumerable<int> users)
        {
            var manager = AdvertisingClientManager.GetInstance();
            var clients = await manager.GetCollectionAsync(x => users.Contains(x.OwnerId));
            return await GetCollectionAsync(x => clients.Any(y => y.Id == x.AdvertisingClientId));
        }

        public async Task<bool> TryAddOrUpdateAsync(AdvertisingVideo model) =>
            await TryAddOrUpdateAsync(model.Id, model);

        public override async Task<IEnumerable<Guid>> GetSubscribersAsync(AdvertisingVideo model)
        {
            var advertisingClientManager = AdvertisingClientManager.GetInstance();
            var (isSuccess, advertisingClient) = await advertisingClientManager.TryGetAsync(model.AdvertisingClientId);
            if (isSuccess)
                return await advertisingClientManager.GetSubscribersAsync(advertisingClient);
            return new List<Guid>();
        }
    }
}

using NLog;
using ShowingAds.CoreLibrary.DataProviders;
using ShowingAds.CoreLibrary.Models.Database;
using ShowingAds.WebAssembly.Server.BusinessLayer.Managers.Abstract;
using ShowingAds.WebAssembly.Server.BusinessLayer.Managers.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ShowingAds.CoreLibrary;

namespace ShowingAds.WebAssembly.Server.BusinessLayer.Managers
{
    public sealed class AdvertisingClientManager : NotifyModelManager<Guid, AdvertisingClient, AdvertisingClientManager>, IModelManager<Guid, AdvertisingClient>
    {
        private Logger _logger { get; }

        private AdvertisingClientManager() : base(new WebProvider<Guid, AdvertisingClient>(Settings.AdvertisingClientsPath), Settings.NotifyChannelPath)
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
        }

        public async Task<IEnumerable<AdvertisingClient>> GetPermittedModelsAsync(IEnumerable<int> users) =>
            await GetCollectionAsync(x => users.Contains(x.OwnerId));

        public async Task<bool> TryAddOrUpdateAsync(AdvertisingClient model) =>
            await TryAddOrUpdateAsync(model.Id, model);

        public override async Task<IEnumerable<Guid>> GetSubscribersAsync(AdvertisingClient model)
        {
            var userManager = UserManager.GetInstance();
            var users = await userManager.GetEmployerUsers(model.OwnerId);
            return users.Select(x => x.ToGuid());
        }
    }
}

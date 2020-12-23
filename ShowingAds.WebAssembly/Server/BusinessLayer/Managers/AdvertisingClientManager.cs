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
    public sealed class AdvertisingClientManager : NotifyModelManager<Guid, AdvertisingClient, AdvertisingClientManager>, IModelManager<Guid, AdvertisingClient>
    {
        private Logger _logger { get; }

        private AdvertisingClientManager() : base(new WebProvider<Guid, AdvertisingClient>(Settings.AdvertisingClientsPath))
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
            EventBus.GetInstance().StartManagersUpdate += UpdateOrInitializeModels;
            UpdateOrInitializeModels();
        }

        protected override void UpdateOrInitializeModels()
        {
            _logger.Info("Update or initialize AdvertisingClients...");
            base.UpdateOrInitializeModels();
        }

        public async Task<IEnumerable<AdvertisingClient>> GetPermittedModels(List<int> users) =>
            await GetCollection(x => users.Contains(x.OwnerId));

        public async Task<bool> TryAddOrUpdate(AdvertisingClient model) =>
            await TryAddOrUpdate(model.Id, model);
    }
}

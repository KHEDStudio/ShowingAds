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
    public sealed class AdvertisingVideoManager : NotifyModelManager<Guid, AdvertisingVideo, AdvertisingVideoManager>, IModelManager<Guid, AdvertisingVideo>
    {
        private Logger _logger { get; }

        private AdvertisingVideoManager() : base(new WebProvider<Guid, AdvertisingVideo>(Settings.AdvertisingVideosPath))
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
            UpdateOrInitializeModels(default, default);
        }

        protected override void UpdateOrInitializeModels(object sender, ElapsedEventArgs e)
        {
            _logger.Info("Initialize AdvertisingVideos...");
            base.UpdateOrInitializeModels(sender, e);
            _syncTimer.Start();
        }

        public async Task<IEnumerable<AdvertisingVideo>> GetPermittedModels(List<int> users)
        {
            var manager = AdvertisingClientManager.GetInstance();
            var clients = await manager.GetCollection(x => users.Contains(x.OwnerId));
            return await GetCollection(x => clients.Any(y => y.Id == x.AdvertisingClientId));
        }

        public async Task<bool> TryAddOrUpdate(AdvertisingVideo model) =>
            await TryAddOrUpdate(model.Id, model);
    }
}

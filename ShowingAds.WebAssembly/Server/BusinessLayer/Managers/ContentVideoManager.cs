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
    public sealed class ContentVideoManager : NotifyModelManager<Guid, ContentVideo, ContentVideoManager>, IModelManager<Guid, ContentVideo>
    {
        private Logger _logger { get; }

        private ContentVideoManager() : base(new WebProvider<Guid, ContentVideo>(Settings.ContentVideosPath))
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
            EventBus.GetInstance().StartManagersUpdate += UpdateOrInitializeModels;
            UpdateOrInitializeModels();
        }

        protected override void UpdateOrInitializeModels()
        {
            _logger.Info("Update or initialize ContentVideos...");
            base.UpdateOrInitializeModels();
        }

        public async Task<IEnumerable<ContentVideo>> GetPermittedModels(List<int> users)
        {
            var manager = ContentManager.GetInstance();
            var contents = await manager.GetCollection(x => users.Contains(x.OwnerId));
            return await GetCollection(x => contents.Any(y => y.Id == x.ContentId));
        }

        public async Task<bool> TryAddOrUpdate(ContentVideo model) =>
            await TryAddOrUpdate(model.Id, model);
    }
}

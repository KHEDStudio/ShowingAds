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

        private ContentVideoManager() : base(new WebProvider<Guid, ContentVideo>(Settings.ContentVideosPath), Settings.NotifyChannelPath)
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
        }

        public async Task<IEnumerable<ContentVideo>> GetPermittedModelsAsync(IEnumerable<int> users)
        {
            var manager = ContentManager.GetInstance();
            var contents = await manager.GetCollectionAsync(x => users.Contains(x.OwnerId));
            return await GetCollectionAsync(x => contents.Any(y => y.Id == x.ContentId));
        }

        public async Task<bool> TryAddOrUpdateAsync(ContentVideo model) =>
            await TryAddOrUpdateAsync(model.Id, model);

        public override async Task<IEnumerable<Guid>> GetSubscribersAsync(ContentVideo model)
        {
            var contentManager = ContentManager.GetInstance();
            var (isSuccess, content) = await contentManager.TryGetAsync(model.ContentId);
            if (isSuccess)
                return await contentManager.GetSubscribersAsync(content);
            return new List<Guid>();
        }
    }
}

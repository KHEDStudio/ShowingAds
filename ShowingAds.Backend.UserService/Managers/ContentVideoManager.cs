using ShowingAds.Shared.Backend.DataProviders;
using ShowingAds.Shared.Backend.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.UserService.Managers
{
    public sealed class ContentVideoManager : NotifyModelManager<Guid, ContentVideo, ContentVideoManager>, IModelManager<Guid, ContentVideo>
    {
        private ContentVideoManager() : base(new WebProvider<Guid, ContentVideo>(Settings.ContentVideosPath)) { }

        public async Task<IEnumerable<ContentVideo>> GetPermittedModelsAsync(IEnumerable<int> users)
        {
            var manager = ContentManager.GetInstance();
            var contents = await manager.GetCollectionOrNullAsync(x => users.Contains(x.OwnerId));
            return await GetCollectionOrNullAsync(x => contents.Any(y => y.Id == x.ContentId));
        }

        public async Task<bool> TryAddOrUpdateAsync(ContentVideo model) =>
            await TryAddOrUpdateAsync(model.Id, model);

        public async Task<bool> TryDeleteAsync(ContentVideo model) =>
            await TryDeleteAsync(model.Id, model);

        public override async Task<IEnumerable<Guid>> GetSubscribersAsync(ContentVideo model)
        {
            var contentManager = ContentManager.GetInstance();
            var content = await contentManager.GetOrDefaultAsync(model.ContentId);
            if (content != default)
                return await contentManager.GetSubscribersAsync(content);
            return new List<Guid>();
        }
    }
}

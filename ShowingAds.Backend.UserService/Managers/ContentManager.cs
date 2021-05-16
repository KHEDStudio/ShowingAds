using ShowingAds.Shared.Backend.DataProviders;
using ShowingAds.Shared.Backend.Models.Database;
using ShowingAds.Shared.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.UserService.Managers
{
    public sealed class ContentManager : NotifyModelManager<Guid, Content, ContentManager>, IModelManager<Guid, Content>
    {
        private ContentManager() : base(new WebProvider<Guid, Content>(Settings.ContentsPath)) { }

        public async Task<IEnumerable<Content>> GetPermittedModelsAsync(IEnumerable<int> users) =>
            await GetCollectionOrNullAsync(x => users.Contains(x.OwnerId));

        public async Task<bool> TryAddOrUpdateAsync(Content model) =>
            await TryAddOrUpdateAsync(model.Id, model);

        public async Task<bool> TryDeleteAsync(Content model) =>
            await TryDeleteAsync(model.Id, model);

        public override async Task<IEnumerable<Guid>> GetSubscribersAsync(Content model)
        {
            var userManager = UserManager.GetInstance();
            var employerUsers = await userManager.GetEmployerUsers(model.OwnerId);
            return employerUsers.Select(x => x.ToGuid());
        }
    }
}

using ShowingAds.Shared.Backend.DataProviders;
using ShowingAds.Shared.Backend.Models.Database;
using ShowingAds.Shared.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.UserService.Managers
{
    public sealed class ChannelManager : NotifyModelManager<Guid, Channel, ChannelManager>, IModelManager<Guid, Channel>
    {
        private ChannelManager() : base(new WebProvider<Guid, Channel>(Settings.ChannelsPath)) { }

        public async Task<IEnumerable<Channel>> GetPermittedModelsAsync(IEnumerable<int> users) =>
            await GetCollectionOrNullAsync(x => users.Contains(x.OwnerId));

        public async Task<bool> TryAddOrUpdateAsync(Channel model) =>
            await TryAddOrUpdateAsync(model.Id, model);

        public async Task<bool> TryDeleteAsync(Channel model) =>
            await TryDeleteAsync(model.Id, model);

        public override async Task<IEnumerable<Guid>> GetSubscribersAsync(Channel model)
        {
            var userManager = UserManager.GetInstance();
            var employerUsers = await userManager.GetEmployerUsers(model.OwnerId);
            return employerUsers.Select(x => x.ToGuid());
        }
    }
}

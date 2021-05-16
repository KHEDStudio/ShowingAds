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
    public sealed class ChannelManager : NotifyModelManager<Guid, Channel, ChannelManager>, IModelManager<Guid, Channel>
    {
        private Logger _logger { get; }

        private ChannelManager() : base(new WebProvider<Guid, Channel>(Settings.ChannelsPath), Settings.NotifyChannelPath)
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
        }

        public async Task<IEnumerable<Channel>> GetPermittedModelsAsync(IEnumerable<int> users) =>
            await GetCollectionAsync(x => users.Contains(x.OwnerId));

        public async Task<bool> TryAddOrUpdateAsync(Channel model) =>
            await TryAddOrUpdateAsync(model.Id, model);

        public override async Task<IEnumerable<Guid>> GetSubscribersAsync(Channel model)
        {
            var userManager = UserManager.GetInstance();
            var employerUsers = await userManager.GetEmployerUsers(model.OwnerId);
            return employerUsers.Select(x => x.ToGuid());
        }
    }
}

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
    public sealed class ContentManager : NotifyModelManager<Guid, Content, ContentManager>, IModelManager<Guid, Content>
    {
        private Logger _logger { get; }

        private ContentManager() : base(new WebProvider<Guid, Content>(Settings.ContentsPath))
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
            UpdateOrInitializeModels(default, default);
        }

        protected override void UpdateOrInitializeModels(object sender, ElapsedEventArgs e)
        {
            _logger.Info("Initialize Contents...");
            base.UpdateOrInitializeModels(sender, e);
            _syncTimer.Start();
        }

        public async Task<IEnumerable<Content>> GetPermittedModels(List<int> users) =>
            await GetCollection(x => users.Contains(x.OwnerId));

        public async Task<bool> TryAddOrUpdate(Content model) =>
            await TryAddOrUpdate(model.Id, model);
    }
}

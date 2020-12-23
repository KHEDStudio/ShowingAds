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
    public sealed class ContentManager : NotifyModelManager<Guid, Content, ContentManager>, IModelManager<Guid, Content>
    {
        private Logger _logger { get; }

        private ContentManager() : base(new WebProvider<Guid, Content>(Settings.ContentsPath))
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
            EventBus.GetInstance().StartManagersUpdate += UpdateOrInitializeModels;
            UpdateOrInitializeModels();
        }

        protected override void UpdateOrInitializeModels()
        {
            _logger.Info("Update or initialize Contents...");
            base.UpdateOrInitializeModels();
        }

        public async Task<IEnumerable<Content>> GetPermittedModels(List<int> users) =>
            await GetCollection(x => users.Contains(x.OwnerId));

        public async Task<bool> TryAddOrUpdate(Content model) =>
            await TryAddOrUpdate(model.Id, model);
    }
}

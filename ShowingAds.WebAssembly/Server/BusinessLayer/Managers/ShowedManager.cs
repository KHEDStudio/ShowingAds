using Nito.AsyncEx;
using NLog;
using ShowingAds.CoreLibrary.DataProviders;
using ShowingAds.CoreLibrary.Managers;
using ShowingAds.CoreLibrary.Models.Database;
using ShowingAds.WebAssembly.Server.BusinessLayer.Managers.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace ShowingAds.WebAssembly.Server.BusinessLayer.Managers
{
    public sealed class ShowedManager : NotifyModelManager<Guid, Showed, ShowedManager>
    {
        public event Action<Guid> ShowedUpdated;
        private Logger _logger { get; }

        private ShowedManager() : base(new WebProvider<Guid, Showed>(Settings.ShowedsPath))
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
            UpdateOrInitializeModels(default, default);
        }

        protected override void UpdateOrInitializeModels(object sender, ElapsedEventArgs e)
        {
            _logger.Info("Initialize Showeds...");
            base.UpdateOrInitializeModels(sender, e);
            _syncTimer.Start();
        }
    }
}

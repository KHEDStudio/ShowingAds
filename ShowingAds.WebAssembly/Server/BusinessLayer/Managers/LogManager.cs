using Microsoft.Extensions.Logging;
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
    public sealed class LogManager : NotifyModelManager<Guid, Log, LogManager>
    {
        public event Action<Guid> LogUpdated;
        private Logger _logger { get; }

        private LogManager() : base(new WebProvider<Guid, Log>(Settings.LogsPath))
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
            UpdateOrInitializeModels(default, default);
        }

        protected override void UpdateOrInitializeModels(object sender, ElapsedEventArgs e)
        {
            _logger.Info("Initialize Logs...");
            base.UpdateOrInitializeModels(sender, e);
            _syncTimer.Start();
        }
    }
}

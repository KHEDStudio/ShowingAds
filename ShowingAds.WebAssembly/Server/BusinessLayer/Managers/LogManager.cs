using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using NLog;
using ShowingAds.CoreLibrary;
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
        private Logger _logger { get; }

        private LogManager() : base(new WebProvider<Guid, Log>(Settings.LogsPath), Settings.NotifyChannelPath)
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
        }

        public override async Task<IEnumerable<Guid>> GetSubscribersAsync(Log model)
        {
            var deviceManager = DeviceManager.GetInstance();
            var (isSuccess, device) = await deviceManager.TryGetAsync(model.DeviceId);
            if (isSuccess)
                return await deviceManager.GetSubscribersAsync(device);
            return new List<Guid>();
        }
    }
}

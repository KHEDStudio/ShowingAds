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
    public sealed class ShowedManager : NotifyModelManager<Guid, Showed, ShowedManager>
    {
        private Logger _logger { get; }

        private ShowedManager() : base(new WebProvider<Guid, Showed>(Settings.ShowedsPath), Settings.NotifyChannelPath)
        {
            _logger = NLog.LogManager.GetCurrentClassLogger();
        }

        public override async Task<IEnumerable<Guid>> GetSubscribersAsync(Showed model)
        {
            var advertisingVideoManager = AdvertisingVideoManager.GetInstance();
            var (isSuccess, advertisingVideo) = await advertisingVideoManager.TryGetAsync(model.DeviceId);
            if (isSuccess)
                return await advertisingVideoManager.GetSubscribersAsync(advertisingVideo);
            return new List<Guid>();
        }
    }
}

using Nito.AsyncEx;
using ShowingAds.CoreLibrary.DataProviders;
using ShowingAds.CoreLibrary.Managers;
using ShowingAds.CoreLibrary.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace ShowingAds.WebAssembly.Client.BusinessLayer.Managers
{
    public sealed class AdvertisingVideoManager : WebAssemblyModelManager<Guid, AdvertisingVideo, AdvertisingVideoManager>
    {
        private AdvertisingVideoManager() : base(new WebProvider<Guid, AdvertisingVideo>(Settings.AdvertisingVideosPath)) { }
    }
}

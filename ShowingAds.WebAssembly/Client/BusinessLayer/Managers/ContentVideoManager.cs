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
    public sealed class ContentVideoManager : WebAssemblyModelManager<Guid, ContentVideo, ContentVideoManager>
    {
        private ContentVideoManager() : base(new WebProvider<Guid, ContentVideo>(Settings.ContentVideosPath)) { }
    }
}

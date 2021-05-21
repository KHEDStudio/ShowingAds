using ShowingAds.Shared.Backend.DataProviders;
using ShowingAds.Shared.Backend.Managers;
using ShowingAds.Shared.Core.Models.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.DeviceService.Managers
{
    public class ChannelJsonManager : CachedModelManager<Guid, ChannelJson, ChannelJsonManager>
    {
        public ChannelJsonManager() : base(new EmptyProvider<Guid, ChannelJson>()) { }
    }
}

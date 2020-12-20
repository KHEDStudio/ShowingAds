using Nito.AsyncEx;
using ShowingAds.CoreLibrary.DataProviders;
using ShowingAds.CoreLibrary.Managers;
using ShowingAds.CoreLibrary.Models.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;

namespace ShowingAds.WebAssembly.Client.BusinessLayer.Managers
{
    public class DeviceManager : WebAssemblyModelManager<Guid, DeviceState, DeviceManager>
    {
        private DeviceManager() : base(new WebProvider<Guid, DeviceState>(Settings.DevicesPath)) { }
    }
}

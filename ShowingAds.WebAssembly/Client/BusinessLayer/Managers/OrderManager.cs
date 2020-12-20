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
    public sealed class OrderManager : WebAssemblyModelManager<Guid, Order, OrderManager>
    {
        private OrderManager() : base(new WebProvider<Guid, Order>(Settings.OrdersPath)) { }
    }
}

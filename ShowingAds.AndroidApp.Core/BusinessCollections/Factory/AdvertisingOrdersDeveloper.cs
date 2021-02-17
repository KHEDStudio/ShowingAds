using ShowingAds.AndroidApp.Core.BusinessCollections.Models;
using ShowingAds.AndroidApp.Core.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.AndroidApp.Core.BusinessCollections.Factory
{
    public class AdvertisingOrdersDeveloper : Developer<ClientOrder>
    {
        public override TopLevelCollection<ClientOrder> Create()
        {
            var store = new ConfigFileStore<List<ClientOrder>>(Settings.GetConfigFilePath("orders.config"));
            return new TopLevelCollection<ClientOrder>(store);
        }
    }
}

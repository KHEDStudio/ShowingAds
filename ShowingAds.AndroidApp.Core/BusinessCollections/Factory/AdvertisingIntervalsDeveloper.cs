using ShowingAds.AndroidApp.Core.BusinessCollections.Models;
using ShowingAds.AndroidApp.Core.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.AndroidApp.Core.BusinessCollections.Factory
{
    public class AdvertisingIntervalsDeveloper : Developer<ClientInterval>
    {
        public override TopLevelCollection<ClientInterval> Create()
        {
            var store = new ConfigFileStore<List<ClientInterval>>(Settings.GetConfigFilePath("intervals.config"));
            return new TopLevelCollection<ClientInterval>(store);
        }
    }
}

using ShowingAds.AndroidApp.Core.BusinessCollections.Models;
using ShowingAds.AndroidApp.Core.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.AndroidApp.Core.BusinessCollections.Factory
{
    public class AdvertisingClientsDeveloper : Developer<LowLevelCollection<Video>>
    {
        public override TopLevelCollection<LowLevelCollection<Video>> Create()
        {
            var store = new ConfigFileStore<List<LowLevelCollection<Video>>>(Settings.GetConfigFilePath("clients.config"));
            return new TopLevelCollection<LowLevelCollection<Video>>(store);
        }
    }
}

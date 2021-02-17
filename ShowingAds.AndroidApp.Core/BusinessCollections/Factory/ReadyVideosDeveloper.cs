using ShowingAds.AndroidApp.Core.BusinessCollections.Models;
using ShowingAds.AndroidApp.Core.DataAccess;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.AndroidApp.Core.BusinessCollections.Factory
{
    public class ReadyVideosDeveloper : Developer<Video>
    {
        public override TopLevelCollection<Video> Create()
        {
            var store = new ConfigFileStore<List<Video>>(Settings.GetVideoFilesPath("ready.config"));
            return new TopLevelCollection<Video>(store);
        }
    }
}

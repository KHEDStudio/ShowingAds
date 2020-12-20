using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.WebAssembly.Client
{
    public static class Settings
    {
        public static Uri ServerPath { get; set; }
        public static Uri SessionPath => new Uri(ServerPath, "session/");
        public static Uri UsersPath => new Uri(ServerPath, "user/");
        public static Uri ChannelsPath => new Uri(ServerPath, "channel/");
        public static Uri LogsPath => new Uri(ServerPath, "log/");
        public static Uri ContentsPath => new Uri(ServerPath, "content/");
        public static Uri ContentVideosPath => new Uri(ServerPath, "contentvideo/");
        public static Uri OrdersPath => new Uri(ServerPath, "order/");
        public static Uri ClientChannelsPath => new Uri(ServerPath, "clientchannel/");
        public static Uri AdvertisingClientsPath => new Uri(ServerPath, "advertisingclient/");
        public static Uri ShowedsPath => new Uri(ServerPath, "showed/");
        public static Uri AdvertisingVideosPath => new Uri(ServerPath, "advertisingvideo/");
        public static Uri DevicesPath => new Uri(ServerPath, "device/");
        public static Uri NotifyPath { get; set; }
        public static Uri FileServicePath { get; set; }
        public static Uri LogoPath => new Uri(FileServicePath, "logo/");
        public static Uri VideoPath => new Uri(FileServicePath, "video/");
    }
}

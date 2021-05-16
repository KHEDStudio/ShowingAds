using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.WebAssembly.Server
{
    public static class Settings
    {
        public static Uri DjangoPath { get; set; }
        public static Uri SessionPath => new Uri(DjangoPath, "session/");
        public static Uri UsersPath => new Uri(DjangoPath, "users/");
        public static Uri ChannelsPath => new Uri(DjangoPath, "channels/");
        public static Uri LogsPath => new Uri(DjangoPath, "logs/");
        public static Uri ContentsPath => new Uri(DjangoPath, "contents/");
        public static Uri ContentVideosPath => new Uri(DjangoPath, "videos/");
        public static Uri OrdersPath => new Uri(DjangoPath, "orders/");
        public static Uri ClientChannelsPath => new Uri(DjangoPath, "client_channels/");
        public static Uri AdvertisingClientsPath => new Uri(DjangoPath, "clients/");
        public static Uri ShowedsPath => new Uri(DjangoPath, "showeds/");
        public static Uri AdvertisingVideosPath => new Uri(DjangoPath, "ads_videos/");
        public static Uri NotifyPath { get; set; }
        public static Uri NotifyChannelPath => new Uri(NotifyPath, "channel/");
        public static Uri NotifyDevicePath => new Uri(NotifyPath, "device/");
        public static Uri DevicesServicePath { get; set; }
        public static Uri DevicesPath => new Uri(DevicesServicePath, "device/");
        public static Uri ChannelPath => new Uri(DevicesPath, "channel/");
    }
}

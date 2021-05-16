using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.UserService
{
    public static class Settings
    {
        public static IBus RabbitMq { get; set; }
        public static Uri RabbitMqPath { get; set; }
        public static string RabbitMqUsername { get; set; }
        public static string RabbitMqPassword { get; set; }
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
    }
}

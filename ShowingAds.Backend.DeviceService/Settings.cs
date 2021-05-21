using MassTransit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.DeviceService
{
    public class Settings
    {
        public static IBus RabbitMq { get; set; }
        public static Uri RabbitMqPath { get; set; }
        public static string RabbitMqUsername { get; set; }
        public static string RabbitMqPassword { get; set; }
        public static Uri DjangoPath { get; set; }
        public static Uri SessionPath => new Uri(DjangoPath, "session/");
        public static Uri UsersPath => new Uri(DjangoPath, "users/");
        public static Uri DevicesPath => new Uri(DjangoPath, "devices/");
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.NotifyService
{
    public class Settings
    {
        public static Uri RabbitMqPath { get; set; }
        public static string RabbitMqUsername { get; set; }
        public static string RabbitMqPassword { get; set; }
    }
}

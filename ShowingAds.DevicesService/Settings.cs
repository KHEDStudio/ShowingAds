using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.DevicesService
{
    public static class Settings
    {
        public static Uri DjangoPath { get; set; }
        public static Uri SessionPath => new Uri(DjangoPath, "session/");
        public static Uri DevicesPath => new Uri(DjangoPath, "devices/");
        public static Uri BlazorPath { get; set; }
        public static Uri DevicePath => new Uri(BlazorPath, "device/");
        public static Uri ChannelPath => new Uri(DevicePath, "channel");
        public static Uri UpdatePath => new Uri(DevicePath, "update/");
        public static Uri NotifyPath { get; set; }
        public static Uri NotifyChannelPath => new Uri(NotifyPath, "channel");
    }
}

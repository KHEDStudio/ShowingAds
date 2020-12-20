using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.NotifyService
{
    public class Settings
    {
        public static Uri DevicesService { get; set; }
        public static Uri StatusPath => new Uri(DevicesService, "status");
    }
}

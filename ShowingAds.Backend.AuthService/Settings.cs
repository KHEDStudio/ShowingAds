using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.AuthService
{
    public static class Settings
    {
        public static Uri DjangoPath { get; set; }
        public static Uri SessionPath => new(DjangoPath, "session/");
    }
}

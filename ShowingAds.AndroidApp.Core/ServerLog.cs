using Newtonsoft.Json;
using ShowingAds.AndroidApp.Core.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ShowingAds.AndroidApp.Core
{
    public static class ServerLog
    {
        private static string IdKey = "id";
        private static string LogKey = "log";

        public static BlockingCollection<string> ErrorLogs = new BlockingCollection<string>();

        public static void Debug(string tag, string msg)
        {
            Console.WriteLine(tag.WhitePlus(msg).WithTAG(DateTime.UtcNow.ToString()));
        }

        public static void Error(string tag, string msg)
        {
            var log = tag.WhitePlus(msg)
                .WithTAG(DateTime.UtcNow.ToString());
            Console.WriteLine(log);
            if (Settings.DeviceId != Guid.Empty)
            {
                try
                {
                    LogToServer(log);
                }
                catch { }
            }
        }

        private static void LogToServer(string msg)
        {
            ErrorLogs.Add(msg);
            //using (var httpClient = new HttpClient())
            //{
            //    var values = new Dictionary<string, string>();
            //    values.Add(IdKey, Settings.DeviceId.ToString());
            //    values.Add(LogKey, msg);
            //    var content = new StringContent(JsonConvert.SerializeObject(values));
            //    httpClient.PostAsync(Settings.LogServer, content).Wait();
            //}
        }
    }
}

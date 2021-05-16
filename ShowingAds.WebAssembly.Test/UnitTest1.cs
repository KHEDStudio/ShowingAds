using Newtonsoft.Json;
using NUnit.Framework;
using ShowingAds.CoreLibrary.Models.Json;
using ShowingAds.WebAssembly.Server;
using ShowingAds.WebAssembly.Server.BusinessLayer;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.WebAssembly.Test
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            Settings.DjangoPath = new Uri("http://31.184.219.123:3665/");
            Settings.NotifyPath = new Uri("http://127.0.0.1/");
            Settings.DevicesServicePath = new Uri("http://127.0.0.1/");
        }

        [Test]
        public async Task Test1()
        {
            ChannelJson lastChannel = default;
            var constructor = new ChannelJsonConstructor();
            while (true)
            {
                var channelJson = await constructor.ConstructAsJson(Guid.Parse("e2e2254b-a049-41bc-bc4b-8740f8ae48ec"));
                var channel = JsonConvert.DeserializeObject<ChannelJson>(channelJson);
                if (lastChannel == default || lastChannel.Clients.Count != channel.Clients.Count)
                {
                    lastChannel = channel;
                }
            }
        }
    }
}
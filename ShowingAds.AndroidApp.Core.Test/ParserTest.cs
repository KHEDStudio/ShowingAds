using NUnit.Framework;
using ShowingAds.AndroidApp.Core.Network;
using ShowingAds.AndroidApp.Core.Network.Interfaces;
using ShowingAds.AndroidApp.Core.Network.Parsers;
using ShowingAds.CoreLibrary.Models.Login;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ShowingAds.AndroidApp.Core.Test
{
    public class ParserTest
    {
        private readonly string DeviceName = "VSTest";
        private readonly string UserName = "root";
        private readonly string Password = "2000muse";

        private IClient _client;
        private JsonParser _parser;

        [SetUp]
        public void Setup()
        {
            Settings.DeviceId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            var login = new LoginDevice(DeviceName, Settings.DeviceId, UserName, Password);
            var loginer = new NetworkLoginer();
            var status = loginer.TryLogin(login);
            if (status != Network.Enums.LoginStatus.SuccessLogin)
                Assert.Fail("Fail login request");
            _parser = new JsonParser();
            _parser.AdvertisingIntervalsParsed += Intervals;
            _parser.AdvertisingOrdersParsed += Orders;
            _parser.AdvertisingParsed += Clients;
            _parser.ContentsParsed += Contents;
            _parser.LogotypesParsed += Logotypes;
            _parser.RebootTimeParsed += ReloadTime;
            _parser.TickerParsed += Ticker;
            _client = loginer.GetClient(_parser, TimeSpan.FromSeconds(5));
        }

        [Test]
        public void GetParsedDataTest()
        {
            _client.SendRequestAsync();
            Assert.Pass();
        }

        private void Ticker(string arg1, TimeSpan arg2) => ServerLog.Debug("Event", "Ticker");

        private void ReloadTime(TimeSpan obj) => ServerLog.Debug("Event", "ReloadTime");

        private void Logotypes(Network.WebClientCommands.Filters.BaseFilter obj) => ServerLog.Debug("Event", "Logotypes");

        private void Contents(Network.WebClientCommands.Filters.BaseFilter obj) => ServerLog.Debug("Event", "Contents");

        private void Clients(Network.WebClientCommands.Filters.BaseFilter obj) => ServerLog.Debug("Event", "Clients");

        private void Orders(Network.WebClientCommands.Filters.BaseFilter obj) => ServerLog.Debug("Event", "Orders");

        private void Intervals(Network.WebClientCommands.Filters.BaseFilter obj) => ServerLog.Debug("Event", "Intervals");

        [TearDown]
        public async Task Down()
        {
            await _client.DisposeAsync();
        }
    }
}

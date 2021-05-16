using Newtonsoft.Json;
using NUnit.Framework;
using ShowingAds.AndroidApp.Core.Network;
using ShowingAds.AndroidApp.Core.Network.Enums;
using ShowingAds.AndroidApp.Core.Network.Parsers.Interfaces;
using ShowingAds.CoreLibrary.Models.Json;
using ShowingAds.CoreLibrary.Models.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static ShowingAds.AndroidApp.Core.Test.ClientTest;

namespace ShowingAds.AndroidApp.Core.Test
{
    public class LoginerTest
    {
        private readonly string DeviceName = "VSTest";
        private readonly string UserName = "root";
        private readonly string Password = "2000muse";

        private LoginDevice _login;
        private LoginDevice _fakeLogin;

        [SetUp]
        public void Setup()
        {
            Settings.DeviceId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            _login = new LoginDevice(DeviceName, Settings.DeviceId, UserName, Password);
            _fakeLogin = new LoginDevice(string.Empty, Settings.DeviceId, string.Empty, string.Empty);
        }

        [Test]
        public void TestUpdate()
        {
            var loginer = new NetworkLoginer();
            loginer.TryLogin(_login);
            var client = loginer.GetClient(new Parser(), TimeSpan.FromSeconds(1));
            while (true)
            {
                try
                {
                    client.SendRequest();
                    Task.Delay(1000).Wait();
                }
                catch { }
            }
        }

        private class Parser : IParser
        {
            private ChannelJson _lastChannel = default;

            public void Parse(string json)
            {
                var channel = JsonConvert.DeserializeObject<ChannelJson>(json);
                if (_lastChannel == default || channel.Clients.Count != _lastChannel.Clients.Count)
                {
                    _lastChannel = channel;
                }
            }
        }

        [Test]
        public void SuccessLoginTest()
        {
            var parser = new FakeParser();
            var loginer = new NetworkLoginer();
            var status = loginer.TryLogin(_login);
            Assert.AreEqual(LoginStatus.SuccessLogin, status);
        }

        [Test]
        public void NotFoundLoginTest()
        {
            var parser = new FakeParser();
            var loginer = new NetworkLoginer();
            var status = loginer.TryLogin(_fakeLogin);
            Assert.AreEqual(LoginStatus.NotCorrectLoginData, status);
        }
    }
}

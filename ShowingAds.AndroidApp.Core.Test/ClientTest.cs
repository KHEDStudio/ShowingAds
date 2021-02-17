using Microsoft.AspNetCore.SignalR.Client;
using NUnit.Framework;
using ShowingAds.AndroidApp.Core.Network;
using ShowingAds.AndroidApp.Core.Network.Enums;
using ShowingAds.AndroidApp.Core.Network.Interfaces;
using ShowingAds.AndroidApp.Core.Network.Parsers.Interfaces;
using ShowingAds.CoreLibrary.Models.Login;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ShowingAds.AndroidApp.Core.Test
{
    public class ClientTest
    {
        private readonly string DeviceName = "VSTest";
        private readonly string UserName = "root";
        private readonly string Password = "2000muse";

        private IClient _client;
        private LoginDevice _login;
        private AutoResetEvent _lockEvent;

        [SetUp]
        public void Setup()
        {
            Settings.DeviceId = Guid.Parse("11111111-1111-1111-1111-111111111111");
            _login = new LoginDevice(DeviceName, Settings.DeviceId, UserName, Password);
            _lockEvent = new AutoResetEvent(false);
        }

        [Test]
        public async Task GetClientResponseTest()
        {
            var parser = new FakeParser();
            var loginer = new NetworkLoginer();
            var status = await loginer.TryLoginAsync(_login);
            _client = loginer.GetClient(parser, TimeSpan.Zero);
            await _client.SendRequest();
            if (status != LoginStatus.SuccessLogin)
                Assert.Fail();
            await Task.Delay(TimeSpan.FromSeconds(5));
            Assert.AreEqual(HubConnectionState.Connected,
                (_client as NetworkClient).ConnectionState);
        }

        [Test]
        public async Task TimerRequestTest()
        {
            var parser = new FakeParser(_lockEvent);
            var loginer = new NetworkLoginer();
            var status = await loginer.TryLoginAsync(_login);
            _client = loginer.GetClient(parser, TimeSpan.Zero);
            var watch = new Stopwatch();
            var interval = TimeSpan.FromSeconds(5);
            watch.Start();
            _client.StartPeriodicTimerRequest(interval);
            _lockEvent.WaitOne();
            watch.Stop();
            Assert.Less(interval, watch.Elapsed);
        }

        internal class FakeParser : IParser
        {
            private AutoResetEvent _autoResetEvent;

            internal FakeParser() { }

            internal FakeParser(AutoResetEvent autoResetEvent)
            {
                _autoResetEvent = autoResetEvent;
            }

            public void Parse(string json)
            {
                Assert.AreNotEqual(string.Empty, json);
                if (_autoResetEvent != null)
                    _autoResetEvent.Set();
            }
        }

        [TearDown]
        public async Task Down()
        {
            _lockEvent.Close();
            await _client.DisposeAsync();
        }
    }
}

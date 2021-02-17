using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace ShowingAds.AndroidApp.Core.Test
{
    public class ServerLogTest
    {
        private readonly string Tag = "tag";
        private readonly string Body = "body";

        [SetUp]
        public void Setup()
        {
            Settings.DeviceId = Guid.NewGuid();
        }

        [Test]
        public async Task ErrorTets()
        {
            ServerLog.Debug(Tag, Body);
            await ServerLog.Error(Tag, Body);
            Assert.Pass();
        }
    }
}
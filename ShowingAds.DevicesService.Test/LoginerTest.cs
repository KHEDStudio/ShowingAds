using NUnit.Framework;
using ShowingAds.CoreLibrary.Models.Login;
using ShowingAds.DevicesService.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ShowingAds.DevicesService.Test
{
    public class LoginerTest
    {
        private BusinessLayer.Loginer _login { get; set; }
        
        [SetUp]
        public void SetUp()
        {
            Settings.DjangoPath = new Uri("http://localhost:3665/");
            _login = BusinessLayer.Loginer.GetInstance();
        }

        [Test]
        public async Task CorrectLogin()
        {
            var login = DeviceManagerTest
                .GetLoginDevice("LoginTest", Guid.Parse("7c9e6679-7425-40de-944b-e07fc1f90ae7"), "admin", "1");
            var isSuccess = await _login.TryLogin(login);
            Assert.IsTrue(isSuccess);
        }

        [Test]
        public async Task NotCorrectLogin()
        {
            var login = DeviceManagerTest
                .GetLoginDevice("LoginTest", Guid.Parse("7c9e6679-7425-40de-944b-e07fc1f90ae7"), "admin", "2");
            var isSuccess = await _login.TryLogin(login);
            Assert.IsFalse(isSuccess);
        }
    }
}

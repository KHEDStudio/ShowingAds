using NUnit.Framework;
using ShowingAds.CoreLibrary.Models.Login;
using ShowingAds.CoreLibrary.Models.States;
using ShowingAds.DevicesService.BusinessLayer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.DevicesService.Test
{
    public class DeviceManagerTest
    {
        private BusinessLayer.DeviceManager _manager { get; set; }
        private List<DeviceState> _devices { get; set; }

        [SetUp]
        public void SetUp()
        {
            Settings.DjangoPath = new Uri("http://localhost:3665/");
            _devices = new List<DeviceState>();
            for (int i = 0; i < 10; i++)
                _devices.Add(new DeviceState(
                    GetLoginDevice(Path.GetRandomFileName(), Guid.NewGuid(), "admin", "1"), 1));
            _manager = BusinessLayer.DeviceManager.GetInstance();
        }

        [Test]
        public async void AddUpdateDeleteDeviceTest()
        {
            var tasks = _devices.Select(async x => await _manager.TryAddOrUpdateAsync(x.Id, x)).ToList();
            var result = await Task.WhenAll(tasks);
            Assert.IsTrue(result.All(x => x));
            tasks = _devices.Select(async x => await _manager.TryAddOrUpdateAsync(x.Id, x)).ToList();
            result = await Task.WhenAll(tasks);
            Assert.IsTrue(result.All(x => x));
            tasks = _devices.Select(async x => (await _manager.TryDeleteAsync(x.Id)).Item1).ToList();
            result = await Task.WhenAll(tasks);
            Assert.IsTrue(result.All(x => x));
        }

        public static LoginDevice GetLoginDevice(string name, Guid id, string username, string password)
        {
            return new LoginDevice(name, id, username, password);
        }
    }
}

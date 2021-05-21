using Microsoft.Extensions.Hosting;
using ShowingAds.Backend.DeviceService.Managers;
using ShowingAds.Shared.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ShowingAds.Backend.DeviceService.Services
{
    public class DeviceStatusService : BackgroundService
    {
        private readonly System.Timers.Timer _serviceTimer;

        public DeviceStatusService()
        {
            var random = new Random();
            var secondsInterval = random.Next(60, 120);

            _serviceTimer = new System.Timers.Timer();
            _serviceTimer.Elapsed += CheckDeviceStatus;
            _serviceTimer.AutoReset = true;
            _serviceTimer.Interval = TimeSpan.FromSeconds(secondsInterval).TotalMilliseconds;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _serviceTimer.Start();
            return Task.CompletedTask;
        }

        private async void CheckDeviceStatus(object sender, System.Timers.ElapsedEventArgs e)
        {
            var manager = DeviceManager.GetInstance();
            var devices = await manager.GetCollectionOrNullAsync(x => true);
            if (devices != null)
            {
                var now = DateTime.UtcNow;
                foreach (var device in devices)
                {
                    if (device.LastOnline.Add(TimeSpan.FromMinutes(1)) < now)
                    {
                        device.DeviceStatus = DeviceStatus.Offline;
                        await manager.TryAddOrUpdateAsync(device.Id, device);
                    }
                }
            }
        }
    }
}

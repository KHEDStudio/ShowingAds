using Microsoft.Extensions.Hosting;
using ShowingAds.Backend.DeviceService.Managers;
using ShowingAds.Shared.Core.Enums;
using ShowingAds.Shared.Core.Models.States;
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
        private readonly DiagnosticInfoManager _infoManager = DiagnosticInfoManager.GetInstance();

        public DeviceStatusService()
        {
            var random = new Random();
            var secondsInterval = random.Next(120, 180);

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
                    if (device.LastOnline.Add(TimeSpan.FromMinutes(3)) < now)
                    {
                        var info = await _infoManager.GetOrDefaultAsync(device.Id);
                        if (info == default || info.DeviceStatus.HasFlag(DeviceStatus.Offline) == false)
                        {
                            info ??= new DiagnosticInfo(device.Id);
                            info.DeviceStatus = DeviceStatus.Offline;
                            await _infoManager.TryAddOrUpdateAsync(device.Id, info);
                            await manager.TryAddOrUpdateWithoutProviderAsync(device.Id, device);
                        }
                    }
                }
            }
        }
    }
}

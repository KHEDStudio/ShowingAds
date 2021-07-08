using MassTransit;
using Newtonsoft.Json;
using ShowingAds.Backend.DeviceService.Managers;
using ShowingAds.Shared.Backend.Enums;
using ShowingAds.Shared.Backend.Models.DeviceService;
using ShowingAds.Shared.Core.Models.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.DeviceService.Consumers
{
    public class ChannelJsonConsumer : IConsumer<NotifyChannelJson>
    {
        private readonly DeviceTasksManager _tasksManager = DeviceTasksManager.GetInstance();

        public async Task Consume(ConsumeContext<NotifyChannelJson> context)
        {
            var notify = context.Message;
            var manager = ChannelJsonManager.GetInstance();
            if (notify.Operation == Operation.CreateOrUpdate)
                await manager.TryAddOrUpdateAsync(notify.Channel.Id, notify.Channel);
            else
                await manager.TryDeleteAsync(notify.Channel.Id, notify.Channel);
            await UpdateDevicesAsync(notify.Channel.Id);
        }

        private async Task UpdateDevicesAsync(Guid channel)
        {
            var manager = DeviceManager.GetInstance();
            var devices = await manager.GetCollectionOrNullAsync(x => x.ChannelId == channel);
            foreach (var device in devices)
            {
                var tasks = await _tasksManager.GetOrDefaultAsync(device.Id) ?? new DeviceTasks(device.Id);
                await _tasksManager.TryAddOrUpdateAsync(device.Id, new DeviceTasks(tasks.Id, tasks.PriorityAdvertisingClient, true, tasks.TakeScreenshot, tasks.Reboot));
            }
        }
    }
}

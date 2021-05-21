using MassTransit;
using ShowingAds.Backend.DeviceService.Managers;
using ShowingAds.Shared.Backend.Enums;
using ShowingAds.Shared.Backend.Models.DeviceService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.DeviceService.Consumers
{
    public class ChannelJsonConsumer : IConsumer<NotifyChannelJson>
    {
        public async Task Consume(ConsumeContext<NotifyChannelJson> context)
        {
            var notify = context.Message;
            var manager = ChannelJsonManager.GetInstance();
            if (notify.Operation == Operation.CreateOrUpdate)
                await manager.TryAddOrUpdateAsync(notify.Channel.Id, notify.Channel);
            else
                await manager.TryDeleteAsync(notify.Channel.Id, notify.Channel);
        }
    }
}

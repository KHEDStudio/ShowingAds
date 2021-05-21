using MassTransit;
using ShowingAds.Backend.UserService.ChannelJsonHandlers;
using ShowingAds.Backend.UserService.Managers;
using ShowingAds.Shared.Backend.Enums;
using ShowingAds.Shared.Backend.Models.DeviceService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.UserService.Consumers
{
    public class ChannelJsonUpdaterConsumer : IConsumer<UpdateChannelJson>
    {
        public async Task Consume(ConsumeContext<UpdateChannelJson> context)
        {
            var manager = ChannelManager.GetInstance();
            var constructor = ChannelJsonConstructor.GetInstance();
            var channels = await manager.GetCollectionOrNullAsync(x => true);
            var count = channels.ToList();
            foreach (var channel in channels)
            {
                var channelJson = await constructor.ConstructAsync(channel.Id);
                await Settings.RabbitMq.Publish(
                    new NotifyChannelJson(Operation.CreateOrUpdate, channelJson));
            }
        }
    }
}

using MassTransit;
using ShowingAds.Backend.NotifyService.Managers;
using ShowingAds.Shared.Backend.Models.NotifyService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.NotifyService.Consumers
{
    public class NotifyPacketConsumer : IConsumer<NotifyPacket>
    {
        public Task Consume(ConsumeContext<NotifyPacket> context)
        {
            var manager = SubscriberManager.GetInstance();
            manager.NotifyPacketReceived(context.Message);
            return Task.CompletedTask;
        }
    }
}

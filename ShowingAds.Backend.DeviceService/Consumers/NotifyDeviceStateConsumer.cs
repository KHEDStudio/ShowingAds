using MassTransit;
using Newtonsoft.Json;
using ShowingAds.Backend.DeviceService.Managers;
using ShowingAds.Shared.Backend.Enums;
using ShowingAds.Shared.Backend.Managers;
using ShowingAds.Shared.Backend.Models.NotifyService;
using ShowingAds.Shared.Backend.Models.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.DeviceService.Consumers
{
    public class NotifyDeviceStateConsumer : IConsumer<NotifyPacket>
    {
        public Task Consume(ConsumeContext<NotifyPacket> context)
        {
            if (context.Message.ModelType == nameof(DeviceState))
            {
                var deviceState = JsonConvert.DeserializeObject<DeviceState>(context.Message.Model);
                var manager = DeviceManager.GetInstance() as CachedModelManager<Guid, DeviceState, DeviceManager>;
                if (context.Message.Operation == Operation.CreateOrUpdate)
                    manager.TryAddOrUpdateWithoutProvider(deviceState.Id, deviceState);
                else
                    manager.TryDeleteWithoutProvider(deviceState.Id, deviceState);
            }
            return Task.CompletedTask;
        }
    }
}

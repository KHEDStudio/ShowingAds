using MassTransit;
using ShowingAds.Backend.UserService.Managers;
using ShowingAds.Shared.Backend.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.UserService.Controllers
{
    public class ChannelController : BaseModelController<Guid, Channel, ChannelManager>
    {
        public ChannelController(IBus bus) : base(ChannelManager.GetInstance()) => Settings.RabbitMq = bus;
    }
}

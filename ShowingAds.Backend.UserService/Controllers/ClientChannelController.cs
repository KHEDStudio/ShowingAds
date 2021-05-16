using MassTransit;
using ShowingAds.Backend.UserService.Managers;
using ShowingAds.Shared.Backend.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.UserService.Controllers
{
    public class ClientChannelController : BaseModelController<Guid, ClientChannel, ClientChannelManager>
    {
        public ClientChannelController(IBus bus) : base(ClientChannelManager.GetInstance()) => Settings.RabbitMq = bus;
    }
}

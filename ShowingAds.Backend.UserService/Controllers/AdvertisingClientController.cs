using MassTransit;
using ShowingAds.Backend.UserService.Managers;
using ShowingAds.Shared.Backend.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.UserService.Controllers
{
    public class AdvertisingClientController : BaseModelController<Guid, AdvertisingClient, AdvertisingClientManager>
    {
        public AdvertisingClientController(IBus bus) : base(AdvertisingClientManager.GetInstance()) => Settings.RabbitMq = bus;
    }
}

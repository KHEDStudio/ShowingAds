using MassTransit;
using ShowingAds.Backend.UserService.Managers;
using ShowingAds.Shared.Backend.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.UserService.Controllers
{
    public class AdvertisingVideoController : BaseModelController<Guid, AdvertisingVideo, AdvertisingVideoManager>
    {
        public AdvertisingVideoController(IBus bus) : base(AdvertisingVideoManager.GetInstance()) => Settings.RabbitMq = bus;
    }
}

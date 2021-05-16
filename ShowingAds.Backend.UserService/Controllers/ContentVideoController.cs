using MassTransit;
using ShowingAds.Backend.UserService.Managers;
using ShowingAds.Shared.Backend.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.UserService.Controllers
{
    public class ContentVideoController : BaseModelController<Guid, ContentVideo, ContentVideoManager>
    {
        public ContentVideoController(IBus bus) : base(ContentVideoManager.GetInstance()) => Settings.RabbitMq = bus;
    }
}

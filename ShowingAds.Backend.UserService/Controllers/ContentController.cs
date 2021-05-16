using MassTransit;
using ShowingAds.Backend.UserService.Managers;
using ShowingAds.Shared.Backend.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.UserService.Controllers
{
    public class ContentController : BaseModelController<Guid, Content, ContentManager>
    {
        public ContentController(IBus bus) : base(ContentManager.GetInstance()) => Settings.RabbitMq = bus;
    }
}

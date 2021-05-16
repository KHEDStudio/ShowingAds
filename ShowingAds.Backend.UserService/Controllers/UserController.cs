using MassTransit;
using ShowingAds.Backend.UserService.Managers;
using ShowingAds.Shared.Backend.Models.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.UserService.Controllers
{
    public class UserController : BaseModelController<int, User, UserManager>
    {
        public UserController(IBus bus) : base(UserManager.GetInstance()) => Settings.RabbitMq = bus;
    }
}

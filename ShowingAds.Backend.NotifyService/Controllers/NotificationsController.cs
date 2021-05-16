using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShowingAds.Backend.NotifyService.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.NotifyService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class NotificationsController : Controller
    {
        [HttpGet]
        public IActionResult GetNotifications([FromQuery] string connection)
        {
            var manager = SubscriberManager.GetInstance();
            var subscriberId = User.Claims.First(x => x.Type == "GuidId").Value;
            var notifications = manager.GetNotifications(connection, Guid.Parse(subscriberId));
            return Ok(JsonConvert.SerializeObject(notifications));
        }
    }
}

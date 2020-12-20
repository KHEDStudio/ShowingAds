using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShowingAds.NotifyService.BusinessLayer;
using ShowingAds.NotifyService.BusinessLayer.Interfaces;
using ShowingAds.NotifyService.BusinessLayer.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.NotifyService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class NotifyController : ControllerBase
    {
        private ILogger<NotifyController> _logger { get; }
        private INotifier _notifier { get; }

        public NotifyController(ILogger<NotifyController> logger)
        {
            _logger = logger;
            _notifier = ClientManager.GetInstance();
        }

        [HttpPost("channel/")]
        public async Task<ActionResult> ChannelUpdated([FromQuery] Guid client)
        {
            _logger.LogInformation($"NotifyChannel client {client} from { HttpContext.Connection.RemoteIpAddress}:{ HttpContext.Connection.RemotePort}");
            var (isSuccess, connections) = await _notifier.TryGetConnections(client);
            if (isSuccess)
            {
                foreach (var connectionId in connections)
                    _notifier.AddNotifyTask(new ChannelUpdated(connectionId));
                return StatusCode(StatusCodes.Status200OK);
            } else return StatusCode(StatusCodes.Status404NotFound);
        }
    }
}

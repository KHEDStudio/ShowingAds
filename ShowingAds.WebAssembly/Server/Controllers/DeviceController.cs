using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShowingAds.CoreLibrary.Models.States;
using ShowingAds.WebAssembly.Server.BusinessLayer;
using ShowingAds.WebAssembly.Server.BusinessLayer.Managers;
using ShowingAds.WebAssembly.Server.Controllers.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.WebAssembly.Server.Controllers
{
    public class DeviceController : ModelController<Guid, DeviceState, DeviceManager>
    {
        private ILogger<DeviceController> _logger { get; }

        public DeviceController(ILogger<DeviceController> logger)
            : base(logger, DeviceManager.GetInstance()) => _logger = logger;

        [HttpGet("channel")]
        public async Task<ActionResult> GetChannelJson([FromQuery] Guid channel)
        {
            _logger.LogInformation($"Get channel {channel} from device");
            var manager = ChannelManager.GetInstance();
            var (isExists, _channel) = await manager.TryGetAsync(channel);
            if (isExists)
            {
                var constructor = new ChannelJsonConstructor();
                var json = await constructor.ConstructAsJson(_channel.Id);
                return StatusCode(StatusCodes.Status200OK, json);
            }
            return StatusCode(StatusCodes.Status404NotFound);
        }

        [HttpPost("update")]
        public async Task<ActionResult> UpdateDevice([FromBody] DeviceState device)
        {
            _logger.LogInformation($"Update device {device.Name}");
            var manager = DeviceManager.GetInstance();
            await manager.UpdateModelAsync(device);
            return StatusCode(StatusCodes.Status200OK);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ShowingAds.CoreLibrary.Enums;
using ShowingAds.CoreLibrary.Models.Database;
using ShowingAds.CoreLibrary.Models.States;
using ShowingAds.DevicesService.BusinessLayer;
using ShowingAds.DevicesService.BusinessLayer.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Timers;

namespace ShowingAds.DevicesService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DeviceController : ControllerBase
    {
        private DeviceManager _manager => DeviceManager.GetInstance();
        private ILogger<DeviceController> _logger { get; }

        public DeviceController(ILogger<DeviceController> logger) => _logger = logger;

        [HttpGet("channel")]
        public async Task<ActionResult> GetChannel()
        {
            _logger.LogInformation($"Device {HttpContext.User.Identity.Name} -> {HttpContext.Connection.RemoteIpAddress}:{HttpContext.Connection.RemotePort}");
            var isSuccess = Guid.TryParse(HttpContext.User.Identity.Name, out var deviceId);
            if (isSuccess)
            {
                var (_isSuccess, device) = await _manager.TryGetAsync(deviceId);
                if (_isSuccess)
                {
                    if (device.ChannelId == Guid.Empty)
                        return StatusCode(StatusCodes.Status204NoContent);
                    var channelJson = await _manager.GetChannelJsonAsync(device.ChannelId);
                    return StatusCode(StatusCodes.Status200OK, channelJson);
                }
            }
            return StatusCode(StatusCodes.Status404NotFound);
        }

        [HttpGet]
        public async Task<ActionResult> GetDevices()
        {
            _logger.LogInformation($"Get devices {HttpContext.Connection.RemoteIpAddress}:{HttpContext.Connection.RemotePort}");
            var devices = await _manager.GetCollectionAsync(x => true);
            var json = JsonConvert.SerializeObject(devices);
            return StatusCode(StatusCodes.Status200OK, json);
        }

        [HttpPost("status")]
        public async Task<ActionResult> SetDeviceStatus([FromQuery] DeviceStatus status, Guid device)
        {
            _logger.LogInformation($"Set status device {HttpContext.Connection.RemoteIpAddress}:{HttpContext.Connection.RemotePort}");
            var isSuccess = await _manager.SetDeviceStatusAsync(device, status);
            if (isSuccess)
                return StatusCode(StatusCodes.Status200OK);
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpPost("info")]
        public async Task<ActionResult> SetDiagnosticInfo([FromBody] DiagnosticInfo info)
        {
            _logger.LogInformation($"Set diagnostic info {HttpContext.Connection.RemoteIpAddress}:{HttpContext.Connection.RemotePort}");
            var isSuccess = Guid.TryParse(HttpContext.User.Identity.Name, out var deviceId);
            if (isSuccess)
            {
                isSuccess = await _manager.SetDiagnosticInfoAsync(deviceId, info);
                if (isSuccess)
                    return StatusCode(StatusCodes.Status200OK);
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpPost("content")]
        public async Task<ActionResult> SetContentCount([FromQuery] int count)
        {
            //var filter = RequestsFilter.GetInstance();
            //if (filter.IsBannedDevice(HttpContext.Connection.RemoteIpAddress))
            //    return StatusCode(StatusCodes.Status429TooManyRequests);
            _logger.LogInformation($"Set content count (ver. 1.0) {HttpContext.Connection.RemoteIpAddress}:{HttpContext.Connection.RemotePort}");
            var isSuccess = Guid.TryParse(HttpContext.User.Identity.Name, out var deviceId);
            if (isSuccess)
            {
                var (isExists, device) = await _manager.TryGetAsync(deviceId);
                if (isExists)
                {
                    var info = device.DiagnosticInfo;
                    var newInfo = new DiagnosticInfo("1.0", count, info?.DownloadType ?? 0, info?.DownloadProgress ?? 0, info?.DownloadSpeed ?? 0);
                    if (await _manager.SetDiagnosticInfoAsync(deviceId, newInfo))
                        return StatusCode(StatusCodes.Status200OK);
                }
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpPut]
        public async Task<ActionResult> PutDevice([FromBody] Device device)
        {
            _logger.LogInformation($"Put device ({device.Id}) {HttpContext.Connection.RemoteIpAddress}:{HttpContext.Connection.RemotePort}");
            var isSuccess = await _manager.TryUpdateDeviceAsync(device);
            if (isSuccess)
                return StatusCode(StatusCodes.Status200OK);
            return StatusCode(StatusCodes.Status400BadRequest);
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteDevice([FromQuery] Guid device)
        {
            _logger.LogInformation($"Delete device ({device}) {HttpContext.Connection.RemoteIpAddress}:{HttpContext.Connection.RemotePort}");
            var (isSuccess, _) = await _manager.TryDeleteAsync(device);
            if (isSuccess)
                return StatusCode(StatusCodes.Status200OK);
            return StatusCode(StatusCodes.Status404NotFound);
        }
    }
}

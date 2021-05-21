using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShowingAds.Backend.DeviceService.Managers;
using ShowingAds.Backend.DeviceService.Utils;
using ShowingAds.Shared.Backend.Models.States;
using ShowingAds.Shared.Core.Enums;
using ShowingAds.Shared.Core.Models.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.DeviceService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class DeviceController : Controller
    {
        private readonly DeviceManager _deviceManager = DeviceManager.GetInstance();
        private readonly ChannelJsonManager _channelManager = ChannelJsonManager.GetInstance();

        public DeviceController() { }

        [HttpGet("channel")]
        public async Task<ActionResult> GetChannel()
        {
            var isSuccess = Guid.TryParse(HttpContext.User.Identity.Name, out var deviceId);
            if (isSuccess)
            {
                var device = await _deviceManager.GetOrDefaultAsync(deviceId);
                if (device != default)
                {
                    if (device.ChannelId == Guid.Empty)
                        return NoContent();
                    device.LastOnline = DateTime.UtcNow;
                    device.DeviceStatus &= ~DeviceStatus.Offline;
                    await _deviceManager.TryAddOrUpdateAsync(deviceId, device);
                    var channelJson = await _channelManager.GetOrDefaultAsync(device.ChannelId);
                    if (channelJson != default)
                        return Ok(channelJson);
                }
            }
            return NotFound();
        }

        [HttpPost("info")]
        public async Task<ActionResult> SetDiagnosticInfo([FromBody] DiagnosticInfo info)
        {
            var isSuccess = Guid.TryParse(HttpContext.User.Identity.Name, out var deviceId);
            if (isSuccess)
            {
                isSuccess = await _deviceManager.SetDiagnosticInfoAsync(deviceId, info);
                if (isSuccess)
                    return Ok();
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpPost("content")]
        public async Task<ActionResult> SetContentCount([FromQuery] int count)
        {
            var isSuccess = Guid.TryParse(HttpContext.User.Identity.Name, out var deviceId);
            if (isSuccess)
            {
                var device = await _deviceManager.GetOrDefaultAsync(deviceId);
                if (device != default)
                {
                    var info = device.DiagnosticInfo;
                    var newInfo = new DiagnosticInfo("1.0", count, info?.DownloadType ?? 0, info?.DownloadProgress ?? 0, info?.DownloadSpeed ?? 0);
                    if (await _deviceManager.SetDiagnosticInfoAsync(deviceId, newInfo))
                        return Ok();
                }
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpGet]
        public async Task<ActionResult> GetDevices()
        {
            var isSuccess = Request.Headers.TryGetValue("Authorization", out var authHeaderValue);
            if (isSuccess)
            {
                var token = authHeaderValue[0].Split(' ')[1];
                var validator = new JwtTokenValidator(token);
                if (validator.ValidateToken())
                {
                    var manager = UserManager.GetInstance();
                    var id = validator.GetId();
                    var user = await manager.GetOrDefaultAsync(x => x.Id == id);
                    if (user == default)
                        return Unauthorized();
                    var users = await manager.GetEmployeeUsers(user.Id);
                    var models = await _deviceManager.GetPermittedModelsAsync(users);
                    return Ok(JsonConvert.SerializeObject(models));
                }
            }
            return Unauthorized();
        }

        [HttpPost]
        public async Task<ActionResult> AddOrUpdateModel([FromBody] DeviceState device)
        {
            var isSuccess = await _deviceManager.TryAddOrUpdateAsync(device.Id, device);
            if (isSuccess)
                return Ok();
            return BadRequest();
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteModel([FromBody] DeviceState device)
        {
            var isDeleted = await _deviceManager.TryDeleteAsync(device.Id, device);
            if (isDeleted)
                return Ok();
            return BadRequest();
        }
    }
}

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShowingAds.Backend.DeviceService.Managers;
using ShowingAds.Backend.DeviceService.Utils;
using ShowingAds.Shared.Backend.Models.Database;
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
        private readonly DeviceTasksManager _tasksManager = DeviceTasksManager.GetInstance();
        private readonly DiagnosticInfoManager _infoManager = DiagnosticInfoManager.GetInstance();
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
                    var tasks = await _tasksManager.GetOrDefaultAsync(device.Id) ?? new DeviceTasks(device.Id);
                    await _tasksManager.TryAddOrUpdateAsync(device.Id, new DeviceTasks(tasks.Id, tasks.PriorityAdvertisingClient, false, tasks.TakeScreenshot, tasks.Reboot));

                    var info = await _infoManager.GetOrDefaultAsync(device.Id) ?? new DiagnosticInfo(device.Id);
                    info.DeviceStatus &= ~DeviceStatus.Offline;
                    await _infoManager.TryAddOrUpdateAsync(device.Id, info);

                    device.LastOnline = DateTime.UtcNow;
                    await (_deviceManager as MongoCachedModelManager<Guid, Device, DeviceManager>).TryAddOrUpdateAsync(deviceId, device);

                    if (device.ChannelId == Guid.Empty)
                        return NoContent();
                    var channelJson = await _channelManager.GetOrDefaultAsync(device.ChannelId);
                    if (channelJson != default)
                        return Ok(channelJson);
                }
            }
            return NotFound();
        }

        [HttpPost("screenshot")]
        public async Task<ActionResult> ScreenshotDevice()
        {
            var isSuccess = Guid.TryParse(HttpContext.User.Identity.Name, out var deviceId);
            if (isSuccess)
            {
                var device = await _deviceManager.GetOrDefaultAsync(deviceId);
                if (device != default)
                {
                    var tasks = await _tasksManager.GetOrDefaultAsync(device.Id) ?? new DeviceTasks(device.Id);
                    await _tasksManager.TryAddOrUpdateAsync(device.Id, new DeviceTasks(tasks.Id, tasks.PriorityAdvertisingClient, tasks.IsUpdated, false, tasks.Reboot));

                    var info = await _infoManager.GetOrDefaultAsync(device.Id) ?? new DiagnosticInfo(deviceId);
                    info.DeviceStatus &= ~DeviceStatus.Offline;
                    await _infoManager.TryAddOrUpdateAsync(device.Id, info);

                    device.LastOnline = DateTime.UtcNow;
                    await (_deviceManager as MongoCachedModelManager<Guid, Device, DeviceManager>).TryAddOrUpdateAsync(device.Id, device);
                    return Ok();
                }
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        [HttpPost("info")]
        public async Task<ActionResult> SetDiagnosticInfo([FromBody] DiagnosticInfo info)
        {
            var isSuccess = Guid.TryParse(HttpContext.User.Identity.Name, out var deviceId);
            if (isSuccess)
            {
                info.Id = deviceId;
                info.DeviceStatus &= ~DeviceStatus.Offline;
                await _infoManager.TryAddOrUpdateAsync(deviceId, info);

                var device = await _deviceManager.GetOrDefaultAsync(deviceId);
                if (device != default)
                {
                    device.LastOnline = DateTime.UtcNow;
                    await (_deviceManager as MongoCachedModelManager<Guid, Device, DeviceManager>).TryAddOrUpdateAsync(device.Id, device);
                }

                var tasks = await _tasksManager.GetOrDefaultAsync(deviceId);
                return Ok(JsonConvert.SerializeObject(tasks));
            }
            return StatusCode(StatusCodes.Status500InternalServerError);
        }

        //[HttpPost("content")]
        //public async Task<ActionResult> SetContentCount([FromQuery] int count)
        //{
        //    var isSuccess = Guid.TryParse(HttpContext.User.Identity.Name, out var deviceId);
        //    if (isSuccess)
        //    {
        //        var device = await _deviceManager.GetOrDefaultAsync(deviceId);
        //        if (device != default)
        //        {
        //            var info = device.DiagnosticInfo;
        //            var newInfo = new DiagnosticInfo("1.0", count, info?.ClientVideos ?? new Dictionary<string, int>(), 
        //                info?.ClientVideosShowed ?? new Dictionary<string, int>(), info?.ClientVideosWillShow ?? new Dictionary<string, DateTime>(),
        //                info?.DownloadType ?? 0, info?.DownloadProgress ?? 0, info?.DownloadSpeed ?? 0, info?.CurrentVideo ?? string.Empty,
        //                info?.Screenshot ?? string.Empty, info?.FreeSpaceDisk ?? 0, info?.Logs ?? new List<string>());
        //            await _deviceManager.SetDiagnosticInfoAsync(deviceId, newInfo);
        //            return Ok();
        //        }
        //    }
        //    return StatusCode(StatusCodes.Status500InternalServerError);
        //}

        [HttpPost("tasks")]
        public async Task<ActionResult> SetDeviceTasks([FromBody] DeviceTasks tasks)
        {
            var device = await _tasksManager.GetOrDefaultAsync(tasks.Id);
            if (device != default)
            {
                await _tasksManager.TryAddOrUpdateAsync(device.Id, tasks);
                return Ok();
            }
            return BadRequest();
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
                    var tasks = models.Select(async x =>
                    {
                        var info = await _infoManager.GetOrDefaultAsync(x.Id) ?? new DiagnosticInfo(x.Id);
                        var tasks = await _tasksManager.GetOrDefaultAsync(x.Id) ?? new DeviceTasks(x.Id);
                        return new DeviceState(x, info, tasks);
                    });
                    Task.WaitAll(tasks.ToArray());
                    return Ok(JsonConvert.SerializeObject(tasks.Select(x => x.Result)));
                }
            }
            return Unauthorized();
        }

        [HttpPost]
        public async Task<ActionResult> AddOrUpdateModel([FromBody] Device device)
        {
            var tasks = await _tasksManager.GetOrDefaultAsync(device.Id) ?? new DeviceTasks(device.Id);
            await _tasksManager.TryAddOrUpdateAsync(tasks.Id, new DeviceTasks(tasks.Id, tasks.PriorityAdvertisingClient, true, tasks.TakeScreenshot, tasks.Reboot));
            var isSuccess = await _deviceManager.TryAddOrUpdateAsync(device.Id, device);
            if (isSuccess)
                return Ok();
            return BadRequest();
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteModel([FromBody] Device device)
        {
            var isDeleted = await _deviceManager.TryDeleteAsync(device.Id, device);
            if (isDeleted)
                return Ok();
            return BadRequest();
        }
    }
}

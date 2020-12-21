using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ShowingAds.CoreLibrary.Models.FilesService;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ShowingAds.FilesService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class VideoController : ControllerBase
    {
        private ILogger<VideoController> _logger { get; }
        private readonly string _videoDirectory;

        public VideoController(IConfiguration configuration, ILogger<VideoController> logger)
        {
            _logger = logger;
            _videoDirectory = configuration.GetValue<string>("VideoDirectory") ?? string.Empty;
        }

        [HttpGet]
        public ActionResult GetVideo(Guid video)
        {
            _logger.LogInformation($"Download video ({video}) -> {HttpContext.Connection.RemoteIpAddress}:{HttpContext.Connection.RemotePort}");
            var dirInfo = new DirectoryInfo(_videoDirectory);
            if (dirInfo.Exists == false)
                dirInfo.Create();
            foreach (var fileInfo in dirInfo.GetFiles())
            {
                if (fileInfo.Name.Contains(video.ToString()))
                    return PhysicalFile(fileInfo.FullName, "application/octet-stream", fileInfo.Name);
            }
            return NotFound();
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<ActionResult> PostVideo()
        {
            _logger.LogInformation($"Upload video size {Request.ContentLength} bytes -> {HttpContext.Connection.RemoteIpAddress}:{HttpContext.Connection.RemotePort}");
            var file = Request.Form.Files[0];
            var guid = Guid.NewGuid();
            var extension = Path.GetExtension(file.FileName);
            string filePath = $"{_videoDirectory}/{guid}{extension}";
            _logger.LogInformation($"Save video path: {filePath}");
            try
            {
                if (Directory.Exists(_videoDirectory) == false)
                    Directory.CreateDirectory(_videoDirectory);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                    await file.CopyToAsync(fileStream);
                var response = new FileUploadResponse(guid, GetDurationVideo(filePath));
                var jsonResponse = JsonConvert.SerializeObject(response);
                return StatusCode(StatusCodes.Status201Created, jsonResponse);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.ToString());
                var _file = new FileInfo(filePath);
                if (_file.Exists)
                    _file.Delete();
                return StatusCode(StatusCodes.Status400BadRequest);
            }
        }

        private long GetDurationVideo(string filePath)
        {
            _logger.LogInformation($"Get duration video by path: {filePath}");
            var ffProbe = new NReco.VideoInfo.FFProbe();
            var videoInfo = ffProbe.GetMediaInfo(filePath);
            return videoInfo.Duration.Ticks;
        }

        [HttpDelete]
        public ActionResult DeleteVideo(Guid video)
        {
            _logger.LogInformation($"Delete video ({video}) -> {HttpContext.Connection.RemoteIpAddress}:{HttpContext.Connection.RemotePort}");
            var dirInfo = new DirectoryInfo(_videoDirectory);
            foreach (var fileInfo in dirInfo.GetFiles())
            {
                if (fileInfo.Name.Contains(video.ToString()))
                {
                    fileInfo.Delete();
                    return StatusCode(StatusCodes.Status200OK);
                }
            }
            return StatusCode(StatusCodes.Status404NotFound);
        }
    }
}

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
    public class LogoController : ControllerBase
    {
        private ILogger<LogoController> _logger { get; }
        private readonly string _logoDirectory;

        public LogoController(IConfiguration configuration, ILogger<LogoController> logger)
        {
            _logger = logger;
            _logoDirectory = configuration.GetValue<string>("LogoDirectory") ?? string.Empty;
        }

        [HttpGet]
        public ActionResult GetLogo(Guid logo)
        {
            _logger.LogInformation($"Download logo ({logo}) -> {HttpContext.Connection.RemoteIpAddress}:{HttpContext.Connection.RemotePort}");
            var dirInfo = new DirectoryInfo(_logoDirectory);
            if (dirInfo.Exists == false)
                dirInfo.Create();
            foreach (var fileInfo in dirInfo.GetFiles())
            {
                if (fileInfo.Name.Contains(logo.ToString()))
                    return PhysicalFile(fileInfo.FullName, "application/octet-stream", fileInfo.Name);
            }
            return NotFound();
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<ActionResult> PostLogo()
        {
            _logger.LogInformation($"Upload logo size {Request.ContentLength} bytes -> {HttpContext.Connection.RemoteIpAddress}:{HttpContext.Connection.RemotePort}");
            var file = Request.Form.Files[0];
            var guid = Guid.NewGuid();
            var extension = Path.GetExtension(file.FileName);
            string filePath = $"{_logoDirectory}/{guid}{extension}";
            _logger.LogInformation($"Save logo path: {filePath}");
            try
            {
                if (Directory.Exists(_logoDirectory) == false)
                    Directory.CreateDirectory(_logoDirectory);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                    await file.CopyToAsync(fileStream);
                var response = new FileUploadResponse(guid);
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

        [HttpDelete]
        public ActionResult DeleteLogo(Guid logo)
        {
            _logger.LogInformation($"Delete logo ({logo}) -> {HttpContext.Connection.RemoteIpAddress}:{HttpContext.Connection.RemotePort}");
            var dirInfo = new DirectoryInfo(_logoDirectory);
            foreach (var fileInfo in dirInfo.GetFiles())
            {
                if (fileInfo.Name.Contains(logo.ToString()))
                {
                    fileInfo.Delete();
                    return StatusCode(StatusCodes.Status200OK);
                }
            }
            return StatusCode(StatusCodes.Status404NotFound);
        }
    }
}

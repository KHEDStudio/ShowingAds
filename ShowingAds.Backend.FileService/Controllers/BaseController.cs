using Microsoft.AspNetCore.Mvc;
using ShowingAds.Shared.Core.Models.FileService;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.FileService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public abstract class BaseController : Controller
    {
        private readonly string _directory;

        public BaseController(string directory)
        {
            _directory = directory;
        }

        [HttpGet]
        public virtual ActionResult GetFile([FromQuery] Guid file)
        {
            var dirInfo = new DirectoryInfo(_directory);
            if (dirInfo.Exists == false)
                dirInfo.Create();
            foreach (var fileInfo in dirInfo.GetFiles())
                if (fileInfo.Name.Contains(file.ToString()))
                    return PhysicalFile(fileInfo.FullName, "application/octet-stream", fileInfo.Name);
            return NotFound();
        }

        [HttpPost]
        [DisableRequestSizeLimit]
        public async Task<ActionResult> PostFile()
        {
            var guid = Guid.NewGuid();
            var file = Request.Form.Files[0];
            var extension = Path.GetExtension(file.FileName);
            var filePath = $"{_directory}/{guid}{extension}";
            try
            {
                if (Directory.Exists(_directory) == false)
                    Directory.CreateDirectory(_directory);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                    await file.CopyToAsync(fileStream);
                var response = new FileUploadResponse(guid, GetDuration(filePath));
                return Ok(response);
            }
            catch
            {
                var _file = new FileInfo(filePath);
                if (_file.Exists)
                    _file.Delete();
                return BadRequest();
            }
        }

        protected abstract long GetDuration(string filePath);

        [HttpDelete]
        public virtual ActionResult DeleteFile([FromQuery] Guid file)
        {
            var dirInfo = new DirectoryInfo(_directory);
            foreach (var fileInfo in dirInfo.GetFiles())
            {
                if (fileInfo.Name.Contains(file.ToString()))
                {
                    fileInfo.Delete();
                    return Ok();
                }
            }
            return NotFound();
        }
    }
}

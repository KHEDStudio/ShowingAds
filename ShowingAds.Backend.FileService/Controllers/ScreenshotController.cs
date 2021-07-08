using Microsoft.AspNetCore.Mvc;
using NReco.VideoInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.FileService.Controllers
{
    public class ScreenshotController : BaseController
    {
        public ScreenshotController() : base(Settings.ScreenshotDirectory) { }

        public override ActionResult GetFile([FromQuery] Guid screenshot) => base.GetFile(screenshot);

        protected override long GetDuration(string filePath) => 0;

        public override ActionResult DeleteFile([FromQuery] Guid screenshot) => base.DeleteFile(screenshot);
    }
}

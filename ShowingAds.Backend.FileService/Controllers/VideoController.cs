using Microsoft.AspNetCore.Mvc;
using NReco.VideoInfo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.FileService.Controllers
{
    public class VideoController : BaseController
    {
        public VideoController() : base(Settings.VideoDirectory) { }

        public override ActionResult GetFile([FromQuery] Guid video) => base.GetFile(video);

        protected override long GetDuration(string filePath)
        {
            var ffProbe = new FFProbe();
            var videoInfo = ffProbe.GetMediaInfo(filePath);
            var ticks = videoInfo.Duration.Ticks;
            return ticks;
        }

        public override ActionResult DeleteFile([FromQuery] Guid video) => base.DeleteFile(video);
    }
}

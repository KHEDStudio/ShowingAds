using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.Backend.FileService.Controllers
{
    public class LogoController : BaseController
    {
        public LogoController() : base(Settings.LogoDirectory) { }

        public override ActionResult GetFile([FromQuery] Guid logo) => base.GetFile(logo);

        protected override long GetDuration(string filePath) => 0;

        public override ActionResult DeleteFile([FromQuery] Guid logo) => base.DeleteFile(logo);
    }
}

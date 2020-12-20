using Microsoft.Extensions.Logging;
using ShowingAds.CoreLibrary.Models.Database;
using ShowingAds.WebAssembly.Server.BusinessLayer.Managers;
using ShowingAds.WebAssembly.Server.Controllers.Abstract;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.WebAssembly.Server.Controllers
{
    public class ContentVideoController : ModelController<Guid, ContentVideo, ContentVideoManager>
    {
        public ContentVideoController(ILogger<ContentVideoController> logger)
            : base(logger, ContentVideoManager.GetInstance()) { }
    }
}

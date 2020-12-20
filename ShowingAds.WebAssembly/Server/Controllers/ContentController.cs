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
    public class ContentController : ModelController<Guid, Content, ContentManager>
    {
        public ContentController(ILogger<ContentController> logger)
            : base(logger, ContentManager.GetInstance()) { }
    }
}

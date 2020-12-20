using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
    public class ChannelController : ModelController<Guid, Channel, ChannelManager>
    {
        public ChannelController(ILogger<ChannelController> logger)
            : base(logger, ChannelManager.GetInstance()) { }
    }
}

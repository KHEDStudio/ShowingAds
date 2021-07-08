using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShowingAds.FilesService.Controllers
{
    public class ScreenshotController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

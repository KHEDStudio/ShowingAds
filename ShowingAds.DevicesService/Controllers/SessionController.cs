using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ShowingAds.CoreLibrary.Models.Login;
using ShowingAds.DevicesService.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShowingAds.DevicesService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private ILogger<SessionController> _logger { get; }

        public SessionController(ILogger<SessionController> logger) => _logger = logger;

        [HttpPost]
        public async Task<ActionResult> TryLogin([FromBody] LoginDevice login)
        {
            _logger.LogInformation($"Device is trying login -> {HttpContext.Connection.RemoteIpAddress}:{HttpContext.Connection.RemotePort}");
            var loginer = Loginer.GetInstance();
            var isSuccess = await loginer.TryLogin(login);
            if (isSuccess)
            {
                await Authenticate(login);
                return StatusCode(StatusCodes.Status200OK);
            }
            else return StatusCode(StatusCodes.Status404NotFound);
        }

        private async Task Authenticate(LoginDevice device)
        {
            _logger.LogInformation($"Authenticate device {device.Name} ({device.UUID}) -> {device.Username}");
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, device.UUID.ToString()),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, device.Username)
            };
            var claimsIdentity = new ClaimsIdentity(claims, "ApplicationCookie",
                ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
        }
    }
}

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using ShowingAds.CoreLibrary.Models.Database;
using ShowingAds.CoreLibrary.Models.Login;
using ShowingAds.WebAssembly.Server.BusinessLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShowingAds.WebAssembly.Server.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SessionController : ControllerBase
    {
        private ILogger<SessionController> _logger { get; }

        public SessionController(ILogger<SessionController> logger) => _logger = logger;

        [HttpPost]
        public async Task<ActionResult> TryLogin([FromBody] LoginUser login)
        {
            _logger.LogInformation($"User is trying login -> {HttpContext.Connection.RemoteIpAddress}:{HttpContext.Connection.RemotePort}");
            var loginer = Loginer.GetInstance();
            (var isSuccess, var json) = await loginer.TryLogin(login);
            if (isSuccess)
            {
                var user = JsonConvert.DeserializeObject<User>(json);
                await Authenticate(user);
                return StatusCode(StatusCodes.Status200OK, json);
            }
            else return StatusCode(StatusCodes.Status404NotFound);
        }

        private async Task Authenticate(User user)
        {
            _logger.LogInformation($"Authenticate user {user.Name} role {user.Role}");
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.ToString())
            };
            var claimsIdentity = new ClaimsIdentity(claims, "ApplicationCookie",
                ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
        }
    }
}

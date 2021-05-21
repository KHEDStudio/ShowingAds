using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using ShowingAds.Backend.DeviceService.Managers;
using ShowingAds.Shared.Core.Models.Login;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ShowingAds.Backend.DeviceService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class SessionController : Controller
    {
        [HttpPost]
        public async Task<ActionResult> TryLogin([FromBody] LoginDevice login)
        {
            var loginer = Loginer.GetInstance();
            var isSuccess = await loginer.TryLogin(login);
            if (isSuccess)
            {
                await Authenticate(login);
                return Ok();
            }
            else return NotFound();
        }

        private async Task Authenticate(LoginDevice device)
        {
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

using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ShowingAds.Backend.AuthService.Managers;
using ShowingAds.Shared.Backend;
using ShowingAds.Shared.Backend.Models.Database;
using ShowingAds.Shared.Core.Models.Login;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using ShowingAds.Shared.Core;

namespace ShowingAds.Backend.AuthService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class LoginController : Controller
    {
        [HttpPost]
        public async Task<IActionResult> LoginAsync([FromBody] LoginUser loginUser)
        {
            var manager = LoginManager.GetInstance();
            var user = await manager.GetUserOrNullAsync(loginUser);
            if (user != null)
                return Ok(GetResponseToken(user));
            return BadRequest(new
                {
                    errorText = "Invalid username or password."
                });
        }

        private object GetResponseToken(User user)
        {
            var now = DateTime.UtcNow;
            var claimsIdentity = GetClaimsIdentity(user);
            var signingCredentials = new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(),
                SecurityAlgorithms.HmacSha256);
            var jwt = new JwtSecurityToken(
                issuer: AuthOptions.ISSUER,
                audience: AuthOptions.AUDIENCE,
                notBefore: now,
                claims: claimsIdentity.Claims,
                expires: now.AddMinutes(AuthOptions.LIFETIME),
                signingCredentials: signingCredentials);
            var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);
            return new
                {
                    access_token = encodedJwt
                };
        }

        private ClaimsIdentity GetClaimsIdentity(User user)
        {
            var authenticationType = "Token";
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Name),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.ToString()),
                new Claim(nameof(user.Id), user.Id.ToString()),
                new Claim("GuidId", user.Id.ToGuid().ToString()),
                new Claim(nameof(user.Name), user.Name),
                new Claim(nameof(user.Role), user.Role.ToString()),
                new Claim(nameof(user.MailAddress), user.MailAddress.Address)
            };
            return new ClaimsIdentity(claims, authenticationType,
                ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
        }
    }
}

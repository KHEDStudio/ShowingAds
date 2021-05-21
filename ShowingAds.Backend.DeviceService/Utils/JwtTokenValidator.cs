using Microsoft.IdentityModel.Tokens;
using ShowingAds.Shared.Backend;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Principal;

namespace ShowingAds.Backend.DeviceService.Utils
{
    public class JwtTokenValidator
    {
        public string JwtToken { get; private set; }
        public ClaimsPrincipal User { get; private set; }

        public JwtTokenValidator(string jwtToken)
        {
            JwtToken = jwtToken ?? throw new ArgumentNullException(nameof(jwtToken));
        }

        public bool ValidateToken()
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = GetValidationParameters();

                User = tokenHandler.ValidateToken(JwtToken, validationParameters, out _);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public int GetId()
        {
            var identity = User.Identity as ClaimsIdentity;
            foreach (var claim in identity.Claims)
                if (claim.Type == "Id")
                    return Convert.ToInt32(claim.Value);
            return -1;
        }

        private TokenValidationParameters GetValidationParameters()
        {
            return new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidIssuer = AuthOptions.ISSUER,
                ValidateAudience = true,
                ValidAudience = AuthOptions.AUDIENCE,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
                ValidateLifetime = true
            };
        }
    }
}

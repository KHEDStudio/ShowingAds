using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace ShowingAds.Shared.Backend
{
    public class AuthOptions
    {
        public const string ISSUER = "ShowingAds.Backend.AuthService";
        public const string AUDIENCE = "ShowingAds.Clients";
        private const string KEY = "j++QcQ6W3f82EpYdT5_6tca$m7S-wx@sFPXF!xLeJFpZuggYnaw_+VjcCmeHj4W6Me+HFV_RRY97ynFX@TQ*^EnzFmjH5w@DK38HfCj4ZH?^HCX^+*sXgqQqGewq=q@%7Fb-hCr6z-B3mEHSuamsb7m+JwVNMh^W26FTsH&U8293SFda^U9NKmwM%SR2?2yZmUHkqHQ6ap%A-7s*MU_%X@bB*2BARt!3uN&KVqADv?8MU2YnRTJ-+ePvAfbFud=T";
        public const int LIFETIME = 60 * 24 * 365;

        public static SymmetricSecurityKey GetSymmetricSecurityKey()
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(KEY));
        }
    }
}

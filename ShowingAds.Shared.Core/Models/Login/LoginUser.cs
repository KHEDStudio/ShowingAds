using Newtonsoft.Json;
using System;

namespace ShowingAds.Shared.Core.Models.Login
{
    public class LoginUser
    {
        [JsonProperty("username")]
        public string Username { get; private set; }
        [JsonProperty("password")]
        public string Password { get; private set; }

        [JsonConstructor]
        public LoginUser(string username, string password)
        {
            Username = username ?? throw new ArgumentNullException(nameof(username));
            Password = password ?? throw new ArgumentNullException(nameof(password));
        }
    }
}

using Newtonsoft.Json;
using System;

namespace ShowingAds.CoreLibrary.Models.Login
{
    public class LoginDevice : LoginUser
    {
        [JsonProperty("name")]
        public string Name { get; private set; }
        [JsonProperty("id")]
        public Guid UUID { get; private set; }

        [JsonConstructor]
        public LoginDevice(string name, Guid id, string username, string password) : base(username, password)
        {
            Name = name ?? string.Empty;
            UUID = id;
        }
    }
}

using Newtonsoft.Json;
using ShowingAds.CoreLibrary.Converters;
using ShowingAds.CoreLibrary.Enums;
using ShowingAds.CoreLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;

namespace ShowingAds.CoreLibrary.Models.Database
{
    public class User : ICloneable, IModel<int>
    {
        [JsonProperty("id")]
        public int Id { get; private set; }
        [JsonProperty("red")]
        public byte Red { get; private set; }
        [JsonProperty("green")]
        public byte Green { get; private set; }
        [JsonProperty("blue")]
        public byte Blue { get; private set; }
        [JsonProperty("role")]
        public UserRole Role { get; private set; }
        [JsonProperty("owner")]
        public int OwnerId { get; private set; }
        [JsonProperty("username")]
        public string Name { get; private set; }
        [JsonProperty("email"), JsonConverter(typeof(MailConverter))]
        public MailAddress MailAddress { get; private set; }
        [JsonProperty("last_login")]
        public DateTime LastLogin { get; private set; }

        [JsonConstructor]
        public User(int id, byte red, byte green, byte blue, UserRole role, int owner, string username, string email, string last_login)
        {
            Id = id;
            Red = red;
            Green = green;
            Blue = blue;
            Role = role;
            OwnerId = owner;
            Name = username ?? throw new ArgumentNullException(nameof(username));
            MailAddress = string.IsNullOrEmpty(email) ? null : new MailAddress(email);
            LastLogin = string.IsNullOrEmpty(last_login) ? DateTime.MinValue : DateTime.Parse(last_login);
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public int GetKey() => Id;
    }
}

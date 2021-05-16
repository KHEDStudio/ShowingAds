using Newtonsoft.Json;
using ShowingAds.Shared.Core.Converters;
using ShowingAds.Shared.Core.Models;
using ShowingAds.Shared.Core.Models.Login;
using System;

namespace ShowingAds.Shared.Backend.Models.Database
{
    public class Device : ICloneable, IModel<Guid>
    {
        [JsonProperty("id"), JsonConverter(typeof(GuidConverter))]
        public Guid Id { get; private set; }
        [JsonProperty("name")]
        public string Name { get; private set; }
        [JsonProperty("address")]
        public string Address { get; private set; }
        [JsonProperty("latitude")]
        public double Latitude { get; private set; }
        [JsonProperty("longitude")]
        public double Longitude { get; private set; }
        [JsonProperty("last_online")]
        public DateTime LastOnline { get; set; }

        [JsonProperty("account")]
        public int OwnerId { get; private set; }
        [JsonProperty("channel"), JsonConverter(typeof(GuidConverter))]
        public Guid ChannelId { get; set; }

        [JsonConstructor]
        public Device(Guid id, string name, string address, double latitude, double longitude, DateTime last_online, int account, Guid channel)
        {
            Id = id;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Address = address ?? string.Empty;
            Latitude = latitude;
            Longitude = longitude;
            LastOnline = last_online;
            OwnerId = account;
            ChannelId = channel;
        }

        public Device(Device device)
        {
            Id = device.Id;
            Name = device.Name;
            Address = device.Address;
            Latitude = device.Latitude;
            Longitude = device.Longitude;
            LastOnline = device.LastOnline;
            OwnerId = device.OwnerId;
            ChannelId = device.ChannelId;
        }

        public Device(LoginDevice login, int ownerId)
        {
            Id = login.UUID;
            Name = login.Name;
            Address = string.Empty;
            Latitude = double.MinValue;
            Longitude = double.MinValue;
            LastOnline = DateTime.Now;
            OwnerId = ownerId;
            ChannelId = Guid.Empty;
        }

        public override bool Equals(object obj)
        {
            return obj is Device device &&
                   Id.Equals(device.Id) &&
                   Name == device.Name &&
                   Address == device.Address &&
                   Latitude == device.Latitude &&
                   Longitude == device.Longitude &&
                   LastOnline == device.LastOnline &&
                   OwnerId == device.OwnerId &&
                   ChannelId.Equals(device.ChannelId);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Id, Name, Address, Latitude, Longitude, LastOnline, OwnerId, ChannelId);
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public Guid GetKey() => Id;
    }
}

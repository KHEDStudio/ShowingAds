using Newtonsoft.Json;
using ShowingAds.CoreLibrary.Converters;
using ShowingAds.CoreLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.CoreLibrary.Models.Database
{
    public class AdvertisingClient : ICloneable, IModel<Guid>
    {
        [JsonProperty("id"), JsonConverter(typeof(GuidConverter))]
        public Guid Id { get; private set; }
        [JsonProperty("founder")]
        public string FounderName { get; private set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("account")]
        public int OwnerId { get; private set; }
        [JsonProperty("client")]
        public int? ClientId { get; private set; }

        [JsonConstructor]
        public AdvertisingClient(Guid id, string founder, string name, string description, int account, int? client)
        {
            Id = id;
            FounderName = founder ?? throw new ArgumentNullException(nameof(founder));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? string.Empty;
            OwnerId = account;
            ClientId = client;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public Guid GetKey() => Id;
    }
}

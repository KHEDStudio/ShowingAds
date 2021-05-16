using Newtonsoft.Json;
using ShowingAds.Shared.Core.Converters;
using ShowingAds.Shared.Core.Models;
using System;

namespace ShowingAds.Shared.Backend.Models.Database
{
    public class Content : ICloneable, IModel<Guid>
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

        [JsonConstructor]
        public Content(Guid id, string founder, string name, string description, int account)
        {
            Id = id;
            FounderName = founder ?? throw new ArgumentNullException(nameof(founder));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? string.Empty;
            OwnerId = account;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public Guid GetKey() => Id;
    }
}

using Newtonsoft.Json;
using ShowingAds.Shared.Core.Converters;
using System;

namespace ShowingAds.Shared.Core.Models.Json
{
    public class VideoJson
    {
        [JsonProperty("id"), JsonConverter(typeof(GuidConverter))]
        public Guid Id { get; private set; }

        [JsonConstructor]
        public VideoJson(Guid id)
        {
            Id = id;
        }
    }
}

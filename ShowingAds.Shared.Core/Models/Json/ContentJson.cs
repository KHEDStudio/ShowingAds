using Newtonsoft.Json;
using ShowingAds.Shared.Core.Converters;
using System;
using System.Collections.Generic;

namespace ShowingAds.Shared.Core.Models.Json
{
    public class ContentJson
    {
        [JsonProperty("id"), JsonConverter(typeof(GuidConverter))]
        public Guid Id { get; private set; }
        [JsonProperty("videos")]
        public IEnumerable<VideoJson> Videos { get; private set; }

        [JsonConstructor]
        public ContentJson(Guid id, IEnumerable<VideoJson> videos)
        {
            Id = id;
            Videos = videos ?? new List<VideoJson>();
        }
    }
}

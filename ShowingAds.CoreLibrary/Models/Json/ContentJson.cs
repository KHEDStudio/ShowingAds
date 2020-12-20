using Newtonsoft.Json;
using ShowingAds.CoreLibrary.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.CoreLibrary.Models.Json
{
    public class ContentJson
    {
        [JsonProperty("id"), JsonConverter(typeof(GuidConverter))]
        public Guid Id { get; private set; }
        [JsonProperty("videos")]
        public List<VideoJson> Videos { get; private set; }

        [JsonConstructor]
        public ContentJson(Guid id, List<VideoJson> videos)
        {
            Id = id;
            Videos = videos ?? new List<VideoJson>();
        }
    }
}

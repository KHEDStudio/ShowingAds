using Newtonsoft.Json;
using ShowingAds.Shared.Core.Converters;
using System;
using System.Collections.Generic;

namespace ShowingAds.Shared.Core.Models.Json
{
    public class ClientChannelJson
    {
        [JsonProperty("id"), JsonConverter(typeof(GuidConverter))]
        public Guid Id { get; private set; }
        [JsonProperty("interval"), JsonConverter(typeof(TimeSpanConverter))]
        public TimeSpan Interval { get; private set; }
        [JsonProperty("videos")]
        public IEnumerable<VideoJson> Videos { get; private set; }

        [JsonConstructor]
        public ClientChannelJson(Guid id, TimeSpan interval, IEnumerable<VideoJson> videos)
        {
            Id = id;
            Interval = interval;
            Videos = videos ?? new List<VideoJson>();
        }
    }
}

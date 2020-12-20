using Newtonsoft.Json;
using ShowingAds.CoreLibrary.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.CoreLibrary.Models.Json
{
    public class ClientChannelJson
    {
        [JsonProperty("id"), JsonConverter(typeof(GuidConverter))]
        public Guid Id { get; private set; }
        [JsonProperty("interval"), JsonConverter(typeof(TimeSpanConverter))]
        public TimeSpan Interval { get; private set; }
        [JsonProperty("videos")]
        public List<VideoJson> Videos { get; private set; }

        [JsonConstructor]
        public ClientChannelJson(Guid id, TimeSpan interval, List<VideoJson> videos)
        {
            Id = id;
            Interval = interval;
            Videos = videos ?? new List<VideoJson>();
        }
    }
}

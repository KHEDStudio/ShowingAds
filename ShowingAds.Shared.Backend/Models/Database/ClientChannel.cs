using Newtonsoft.Json;
using ShowingAds.Shared.Core.Converters;
using ShowingAds.Shared.Core.Models;
using System;
using System.Collections.Generic;

namespace ShowingAds.Shared.Backend.Models.Database
{
    public class ClientChannel : ICloneable, IModel<Guid>
    {
        [JsonProperty("id"), JsonConverter(typeof(GuidConverter))]
        public Guid Id { get; private set; }
        [JsonProperty("interval"), JsonConverter(typeof(TimeSpanConverter))]
        public TimeSpan Interval { get; set; }

        [JsonProperty("channel"), JsonConverter(typeof(GuidConverter))]
        public Guid ChannelId { get; private set; }
        [JsonProperty("ads_client"), JsonConverter(typeof(GuidConverter))]
        public Guid AdvertisingClientId { get; private set; }
        [JsonProperty("ads_videos")]
        public IEnumerable<Guid> AdvertisingVideosId { get; private set; }

        [JsonConstructor]
        public ClientChannel(Guid id, TimeSpan interval, Guid channel, Guid ads_client, IEnumerable<Guid> ads_videos)
        {
            Id = id;
            Interval = interval;
            ChannelId = channel;
            AdvertisingClientId = ads_client;
            AdvertisingVideosId = ads_videos ?? new List<Guid>();
        }

        public object Clone()
        {
            var other = (ClientChannel)MemberwiseClone();
            other.AdvertisingVideosId = new List<Guid>(AdvertisingVideosId);
            return other;
        }

        public Guid GetKey() => Id;
    }
}

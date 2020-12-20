using Newtonsoft.Json;
using ShowingAds.CoreLibrary.Converters;
using ShowingAds.CoreLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.CoreLibrary.Models.Database
{
    public class AdvertisingVideo : ICloneable, IModel<Guid>
    {
        [JsonProperty("id"), JsonConverter(typeof(GuidConverter))]
        public Guid Id { get; private set; }
        [JsonProperty("duration"), JsonConverter(typeof(TimeSpanConverter))]
        public TimeSpan Duration { get; private set; }
        [JsonProperty("ads_client"), JsonConverter(typeof(GuidConverter))]
        public Guid AdvertisingClientId { get; private set; }

        [JsonConstructor]
        public AdvertisingVideo(Guid id, TimeSpan duration, Guid ads_client)
        {
            Id = id;
            Duration = duration;
            AdvertisingClientId = ads_client;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public Guid GetKey() => Id;
    }
}

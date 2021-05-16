using Newtonsoft.Json;
using ShowingAds.Shared.Core.Converters;
using ShowingAds.Shared.Core.Models;
using System;

namespace ShowingAds.Shared.Backend.Models.Database
{
    public class ContentVideo : ICloneable, IModel<Guid>
    {
        [JsonProperty("id"), JsonConverter(typeof(GuidConverter))]
        public Guid Id { get; private set; }
        [JsonProperty("duration"), JsonConverter(typeof(TimeSpanConverter))]
        public TimeSpan Duration { get; private set; }
        [JsonProperty("content"), JsonConverter(typeof(GuidConverter))]
        public Guid ContentId { get; private set; }

        [JsonConstructor]
        public ContentVideo(Guid id, TimeSpan duration, Guid content)
        {
            Id = id;
            Duration = duration;
            ContentId = content;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public Guid GetKey() => Id;
    }
}

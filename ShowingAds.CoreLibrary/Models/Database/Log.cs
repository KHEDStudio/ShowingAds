using Newtonsoft.Json;
using ShowingAds.CoreLibrary.Converters;
using ShowingAds.CoreLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.CoreLibrary.Models.Database
{
    public class Log : ICloneable, IModel<Guid>
    {
        [JsonProperty("id"), JsonConverter(typeof(GuidConverter))]
        public Guid Id { get; private set; }
        [JsonProperty("title")]
        public string Title { get; private set; }
        [JsonProperty("description")]
        public string Description { get; private set; }
        [JsonProperty("time")]
        public DateTime DateTime { get; private set; }

        [JsonProperty("device"), JsonConverter(typeof(GuidConverter))]
        public Guid DeviceId { get; private set; }

        [JsonConstructor]
        public Log(Guid id, string title, string description, DateTime time, Guid device)
        {
            Id = id;
            Title = title ?? string.Empty;
            Description = description ?? string.Empty;
            DateTime = time;
            DeviceId = device;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public Guid GetKey() => Id;
    }
}

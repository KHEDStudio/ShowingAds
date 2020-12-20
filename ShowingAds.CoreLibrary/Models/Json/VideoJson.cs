using Newtonsoft.Json;
using ShowingAds.CoreLibrary.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.CoreLibrary.Models.Json
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

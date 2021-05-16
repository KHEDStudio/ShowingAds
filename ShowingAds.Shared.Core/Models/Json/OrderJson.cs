using Newtonsoft.Json;
using ShowingAds.Shared.Core.Converters;
using System;

namespace ShowingAds.Shared.Core.Models.Json
{
    public class OrderJson
    {
        [JsonProperty("id"), JsonConverter(typeof(GuidConverter))]
        public Guid Id { get; private set; }
        [JsonProperty("order_field")]
        public DateTime OrderField { get; private set; }

        [JsonConstructor]
        public OrderJson(Guid id, DateTime order_field)
        {
            Id = id;
            OrderField = order_field;
        }
    }
}

using Newtonsoft.Json;
using ShowingAds.CoreLibrary.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.CoreLibrary.Models.Json
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

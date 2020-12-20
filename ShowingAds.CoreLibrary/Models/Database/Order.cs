using Newtonsoft.Json;
using ShowingAds.CoreLibrary.Converters;
using ShowingAds.CoreLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.CoreLibrary.Models.Database
{
    public class Order : ICloneable, IModel<Guid>
    {
        [JsonProperty("id"), JsonConverter(typeof(GuidConverter))]
        public Guid Id { get; private set; }
        [JsonProperty("order_field")]
        public DateTime OrderField { get; private set; }

        [JsonProperty("channel"), JsonConverter(typeof(GuidConverter))]
        public Guid ChannelId { get; private set; }
        [JsonProperty("ads_client_channel"), JsonConverter(typeof(GuidConverter))]
        public Guid ClientChannelConnectionId { get; private set; }

        [JsonConstructor]
        public Order(Guid id, DateTime order_field, Guid channel, Guid ads_client_channel)
        {
            Id = id;
            OrderField = order_field;
            ChannelId = channel;
            ClientChannelConnectionId = ads_client_channel;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public Guid GetKey() => Id;
    }
}

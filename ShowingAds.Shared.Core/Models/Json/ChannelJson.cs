using Newtonsoft.Json;
using ShowingAds.Shared.Core.Converters;
using ShowingAds.Shared.Core.Enums;
using System;
using System.Collections.Generic;

namespace ShowingAds.Shared.Core.Models.Json
{
    public class ChannelJson : ICloneable, IModel<Guid>
    {
        [JsonProperty("id"), JsonConverter(typeof(GuidConverter))]
        public Guid Id { get; private set; }
        [JsonProperty("logo_left"), JsonConverter(typeof(GuidConverter))]
        public Guid LogoLeft { get; private set; }
        [JsonProperty("logo_right"), JsonConverter(typeof(GuidConverter))]
        public Guid LogoRight { get; private set; }
        [JsonProperty("ticker")]
        public string Ticker { get; private set; }
        [JsonProperty("ticker_interval"), JsonConverter(typeof(TimeSpanConverter))]
        public TimeSpan TickerInterval { get; private set; }
        [JsonProperty("reload_time"), JsonConverter(typeof(TimeSpanConverter))]
        public TimeSpan ReloadTime { get; private set; }
        [JsonProperty("orientation")]
        public DisplayOrientation DisplayOrientation { get; private set; }
        [JsonProperty("contents")]
        public IEnumerable<ContentJson> Contents { get; private set; }
        [JsonProperty("clients")]
        public IEnumerable<ClientChannelJson> Clients { get; private set; }
        [JsonProperty("orders")]
        public IEnumerable<OrderJson> Orders { get; private set; }

        [JsonConstructor]
        public ChannelJson(Guid id, Guid logo_left, Guid logo_right, string ticker, TimeSpan ticker_interval, TimeSpan reload_time, DisplayOrientation orientation, IEnumerable<ContentJson> contents, IEnumerable<ClientChannelJson> clients, IEnumerable<OrderJson> orders)
        {
            Id = id;
            LogoLeft = logo_left;
            LogoRight = logo_right;
            Ticker = ticker ?? string.Empty;
            TickerInterval = ticker_interval;
            ReloadTime = reload_time;
            DisplayOrientation = orientation;
            Contents = contents ?? new List<ContentJson>();
            Clients = clients ?? new List<ClientChannelJson>();
            Orders = orders ?? new List<OrderJson>();
        }

        public object Clone()
        {
            var json = JsonConvert.SerializeObject(this);
            return JsonConvert.DeserializeObject<ChannelJson>(json);
        }

        public Guid GetKey() => Id;
    }
}

using Newtonsoft.Json;
using ShowingAds.CoreLibrary.Converters;
using ShowingAds.CoreLibrary.Enums;
using ShowingAds.CoreLibrary.Models.Database;
using ShowingAds.CoreLibrary.Models.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.CoreLibrary.Models.Json
{
    public class ChannelJson
    {
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
        public List<ContentJson> Contents { get; private set; }
        [JsonProperty("clients")]
        public List<ClientChannelJson> Clients { get; private set; }
        [JsonProperty("orders")]
        public List<OrderJson> Orders { get; private set; }

        [JsonConstructor]
        public ChannelJson(Guid logo_left, Guid logo_right, string ticker, TimeSpan ticker_interval, TimeSpan reload_time, DisplayOrientation orientation, List<ContentJson> contents, List<ClientChannelJson> clients, List<OrderJson> orders)
        {
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

        public ChannelJson(Channel channel, List<ContentJson> contents, List<ClientChannelJson> clients, List<OrderJson> orders)
        {
            LogoLeft = channel.LogoLeft;
            LogoRight = channel.LogoRight;
            Ticker = channel.Ticker ?? string.Empty;
            TickerInterval = channel.TickerInterval;
            ReloadTime = channel.ReloadTime;
            DisplayOrientation = channel.DisplayOrientation;
            Contents = contents ?? new List<ContentJson>();
            Clients = clients ?? new List<ClientChannelJson>();
            Orders = orders ?? new List<OrderJson>();
        }
    }
}

using Newtonsoft.Json;
using ShowingAds.CoreLibrary.Converters;
using ShowingAds.CoreLibrary.Enums;
using ShowingAds.CoreLibrary.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ShowingAds.CoreLibrary.Models.Database
{
    public class Channel : ICloneable, IModel<Guid>
    {
        [JsonProperty("id"), JsonConverter(typeof(GuidConverter))]
        public Guid Id { get; private set; }
        [JsonProperty("founder")]
        public string FounderName { get; private set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }
        [JsonProperty("logo_left"), JsonConverter(typeof(GuidConverter))]
        public Guid LogoLeft { get; set; }
        [JsonProperty("logo_right"), JsonConverter(typeof(GuidConverter))]
        public Guid LogoRight { get; set; }
        [JsonProperty("ticker")]
        public string Ticker { get; set; }
        [JsonProperty("ticker_interval"), JsonConverter(typeof(TimeSpanConverter))]
        public TimeSpan TickerInterval { get; set; }
        [JsonProperty("reload_time"), JsonConverter(typeof(TimeSpanConverter))]
        public TimeSpan ReloadTime { get; set; }
        [JsonProperty("orientation")]
        public DisplayOrientation DisplayOrientation { get; set; }

        [JsonProperty("account")]
        public int OwnerId { get; private set; }
        [JsonProperty("workers")]
        public List<int> WorkersId { get; private set; }
        [JsonProperty("contents")]
        public List<Guid> ContentsId { get; private set; }

        [JsonConstructor]
        public Channel(Guid id, string founder, string name, string description, Guid logo_left, Guid logo_right, string ticker, TimeSpan ticker_interval, TimeSpan reload_time, DisplayOrientation orientation, int account, List<int> workers, List<Guid> contents)
        {
            Id = id;
            FounderName = founder ?? throw new ArgumentNullException(nameof(founder));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? string.Empty;
            LogoLeft = logo_left;
            LogoRight = logo_right;
            Ticker = ticker ?? string.Empty;
            TickerInterval = ticker_interval;
            ReloadTime = reload_time;
            DisplayOrientation = orientation;
            OwnerId = account;
            WorkersId = workers ?? new List<int>();
            ContentsId = contents ?? new List<Guid>();
        }

        public object Clone()
        {
            var other = (Channel)MemberwiseClone();
            other.WorkersId = new List<int>(WorkersId);
            other.ContentsId = new List<Guid>(ContentsId);
            return other;
        }

        public Guid GetKey() => Id;
    }
}

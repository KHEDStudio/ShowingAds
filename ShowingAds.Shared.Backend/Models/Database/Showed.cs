using Newtonsoft.Json;
using ShowingAds.Shared.Core.Converters;
using ShowingAds.Shared.Core.Models;
using System;

namespace ShowingAds.Shared.Backend.Models.Database
{
    public class Showed : ICloneable, IModel<Guid>
    {
        [JsonProperty("id"), JsonConverter(typeof(GuidConverter))]
        public Guid Id { get; private set; }
        [JsonProperty("all_time")]
        public int TotalShowes { get; private set; }
        [JsonProperty("year")]
        public int YearShowes { get; private set; }
        [JsonProperty("update_year")]
        public DateTime UpdateYearShowesTime { get; private set; }
        [JsonProperty("month")]
        public int MonthShowes { get; private set; }
        [JsonProperty("update_month")]
        public DateTime UpdateMonthShowesTime { get; private set; }
        [JsonProperty("week")]
        public int WeekShowes { get; private set; }
        [JsonProperty("update_week")]
        public DateTime UpdateWeekShowesTime { get; private set; }
        [JsonProperty("day")]
        public int DayShowes { get; private set; }
        [JsonProperty("update_day")]
        public DateTime UpdateDayShowesTime { get; private set; }
        [JsonProperty("last_time")]
        public DateTime LastShowedTime { get; private set; }
        [JsonProperty("next_time")]
        public DateTime NextShowedTime { get; private set; }

        [JsonProperty("ads_video"), JsonConverter(typeof(GuidConverter))]
        public Guid AdvertisingVideoId { get; private set; }
        [JsonProperty("device"), JsonConverter(typeof(GuidConverter))]
        public Guid DeviceId { get; private set; }

        [JsonConstructor]
        public Showed(Guid id, int all_time, int year, DateTime update_year, int month, DateTime update_month, int week, DateTime update_week, int day, DateTime update_day, DateTime last_time, DateTime next_time, Guid ads_video, Guid device)
        {
            Id = id;
            TotalShowes = all_time;
            YearShowes = year;
            UpdateYearShowesTime = update_year;
            MonthShowes = month;
            UpdateMonthShowesTime = update_month;
            WeekShowes = week;
            UpdateWeekShowesTime = update_week;
            DayShowes = day;
            UpdateDayShowesTime = update_day;
            LastShowedTime = last_time;
            NextShowedTime = next_time;
            AdvertisingVideoId = ads_video;
            DeviceId = device;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }

        public Guid GetKey() => Id;
    }
}

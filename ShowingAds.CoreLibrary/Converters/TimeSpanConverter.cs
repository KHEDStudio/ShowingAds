using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ShowingAds.CoreLibrary.Converters
{
    public class TimeSpanConverter : JsonConverter<TimeSpan>
    {
        public override TimeSpan ReadJson(JsonReader reader, Type objectType, [AllowNull] TimeSpan existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value is string)
                return TimeSpan.Parse(reader.Value.ToString());
            var ticks = Convert.ToInt64(reader.Value);
            return ticks < 0 ? TimeSpan.Zero : TimeSpan.FromTicks(ticks);
        }

        public override void WriteJson(JsonWriter writer, [AllowNull] TimeSpan value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Ticks);
        }
    }
}

using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace ShowingAds.CoreLibrary.Converters
{
    class GuidConverter : JsonConverter<Guid>
    {
        public override Guid ReadJson(JsonReader reader, Type objectType, [AllowNull] Guid existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value == null || reader.Value.ToString() == string.Empty)
                return Guid.Empty;
            return Guid.Parse(reader.Value.ToString());
        }

        public override void WriteJson(JsonWriter writer, [AllowNull] Guid value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}

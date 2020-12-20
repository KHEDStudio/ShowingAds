using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mail;
using System.Text;

namespace ShowingAds.CoreLibrary.Converters
{
    class MailConverter : JsonConverter<MailAddress>
    {
        public override MailAddress ReadJson(JsonReader reader, Type objectType, [AllowNull] MailAddress existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.Value == null || reader.Value.ToString() == string.Empty)
                return default;
            return new MailAddress(reader.Value.ToString());
        }

        public override void WriteJson(JsonWriter writer, [AllowNull] MailAddress value, JsonSerializer serializer)
        {
            writer.WriteValue(value.Address);
        }
    }
}

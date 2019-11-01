using Newtonsoft.Json;
using System;

namespace OpenFin.Notifications
{
    /**
     * Serailizes DateTime objects into integer JavaScript timestamps.
     * 
     * Based on https://github.com/JamesNK/Newtonsoft.Json/blob/master/Src/Newtonsoft.Json/Utilities/DateTimeUtils.cs
     */
    public class DateTimeConverter : JsonConverter
    {
        internal static readonly long InitialJavaScriptDateTicks = 621355968000000000;

        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Integer)
            {
                long javaScriptTicks = serializer.Deserialize<long>(reader);
                long ticks = (javaScriptTicks * 10000) + InitialJavaScriptDateTicks;

                return new DateTime(ticks);
            }
            else if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }
            else
            {
                throw new Exception("Must be an integer or null");
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is DateTime)
            {
                long ticks = ((DateTime)value).ToUniversalTime().Ticks;
                long javaScriptTicks = (ticks - InitialJavaScriptDateTicks) / 10000;

                serializer.Serialize(writer, javaScriptTicks);
            }
            else
            {
                throw new Exception("Must be a DateTime object");
            }
        }
    }
}

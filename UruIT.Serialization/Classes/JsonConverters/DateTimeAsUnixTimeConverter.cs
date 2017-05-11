using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace UruIT.Serialization.JsonConverters
{
    /// <summary>
    /// Serializes a date into unix time (in milliseconds).
    /// </summary>
    public class DateTimeAsUnixTimeConverter : JsonConverter
    {
        private DateTime UnixEpoch { get { return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc); } }

        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            var date = (DateTime)value;

            long unixTime = (long)(date - UnixEpoch).TotalMilliseconds;

            writer.WriteValue(unixTime);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            JToken jsonToken = JToken.Load(reader);

            if (jsonToken is JValue)
            {
                JValue jsonValue = (JValue)jsonToken;
                if (jsonValue.Type != JTokenType.Integer)
                    throw new ArgumentException("Value isn't an integer", "reader");

                long unixTime = (long)jsonValue.Value;

                return UnixEpoch.AddMilliseconds(unixTime);
            }
            else
            {
                throw new ArgumentException("Invalid JSON", "reader");
            }
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime);
        }
    }
}
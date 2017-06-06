using Monad.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace UruIT.Serialization.JsonConverters
{
    /// <summary>
    /// Converts unit as an empty JSON.
    /// </summary>
    public class UnitConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            //Serializes it into an empty object
            writer.WriteStartObject();
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            JToken jToken = JToken.Load(reader);

            //It must be an empty JSON object
            if (jToken is JObject)
            {
                JObject jObject = (JObject)jToken;
                if (jObject.Count > 0)
                    throw new ArgumentException("The object isn't empty", "reader");

                return Unit.Default;
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
            return objectType == typeof(Unit);
        }
    }
}
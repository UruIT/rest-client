using Monad;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace UruIT.Serialization
{
    /// <summary>
    /// JSON Serializer
    /// </summary>
    public class JsonSerializer : IJsonSerializer
    {
        public SerializerFormat Format { get { return SerializerFormat.Json; } }

        public UruIT.Serialization.JsonSerializerSettings Settings { get; private set; }

        private JsonSerializer(JsonSerializerSettings settings)
        {
            this.Settings = settings;
        }

        public JsonSerializer()
        {
            //Initializes the serializers settings
            Settings = new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Serialize,

                //JSON is always indented to be legible
                Formatting = Formatting.Indented,
                Converters = new List<JsonConverter>(),

                //Missing fields return an error
                MissingMemberHandling = MissingMemberHandling.Error
            };

            //Expected fields can be null
            Settings.ContractResolver.ObjectContract = new RequiredAttributesObjectContract(RequiredLevel.AllowNull);
        }

        public string SerializeObject(object value)
        {
            return JsonConvert.SerializeObject(value, Settings.GetJsonNetSettings());
        }

        public T DeserializeObject<T>(string value)
        {
            return JsonConvert.DeserializeObject<T>(value, Settings.GetJsonNetSettings());
        }

        public OptionStrict<T> TryDeserialize<T>(string value)
        {
            try
            {
                return JsonConvert.DeserializeObject<T>(value, Settings.GetJsonNetSettings());
            }
            catch (JsonException)
            {
                return OptionStrict<T>.Nothing;
            }
        }

        public IJsonSerializer AddConverter(JsonConverter converter)
        {
            Settings.Converters.Add(converter);
            return this;
        }

        public ISerializer<string> Clone()
        {
            return new JsonSerializer(new JsonSerializerSettings(Settings));
        }
    }
}
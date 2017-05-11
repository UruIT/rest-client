using Monad;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Reflection;

namespace UruIT.Serialization.JsonConverters
{
    /// <summary>
    /// Serializes an Option into an object appending a property "Just" if it has a value. It serializes Nothing into null.
    /// </summary>
    public class OptionConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            var objHasValue = (bool)value.GetType().GetProperty("HasValue").GetValue(value, null);
            if (objHasValue)
            {
                //If it has a value then it wraps it in a "Just" object
                var objValue = value.GetType().GetProperty("Value").GetValue(value, null);
                writer.WriteStartObject();
                writer.WritePropertyName("Just");
                serializer.Serialize(writer, objValue);
                writer.WriteEndObject();
            }
            else
            {
                writer.WriteToken(JsonToken.Null);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            JToken jsonToken = JToken.Load(reader);

            //Obtains the inner type of the optional
            Type innerType = objectType.GetGenericArguments()[0];

            if (jsonToken is JValue)
            {
                JValue jsonValue = (JValue)jsonToken;
                if (jsonValue.Type != JTokenType.Null)
                    throw new ArgumentException("Value isn't null", "reader");

                //Generates Nothing dynamically
                Type nothingType = typeof(NothingStrict<>);
                Type resNothingType = nothingType.MakeGenericType(new Type[] { innerType });
                return Activator.CreateInstance(resNothingType);
            }
            else if (jsonToken is JObject)
            {
                JObject jsonObject = (JObject)jsonToken;
                var properties = jsonObject.Properties().ToList();
                if (properties.Count != 1)
                    throw new ArgumentException(string.Format("The JSON object must have 1 property, but it has {0}", properties.Count), "reader");
                var prop = properties[0];
                if (prop.Name != "Just")
                    throw new ArgumentException(string.Format("The JSON property should have name 'Just', but it is '{0}'", prop.Name), "reader");

                //Deserializes the inner value recursively
                var justVal = serializer.Deserialize(prop.Value.CreateReader(), innerType);

                //If the inner type is "int" then JSON.NET deserializes it as "long", so it must be cast
                if (justVal is long && innerType == typeof(int))
                {
                    justVal = Convert.ChangeType(justVal, typeof(int));
                }

                //Generates Just dynamically
                Type justType = typeof(JustStrict<>);
                Type resJustType = justType.MakeGenericType(new Type[] { innerType });
                ConstructorInfo ctor = resJustType.GetConstructor(new Type[] { innerType });
                return ctor.Invoke(new object[] { justVal });
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
            //Should be a class of OptionStrict<T>
            bool isOptionClass = objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(OptionStrict<>);

            //If not, then it must be a direct subclass of OptionStrict<T>
            bool isOptionSubclass = objectType.IsGenericType &&
                objectType.GetGenericTypeDefinition().BaseType != null &&
                objectType.GetGenericTypeDefinition().BaseType.IsGenericType &&
                objectType.GetGenericTypeDefinition().BaseType.GetGenericTypeDefinition() == typeof(OptionStrict<>);

            return isOptionClass || isOptionSubclass;
        }
    }
}
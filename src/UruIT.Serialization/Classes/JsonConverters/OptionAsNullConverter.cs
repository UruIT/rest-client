using Monad;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;

namespace UruIT.Serialization.JsonConverters
{
    /// <summary>
    /// Serializes Option into a nullable value without wrapping it.
    /// </summary>
    public class OptionAsNullConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            var objHasValue = (bool)value.GetType().GetProperty("HasValue").GetValue(value, null);
            if (objHasValue)
            {
                //If it has a value then it serializes it into the same object
                var objValue = value.GetType().GetProperty("Value").GetValue(value, null);
                serializer.Serialize(writer, objValue);
            }
            else
            {
                //If it doesn't have a value, then it serializes it into null
                writer.WriteToken(JsonToken.Null);
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            JToken jsonToken = JToken.Load(reader);

            //Obtains the inner type of the optional
            Type innerType = objectType.GetGenericArguments()[0];

            // If it's a null token then it returns Nothing
            if (jsonToken is JValue && ((JValue)jsonToken).Type == JTokenType.Null)
            {
                //Generates Nothing dynamically
                Type nothingType = typeof(NothingStrict<>);
                Type resNothingType = nothingType.MakeGenericType(new Type[] { innerType });
                return Activator.CreateInstance(resNothingType);
            }
            else
            {
                //Deserializes the inner value recursively
                var justVal = serializer.Deserialize(jsonToken.CreateReader(), innerType);

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
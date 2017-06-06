using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq.Expressions;
using UruIT.Utilities;

namespace UruIT.Serialization.JsonConverters
{
    /// <summary>
    /// Serializes a LambdaExpression.
    /// </summary>
    public class LambdaExpressionConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            //Flattens the expression and serializes the resulting object
            serializer.Serialize(writer, ((LambdaExpression)value).Flatten());
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            JToken jsonToken = JToken.Load(reader);

            if (jsonToken is JValue && ((JValue)jsonToken).Type == JTokenType.Null)
            {
                return null;
            }
            else
            {
                // It deserializes the flattened form, and then unflattens it
                var flattedPredicate = (FlatExpression)serializer.Deserialize(jsonToken.CreateReader(), typeof(FlatExpression));

                return flattedPredicate.Unflatten();
            }
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(LambdaExpression) ||
                (objectType.BaseType != null && CanConvert(objectType.BaseType));
        }
    }
}
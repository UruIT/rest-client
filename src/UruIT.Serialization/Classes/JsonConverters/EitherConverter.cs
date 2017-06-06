using Monad;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Reflection;

namespace UruIT.Serialization.JsonConverters
{
    /// <summary>
    /// Serializes an Either into an object with two possible properties for each state.
    /// </summary>
    public class EitherConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer)
        {
            var objIsRight = (bool)value.GetType().GetProperty("IsRight").GetValue(value, null);

            //The internal object is serialized with either a property "Left" or "Right"
            string jsonProp = objIsRight ? "Right" : "Left";
            object internalObj = value.GetType().GetProperty(jsonProp).GetValue(value, null);

            writer.WriteStartObject();
            writer.WritePropertyName(jsonProp);
            serializer.Serialize(writer, internalObj);
            writer.WriteEndObject();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            //Se obtiene el token base a parsear
            JToken jsonToken = JToken.Load(reader);

            //Se obtienen los tipos dentro del "Either<L,R>" (el L y R)
            Type innerTypeLeft = objectType.GetGenericArguments()[0];
            Type innerTypeRight = objectType.GetGenericArguments()[1];

            if (jsonToken is JObject)
            {
                //Hay que fijarse si es Left o Right
                JObject jsonObject = (JObject)jsonToken;
                var properties = jsonObject.Properties().ToList();
                if (properties.Count != 1)
                    throw new ArgumentException(string.Format("El objecto JSON debe tener 1 propiedad, pero tiene {0}", properties.Count), "reader");
                var prop = properties[0];
                if (prop.Name != "Left" && prop.Name != "Right")
                    throw new ArgumentException(string.Format("La propiedad JSON debería tener el nombre 'Left' o 'Right' pero es {0}", prop.Name), "reader");

                //Se determina cómo deserializar el resto del objeto dependiendo si es "Left" o "Right
                string methodName = prop.Name == "Left" ? "Left" : "Right";
                Type subObjType = prop.Name == "Left" ? innerTypeLeft : innerTypeRight;

                //Se obtiene el valor deserializado recursivamente
                var justVal = serializer.Deserialize(prop.Value.CreateReader(), subObjType);

                //Si el tipo es "int" JSON.NET serializa valores como "long". Debe castearlo a int
                if (justVal is long && innerTypeLeft == typeof(int))
                {
                    justVal = Convert.ChangeType(justVal, typeof(int));
                }

                //Se llama al constructor "public static Either<L, R> Left<L, R>(L left)" o
                //"public static Either<L, R> Left<L, R>(L left)"
                Type eitherStaticClass = Assembly.GetAssembly(typeof(EitherStrict<,>)).GetTypes()
                    .Where(x => x.Namespace == "Monad")
                    .Where(x => x.FullName == "Monad.EitherStrict")
                    .Where(x => x.IsClass && x.GenericTypeArguments.Length == 0)
                    .First();

                return eitherStaticClass
                    .GetMethod(methodName, BindingFlags.Public | BindingFlags.Static)
                    .MakeGenericMethod(new[] { innerTypeLeft, innerTypeRight })
                    .Invoke(null, new[] { justVal });
            }
            else
            {
                //Si no tiene el tipo adecuado es inválido
                throw new ArgumentException("El JSON no es válido", "reader");
            }
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanConvert(Type objectType)
        {
            //Me fijo si se pasó "Either<L,R>"
            return objectType.IsGenericType && objectType.GetGenericTypeDefinition() == typeof(EitherStrict<,>);
        }
    }
}
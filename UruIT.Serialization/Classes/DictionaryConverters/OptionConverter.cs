using Monad;
using System;
using System.Collections.Generic;
using System.Linq;

namespace UruIT.Serialization.DictionaryConverters
{
    /// <summary>
    /// Serializes an option by serializing an empty value with key "Nothing" if it doesn't have any value.
    /// </summary>
    public class OptionConverter : IDictionaryConverter
    {
        public bool CanConvert(Type type)
        {
            return type.IsGenericType && (type.GetGenericTypeDefinition() == typeof(OptionStrict<>) || type.GetGenericTypeDefinition() == typeof(NothingStrict<>) || type.GetGenericTypeDefinition() == typeof(JustStrict<>));
        }

        public string GetSeparator()
        {
            return string.Empty;
        }

        public string GetPath(IDictionaryConverterLocator locator, object container, object value)
        {
            return string.Empty;
        }

        public Dictionary<string, string> Serialize(IDictionaryConverterLocator locator, object value)
        {
            var objHasValue = (bool)value.GetType().GetProperty("HasValue").GetValue(value, null);
            if (objHasValue)
            {
                //If it has a value then it serializes the same object
                var objValue = value.GetType().GetProperty("Value").GetValue(value, null);
                var converter = locator.GetConverter(objValue);
                return converter.Serialize(locator, objValue);
            }
            else
            {
                //If it doesn't have a value then it serializes it as "Nothing"
                return new Dictionary<string, string> { { "Nothing", string.Empty } };
            }
        }

        public object Deserialize(IDictionaryConverterLocator locator, Dictionary<string, string> serialized, Type type)
        {
            //Obtains the inner type of the optional
            var elementType = type.GetGenericArguments().First();

            if (serialized.ContainsKey("Nothing"))
            {
                //Generates Nothing dynamically
                var resNothingType = typeof(NothingStrict<>).MakeGenericType(elementType);
                return Activator.CreateInstance(resNothingType);
            }
            else
            {
                var elementConverter = locator.GetConverter(elementType);
                var justVal = elementConverter.Deserialize(locator, serialized, elementType);

                //Generates Just dynamically
                Type justType = typeof(JustStrict<>);
                Type resJustType = justType.MakeGenericType(elementType);
                System.Reflection.ConstructorInfo ctor = resJustType.GetConstructor(new Type[] { elementType });
                return ctor.Invoke(new object[] { justVal });
            }
        }
    }
}
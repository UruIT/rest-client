using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace UruIT.Serialization.DictionaryConverters
{
    /// <summary>
    /// Serializes a key-value pair.
    /// </summary>
    public class KeyValuePairConverter : DefaultConverter
    {
        public override bool CanConvert(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(KeyValuePair<,>);
        }

        public override object Deserialize(IDictionaryConverterLocator locator, Dictionary<string, string> serialized, Type type)
        {
            return type.GetConstructors()[0].Invoke(
                TypeDescriptor.GetProperties(type)
                .OfType<PropertyDescriptor>()
                .Select(prop => DeserializeProperty(locator, serialized, prop))
                .ToArray());
        }
    }
}
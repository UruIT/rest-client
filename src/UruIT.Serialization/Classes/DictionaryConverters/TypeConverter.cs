using System;
using System.Collections.Generic;

namespace UruIT.Serialization.DictionaryConverters
{
    /// <summary>
    /// Serializes a type into a dictionary by using its Assmebly name
    /// </summary>
    public class TypeConverter : PrimitiveConverter
    {
        public override bool CanConvert(Type type)
        {
            return type == typeof(Type) ||
                (type.BaseType != null && CanConvert(type.BaseType));
        }

        public override Dictionary<string, string> Serialize(IDictionaryConverterLocator locator, object value)
        {
            return new Dictionary<string, string> { { string.Empty, ((Type)value).AssemblyQualifiedName } };
        }

        public override object Deserialize(IDictionaryConverterLocator locator, Dictionary<string, string> serialized, Type type)
        {
            return Type.GetType(serialized[string.Empty]);
        }
    }
}
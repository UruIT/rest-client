using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace UruIT.Serialization.DictionaryConverters
{
    /// <summary>
    /// Serializes a primitive type into a dictionary
    /// </summary>
    public class PrimitiveConverter : IDictionaryConverter
    {
        /// <summary>
        /// Set of basic primitive types that can be serialized
        /// </summary>
        private readonly static HashSet<Type> PrimitiveTypes = new HashSet<Type>
		{
			typeof(string),
			typeof(decimal),
			typeof(DateTime),
		};

        private bool IsPrimitiveType(Type type)
        {
            return type.IsEnum || type.IsPrimitive || PrimitiveTypes.Contains(type);
        }

        public virtual bool CanConvert(Type type)
        {
            return IsPrimitiveType(type);
        }

        public string GetSeparator()
        {
            return string.Empty;
        }

        public string GetPath(IDictionaryConverterLocator locator, object container, object value)
        {
            return string.Empty;
        }

        protected virtual string ToString(object value)
        {
            return Convert.ToString(value, System.Globalization.CultureInfo.InvariantCulture);
        }

        public virtual Dictionary<string, string> Serialize(IDictionaryConverterLocator locator, object value)
        {
            var type = value.GetType();
            var typeConverter = TypeDescriptor.GetConverter(type);
            if (typeConverter.CanConvertTo(typeof(string)))
            {
                return new Dictionary<string, string> { { string.Empty, ToString(value) } };
            }
            else
            {
                throw new ArgumentException(string.Format("It's not possible to convert type '{0}' to 'string'.", type.FullName));
            }
        }

        protected virtual object FromString(Type type, string value)
        {
            var typeConverter = TypeDescriptor.GetConverter(type);
            return typeConverter.ConvertFrom(null, System.Globalization.CultureInfo.InvariantCulture, value);
        }

        public virtual object Deserialize(IDictionaryConverterLocator locator, Dictionary<string, string> serialized, Type type)
        {
            if (serialized.ContainsKey(string.Empty) && serialized[string.Empty] != null)
            {
                return FromString(type, serialized[string.Empty]);
            }
            else
            {
                return type.GetDefaultValue();
            }
        }
    }
}
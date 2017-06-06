using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace UruIT.Serialization.DictionaryConverters
{
    /// <summary>
    /// Default serializer for arbitrary objects with properties.
    /// </summary>
    public class DefaultConverter : IDictionaryConverter
    {
        public const string Separator = ".";
        public const string EmptyObject = "";

        public virtual bool CanConvert(Type type)
        {
            return true;
        }

        public string GetSeparator()
        {
            return Separator;
        }

        public string GetPath(IDictionaryConverterLocator locator, object container, object value)
        {
            var property = TypeDescriptor.GetProperties(container).Cast<PropertyDescriptor>().FirstOrDefault(x => x.GetValue(container) == value);
            if (property != null)
            {
                return NewPrefix(property.Name, string.Empty, locator.GetConverter(value));
            }
            else
            {
                return string.Empty;
            }
        }

        private string NewPrefix(string propName, string key, IDictionaryConverter converter)
        {
            return propName + (string.IsNullOrEmpty(key) ? string.Empty : (converter.GetSeparator() + key));
        }

        public virtual Dictionary<string, string> Serialize(IDictionaryConverterLocator locator, object value)
        {
            var result = new Dictionary<string, string>();
            var properties = TypeDescriptor.GetProperties(value);
            if (properties.Count > 0)
            {
                foreach (PropertyDescriptor property in properties)
                {
                    var propertyValue = property.GetValue(value);
                    var converter = locator.GetConverter(propertyValue);
                    var serialized = converter.Serialize(locator, propertyValue);
                    var separator = locator.GetConverter(propertyValue);
                    result.AddRange(serialized.ToDictionary(x => NewPrefix(property.Name, x.Key, converter), x => x.Value));
                }
            }
            else
            {
                result.Add(string.Empty, value != null ? EmptyObject : null);
            }
            return result;
        }

        public virtual object Deserialize(IDictionaryConverterLocator locator, Dictionary<string, string> serialized, Type type)
        {
            if (serialized.Any() && serialized.All(x => !string.IsNullOrEmpty(x.Key) || x.Value != null))
            {
                var result = Activator.CreateInstance(type);
                foreach (PropertyDescriptor property in TypeDescriptor.GetProperties(result))
                {
                    property.SetValue(result, DeserializeProperty(locator, serialized, property));
                }
                return result;
            }
            else
            {
                return type.GetDefaultValue();
            }
        }

        protected object DeserializeProperty(IDictionaryConverterLocator locator, Dictionary<string, string> serialized, PropertyDescriptor property)
        {
            var converter = locator.GetConverter(property.PropertyType);
            var separator = converter.GetSeparator();
            var propSerialized = serialized.FilterWithKeyPrefix(property.Name)
                .ToDictionary(x => x.Key.StartsWith(separator) ? x.Key.Substring(separator.Length) : x.Key, x => x.Value);
            var propValue = converter.Deserialize(locator, propSerialized, property.PropertyType);
            return propValue;
        }
    }
}
using Monad;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace UruIT.Serialization
{
    /// <summary>
    /// Serializer of values into dictionaries
    /// </summary>
    public class DictionarySerializer : IDictionarySerializer, IDictionaryConverterLocator
    {
        /// <summary>
        /// List of posible converters to use
        /// </summary>
        private readonly List<IDictionaryConverter> converters;

        public DictionarySerializer()
        {
            this.converters = new List<IDictionaryConverter>()
			{
				new DictionaryConverters.DefaultConverter(),
				new DictionaryConverters.EnumerableConverter(),
				new DictionaryConverters.ArrayConverter(),
				new DictionaryConverters.DictionaryConverter(),
				new DictionaryConverters.KeyValuePairConverter(),
				new DictionaryConverters.TypeConverter(),
				new DictionaryConverters.PrimitiveConverter(),
				new DictionaryConverters.BoolConverter(),
			};
        }

        private DictionarySerializer(List<IDictionaryConverter> converters)
        {
            this.converters = converters;
        }

        public SerializerFormat Format
        {
            get { return SerializerFormat.Dictionary; }
        }

        public IDictionarySerializer AddConverter(IDictionaryConverter converter)
        {
            this.converters.Add(converter);

            return this;
        }

        public IDictionaryConverter GetConverter(Type type)
        {
            var converter = converters.LastOrDefault(x => x.CanConvert(type));
            if (converter == null)
            {
                throw new NotImplementedException();
            }
            return converter;
        }

        public Dictionary<string, string> SerializeObject(object value)
        {
            return this.GetConverter(value).Serialize(this, value);
        }

        public OptionStrict<T> TryDeserialize<T>(Dictionary<string, string> value)
        {
            throw new NotImplementedException();
        }

        public T DeserializeObject<T>(Dictionary<string, string> value)
        {
            var type = typeof(T);
            return (T)(this.DeserializeObject(value, type));
        }

        public ISerializer<Dictionary<string, string>> Clone()
        {
            return new DictionarySerializer(new List<IDictionaryConverter>(this.converters));
        }
    }

    public static class DictionarySerializerExtensions
    {
        public static object DeserializeObject(this DictionarySerializer serializer, Dictionary<string, string> value, Type type)
        {
            return serializer.GetConverter(type).Deserialize(serializer, value, type);
        }

        public static Dictionary<string, string> SerializeObject<T, TSelected>(this DictionarySerializer serializer, T value, Expression<Func<T, TSelected>> selector)
        {
            var result = serializer.SerializeObject(selector.Compile().Invoke(value));

            return result;
        }
    }

    public static class IDictionaryConverterLocatorExtensions
    {
        public static IDictionaryConverter GetConverter(this IDictionaryConverterLocator locator, object value)
        {
            var type = value != null ? value.GetType() : typeof(object);
            return locator.GetConverter(type);
        }
    }
}

namespace System.Collections.Generic
{
    public static class DictionaryExtensions
    {
        public static void AddRange<TKey, TValue>(this Dictionary<TKey, TValue> to, Dictionary<TKey, TValue> from)
        {
            foreach (var item in from)
            {
                to.Add(item.Key, item.Value);
            }
        }

        public static Dictionary<string, T> FilterWithKeyPrefix<T>(this Dictionary<string, T> dictionary, string keyPrefix)
        {
            return dictionary
                .Where(x => x.Key.StartsWith(keyPrefix))
                .ToDictionary(
                    x => x.Key.Substring(keyPrefix.Length),
                    x => x.Value);
        }
    }
}

namespace System
{
    public static class TypeExtensions
    {
        public static object GetDefaultValue(this Type type)
        {
            if (type.IsPrimitive)
            {
                return Activator.CreateInstance(type);
            }
            else
            {
                return null;
            }
        }
    }
}
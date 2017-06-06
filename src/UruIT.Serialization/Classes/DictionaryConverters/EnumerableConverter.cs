using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UruIT.Serialization.DictionaryConverters
{
    /// <summary>
    /// Serializes an enumerable type into a dictionary by prefixing each element.
    /// </summary>
    public class EnumerableConverter : IDictionaryConverter
    {
        public virtual bool CanConvert(Type type)
        {
            return type.IsGenericType && typeof(IEnumerable).IsAssignableFrom(type);
        }

        public string GetSeparator()
        {
            return string.Empty;
        }

        public string GetPath(IDictionaryConverterLocator locator, object container, object value)
        {
            var index = 0;
            foreach (var item in (IEnumerable)container)
            {
                if (item == value)
                {
                    return NewPrefix(string.Empty, index, locator.GetConverter(value).GetSeparator());
                }
                index++;
            }
            return string.Empty;
        }

        protected string NewPrefix(string prefix, int index, string separator)
        {
            return string.Format("[{0}]{1}{2}", index, !string.IsNullOrEmpty(prefix) ? separator : string.Empty, prefix);
        }

        protected virtual Type ElementType(Type type)
        {
            return type.GetGenericArguments().FirstOrDefault();
        }

        public Dictionary<string, string> Serialize(IDictionaryConverterLocator locator, object value)
        {
            var result = new Dictionary<string, string>();

            var index = 0;
            foreach (var item in (IEnumerable)value)
            {
                var converter = locator.GetConverter(item);
                var serialized = converter.Serialize(locator, item);
                result.AddRange(serialized.ToDictionary(x => NewPrefix(x.Key, index, converter.GetSeparator()), x => x.Value));
                index++;
            }

            return result;
        }

        public virtual object Deserialize(IDictionaryConverterLocator locator, Dictionary<string, string> serialized, Type type)
        {
            var elementType = ElementType(type);
            var elementConverter = locator.GetConverter(elementType);
            var list = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(elementType));

            if (serialized.Any())
            {
                int deserialized = 0;
                for (int index = 0; true; index++)
                {
                    var indexSerialized = serialized.FilterWithKeyPrefix(NewPrefix(string.Empty, index, string.Empty) + elementConverter.GetSeparator());
                    var listElement = elementConverter.Deserialize(locator, indexSerialized, elementType);
                    list.Add(listElement);

                    deserialized += indexSerialized.Count;
                    if (deserialized == serialized.Count)
                    {
                        break;
                    }
                }
            }
            return TryCastFromList(list, type, elementType);
        }

        protected virtual object TryCastFromList(IList list, Type enumerableType, Type elementType)
        {
            if (!enumerableType.IsAbstract)
            {
                var ctor = enumerableType.GetConstructor(new Type[] { typeof(IEnumerable<>).MakeGenericType(elementType) });
                if (ctor != null)
                {
                    return ctor.Invoke(new object[] { list });
                }
            }
            return list;
        }
    }
}
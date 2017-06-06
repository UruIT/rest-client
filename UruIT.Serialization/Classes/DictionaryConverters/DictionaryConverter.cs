using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace UruIT.Serialization.DictionaryConverters
{
    /// <summary>
    /// Serializes a dictionary.
    /// </summary>
    public class DictionaryConverter : EnumerableConverter
    {
        public override bool CanConvert(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>);
        }

        protected override Type ElementType(Type type)
        {
            var enumerableInterface = type.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IEnumerable<>));
            return enumerableInterface.GetGenericArguments().First();
        }

        protected override object TryCastFromList(IList list, Type enumerableType, Type elementType)
        {
            var dictionary = (IDictionary)Activator.CreateInstance(enumerableType);
            foreach (var item in list)
            {
                dictionary.Add(
                    elementType.GetProperty("Key").GetValue(item, null),
                    elementType.GetProperty("Value").GetValue(item, null));
            }
            return dictionary;
        }
    }
}
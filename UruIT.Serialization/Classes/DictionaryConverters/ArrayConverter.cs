using System;
using System.Collections;

namespace UruIT.Serialization.DictionaryConverters
{
    /// <summary>
    /// Serializes an array into a dictionary.
    /// </summary>
    public class ArrayConverter : EnumerableConverter
    {
        public override bool CanConvert(Type type)
        {
            return type.IsArray;
        }

        protected override Type ElementType(Type type)
        {
            return type.GetElementType();
        }

        protected override object TryCastFromList(IList list, Type enumerableType, Type elementType)
        {
            var array = Array.CreateInstance(elementType, list.Count);
            for (int i = 0; i < list.Count; i++)
            {
                array.SetValue(list[i], i);
            }
            return array;
        }
    }
}
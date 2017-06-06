using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UruIT.Serialization.DictionaryConverters
{
    /// <summary>
    /// Serializes a boolean into a dictionary.
    /// </summary>
    public class BoolConverter : PrimitiveConverter
    {
        public override bool CanConvert(Type type)
        {
            return type == typeof(bool);
        }

        protected override string ToString(object value)
        {
            return base.ToString(value).ToLower();
        }
    }
}
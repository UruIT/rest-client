using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using UruIT.Utilities;

namespace UruIT.Serialization.DictionaryConverters
{
    /// <summary>
    /// Serializes a lambda expression by serializing its flattened form.
    /// </summary>
    public class LambdaExpressionConverter : DefaultConverter
    {
        public override bool CanConvert(Type type)
        {
            return type == typeof(LambdaExpression) ||
                (type.BaseType != null && CanConvert(type.BaseType));
        }

        public override Dictionary<string, string> Serialize(IDictionaryConverterLocator locator, object value)
        {
            return base.Serialize(locator, ((LambdaExpression)value).Flatten());
        }

        public override object Deserialize(IDictionaryConverterLocator locator, Dictionary<string, string> serialized, Type type)
        {
            var flatExpression = (FlatExpression)base.Deserialize(locator, serialized, typeof(FlatExpression));

            return flatExpression.Unflatten();
        }
    }
}
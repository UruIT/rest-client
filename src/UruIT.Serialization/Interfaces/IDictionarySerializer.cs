using System;
using System.Collections.Generic;

namespace UruIT.Serialization
{
    /// <summary>
    /// A serializer into a dictionary.
    /// </summary>
    public interface IDictionarySerializer<TKey, TValue> : ISerializer<Dictionary<TKey, TValue>>
    {
    }

    /// <summary>
    /// A serializer into a dictionary with string keys and values.
    /// </summary>
    public interface IDictionarySerializer : IDictionarySerializer<string, string>
    {
        /// <summary>
        /// Adds a converter for a specific type.
        /// </summary>
        /// <returns>Returns same object for chaining</returns>
        IDictionarySerializer AddConverter(IDictionaryConverter converter);
    }

    /// <summary>
    /// Store or locator for dictionary converters for each different type.
    /// </summary>
    public interface IDictionaryConverterLocator
    {
        /// <summary>
        /// Given a specific type, obtains a dictionary converter for that type.
        /// </summary>
        IDictionaryConverter GetConverter(Type type);
    }

    /// <summary>
    /// Converts values of different types to/from a dictionary
    /// </summary>
    public interface IDictionaryConverter
    {
        /// <summary>
        /// Indicates if a type can be converted into a dictionary.
        /// </summary>
        bool CanConvert(Type type);

        /// <summary>
        /// Returns the separator that separates different levels of a serialized dictionary/object.
        /// </summary>
        string GetSeparator();

        string GetPath(IDictionaryConverterLocator locator, object container, object value);

        /// <summary>
        /// Serializes a value into a dictionary of properties and its values.
        /// </summary>
        /// <param name="locator">Locator of converters needed to convert specific types</param>
        /// <param name="value">Value to serialize</param>
        /// <returns>Serialized dictionary</returns>
        Dictionary<string, string> Serialize(IDictionaryConverterLocator locator, object value);

        /// <summary>
        /// Deserializes a dictionary into an object.
        /// </summary>
        /// <param name="locator">Locator of converters needed to convert specific types</param>
        /// <param name="serialized">Dictionary to serialize</param>
        /// <param name="type">Type of the resulting object</param>
        /// <returns>Deserialized object</returns>
        object Deserialize(IDictionaryConverterLocator locator, Dictionary<string, string> serialized, Type type);
    }
}
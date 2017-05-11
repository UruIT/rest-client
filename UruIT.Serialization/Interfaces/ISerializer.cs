using Monad;

namespace UruIT.Serialization
{
    /// <summary>
    /// Serializer and deserializaer of objects
    /// </summary>
    public interface ISerializer<TSerialize>
    {
        /// <summary>
        /// Format of the serialization.
        /// </summary>
        SerializerFormat Format { get; }

        /// <summary>
        /// Serializes an object using the serialization format.
        /// </summary>
        /// <param name="value">Object to serialize</param>
        /// <returns>Serialized object</returns>
        TSerialize SerializeObject(object value);

        /// <summary>
        /// Deserializes an object using the serialization format.
        /// </summary>
        /// <typeparam name="T">Type of the object to deserialize</typeparam>
        /// <param name="value">Serialized object</param>
        /// <returns>Deserialized object. Returns Nothing if deserialization fails</returns>
        OptionStrict<TObject> TryDeserialize<TObject>(TSerialize value);

        /// <summary>
        /// Deserializes an object using the serialization format.
        /// </summary>
        /// <typeparam name="T">Type of the object to deserialize</typeparam>
        /// <param name="value">Serialized objec</param>
        /// <returns>Deserialized object. Throws exception if deserialization fails</returns>
        TObject DeserializeObject<TObject>(TSerialize value);

        /// <summary>
        /// Creates an exact copy
        /// </summary>
        ISerializer<TSerialize> Clone();
    }

    /// <summary>
    /// Format of the serialized object
    /// </summary>
    public enum SerializerFormat
    {
        Json = 0,
        Xml = 1,
        Dictionary = 2,
    }

    /// <summary>
    /// Default serializer that deserializes into a string
    /// </summary>
    public interface ISerializer : ISerializer<string>
    {
    }
}
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace UruIT.Serialization
{
    /// <summary>
    /// Indicates how to administrate properties
    /// </summary>
    public interface IPropertyContract
    {
        /// <summary>
        /// Takes an already generated JSON property, takes information about the C# property, and returns a new JSON property
        /// </summary>
        JsonProperty CreateProperty(JsonProperty parentProperty, System.Reflection.MemberInfo member, MemberSerialization memberSerialization);

        /// <summary>
        /// Creates an exact copy
        /// </summary>
        IPropertyContract Clone();
    }
}
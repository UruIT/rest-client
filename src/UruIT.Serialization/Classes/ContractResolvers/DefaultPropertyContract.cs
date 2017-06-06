using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace UruIT.Serialization
{
    /// <summary>
    /// Default property creator contract
    /// </summary>
    public class DefaultPropertyContract : IPropertyContract
    {
        public JsonProperty CreateProperty(JsonProperty parentProperty, System.Reflection.MemberInfo member, MemberSerialization memberSerialization)
        {
            return parentProperty;
        }

        public IPropertyContract Clone()
        {
            return new DefaultPropertyContract();
        }
    }
}
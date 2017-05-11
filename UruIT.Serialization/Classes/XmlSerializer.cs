using Monad;
using System;
using System.IO;

namespace UruIT.Serialization
{
    /// <summary>
    /// Serializer to XML
    /// </summary>
    public class XmlSerializer : IXmlSerializer
    {
        public SerializerFormat Format { get { return SerializerFormat.Xml; } }

        private System.Xml.Serialization.XmlSerializer GetSerializer(Type type)
        {
            return new System.Xml.Serialization.XmlSerializer(type);
        }

        public string SerializeObject(object value)
        {
            using (var writer = new StringWriter(System.Globalization.CultureInfo.InvariantCulture))
            {
                GetSerializer(value.GetType()).Serialize(writer, value);
                return writer.ToString();
            }
        }

        public OptionStrict<T> TryDeserialize<T>(string value)
        {
            var xmlSerializer = GetSerializer(typeof(T));
            using (var reader = new System.Xml.XmlTextReader(new StringReader(value)))
            {
                if (xmlSerializer.CanDeserialize(reader))
                {
                    return (T)xmlSerializer.Deserialize(reader);
                }
                else
                {
                    return OptionStrict<T>.Nothing;
                }
            }
        }

        public T DeserializeObject<T>(string value)
        {
            var xmlSerializer = GetSerializer(typeof(T));

            using (var reader = new StringReader(value))
            {
                return (T)xmlSerializer.Deserialize(reader);
            }
        }

        public ISerializer<string> Clone()
        {
            return new XmlSerializer();
        }
    }
}
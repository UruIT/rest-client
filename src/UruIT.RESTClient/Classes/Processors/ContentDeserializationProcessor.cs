using Monad;
using System.Runtime.Serialization;
using UruIT.Serialization;

namespace UruIT.RESTClient.Processors
{
    /// <summary>
    /// Processor that deserializes the body. It tries to deserialize the HTTP response's body into a C# object. If it fails it returns empty.
    /// </summary>
    /// <typeparam name="TResult">Resulting type after deserialization</typeparam>
    /// <typeparam name="TSerializer">Type of the serializer</typeparam>
    public class TryContentDeserializationProcessor<TResult, TSerializer> : ISimpleProcessorNode<OptionStrict<TResult>, TSerializer>
        where TSerializer : ISerializer
    {
        public bool CanProcess(IRestResponse response)
        {
            return true;
        }

        public OptionStrict<TResult> Process(IRestResponse response, TSerializer serializer)
        {
            return serializer.TryDeserialize<TResult>(response.Content);
        }
    }

    /// <summary>
    /// Processor that tries to deserialize the body, and throws an exception if it cant.
    /// </summary>
    /// <typeparam name="TResult">Resulting type after deserialization</typeparam>
    /// <typeparam name="TSerializer">Type of the serializer</typeparam>
    public class ContentDeserializationProcessor<TResult, TSerializer> : ISimpleProcessorNode<TResult, TSerializer>
        where TSerializer : ISerializer
    {
        private readonly IResponseProcessor<OptionStrict<TResult>, TSerializer> innerProcessor;

        public ContentDeserializationProcessor()
        {
            innerProcessor = new TryContentDeserializationProcessor<TResult, TSerializer>();
        }

        public bool CanProcess(IRestResponse response)
        {
            return innerProcessor.CanProcess(response);
        }

        public TResult Process(IRestResponse response, TSerializer serializer)
        {
            var res = innerProcessor.Process(response, serializer);
            if (!res.HasValue)
            {
                var message = string.Format("Error deserializing '{0}' into type {1}.",
                        response.Content, typeof(TResult).FullName);
                throw new SerializationException(message);
            }
            return res.Value;
        }
    }

    /// <summary>
    /// Content deserializer processor with a JSON serializer.
    /// </summary>
    /// <typeparam name="TResult">Resulting type of the deserialization</typeparam>
    public class ContentDeserializationProcessor<TResult> : ContentDeserializationProcessor<TResult, IJsonSerializer>
    {
    }
}
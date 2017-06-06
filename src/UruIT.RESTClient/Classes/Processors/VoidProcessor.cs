using System;
using UruIT.Serialization;

namespace UruIT.RESTClient.Processors
{
    /// <summary>
    /// Processor that doesn't do anything and shouldn't ever be called.
    ///
    /// It's the identity of the alternative processor monoid.
    /// </summary>
    /// <typeparam name="TResult">Type of the result</typeparam>
    /// <typeparam name="TSerializer">Type of the serializer</typeparam>
    public class VoidProcessor<TResult, TSerializer> : ISimpleProcessorNode<TResult, TSerializer>
        where TSerializer : ISerializer
    {
        public bool CanProcess(IRestResponse response)
        {
            return false;
        }

        public TResult Process(IRestResponse response, TSerializer serializer)
        {
            throw new InvalidOperationException("This response shouldn't be processed");
        }
    }

    /// <summary>
    /// Void processor with JSON serializer.
    /// </summary>
    public class VoidProcessor<TResult> : VoidProcessor<TResult, IJsonSerializer>
    {
    }
}
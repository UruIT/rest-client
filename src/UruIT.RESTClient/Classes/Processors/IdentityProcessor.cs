using UruIT.Serialization;

namespace UruIT.RESTClient.Processors
{
    /// <summary>
    /// Identity processor. The recursive composition with any other processor is equivalent to that procesor. Thus it's the identity of the recursive processor monoid.
    /// </summary>
    /// <typeparam name="TResult">Tipo del resultado</typeparam>
    /// <typeparam name="TSerializer">Tipo del serializador</typeparam>
    public class IdentityProcessor<TResult, TSerializer> : RecursiveProcessorNode<TResult, TResult, TSerializer>
        where TSerializer : ISerializer
    {
        protected override bool CanProcessSub(IRestResponse response)
        {
            return ProcessorStructure.CanProcess(response);
        }

        protected override TResult ProcessSub(IRestResponse response, TSerializer serializer)
        {
            return ProcessorStructure.Process(response, serializer);
        }
    }

    /// <summary>
    /// Identity processor with JSON serializer
    /// </summary>
    public class IdentityProcessor<TResult> : IdentityProcessor<TResult, IJsonSerializer>
    {
    }
}
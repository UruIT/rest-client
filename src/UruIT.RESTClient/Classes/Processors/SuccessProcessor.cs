using System.Net;
using UruIT.Serialization;

namespace UruIT.RESTClient.Processors
{
    /// <summary>
    /// Processor that restricts the processing only when the response is successful.
    /// </summary>
    /// <typeparam name="TResult">Type of the result</typeparam>
    /// <typeparam name="TSerializer">Type of the serializer</typeparam>
    public class SuccessProcessor<TResult, TSerializer> : RecursiveProcessorNode<TResult, TResult, TSerializer>
        where TSerializer : ISerializer
    {
        protected override bool CanProcessSub(IRestResponse response)
        {
            //Can only process it if the recursive node can process it and it's succesful.
            return response.StatusCode.IsSuccessful() && ProcessorStructure.CanProcess(response);
        }

        protected override TResult ProcessSub(IRestResponse response, TSerializer serializer)
        {
            return ProcessorStructure.Process(response, serializer);
        }
    }

    /// <summary>
    /// Successful processor with JSON serializer
    /// </summary>
    /// <typeparam name="TResult">Type of the result</typeparam>
    public class SuccessProcessor<TResult> : SuccessProcessor<TResult, IJsonSerializer>
    {
    }
}
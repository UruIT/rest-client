using Monad;
using System.Net;
using UruIT.Serialization;

namespace UruIT.RESTClient.Processors
{
    /// <summary>
    /// Processor that, if the resulting type is Option, then it maps empty values to 404 Not Found.
    /// </summary>
    /// <typeparam name="TResult">Inenr type of the option</typeparam>
    /// <typeparam name="TSerializer">Type of the serializer</typeparam>
    public class OptionAsNotFoundProcessor<TResult, TSerializer> : RecursiveProcessorNode<OptionStrict<TResult>, TResult, TSerializer>
        where TSerializer : ISerializer
    {
        public OptionAsNotFoundProcessor()
            : base()
        {
        }

        public OptionAsNotFoundProcessor(IProcessorStructure<TResult, TSerializer> processorStructure)
            : base(processorStructure)
        {
        }

        protected override bool CanProcessSub(IRestResponse response)
        {
            //It processes the response if it's 404 or the inner processor can
            return response.StatusCode == HttpStatusCode.NotFound || ProcessorStructure.CanProcess(response);
        }

        protected override OptionStrict<TResult> ProcessSub(IRestResponse response, TSerializer serializer)
        {
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return OptionStrict<TResult>.Nothing;
            }
            else
            {
                return ProcessorStructure.Process(response, serializer);
            }
        }
    }

    /// <summary>
    /// Option processor with a JSON serializer
    /// </summary>
    public class OptionAsNotFoundProcessor<TResult> : OptionAsNotFoundProcessor<TResult, IJsonSerializer>
    {
        public OptionAsNotFoundProcessor()
            : base()
        {
        }

        public OptionAsNotFoundProcessor(IProcessorStructure<TResult, IJsonSerializer> processorStructure)
            : base(processorStructure)
        {
        }
    }
}
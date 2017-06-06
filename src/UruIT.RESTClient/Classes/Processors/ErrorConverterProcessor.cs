using Monad;
using UruIT.Serialization;

namespace UruIT.RESTClient.Processors
{
    /// <summary>
    /// Processor that converts possible REST errors into business errors.
    /// </summary>
    /// <typeparam name="TError">Type of the error to return</typeparam>
    /// <typeparam name="TErrorRest">Type of the REST error taken as input</typeparam>
    /// <typeparam name="TSerializer">Type of the serializer</typeparam>
    public class ErrorConverterProcessor<TError, TErrorRest, TSerializer>
        : RecursiveProcessorNode<TError, OptionStrict<TErrorRest>, TSerializer>
        where TSerializer : ISerializer
    {
        private readonly IErrorConverterProvider<TError, TErrorRest> errorConverterProvider;

        public ErrorConverterProcessor(IErrorConverterProvider<TError, TErrorRest> errorConverterProvider)
        {
            this.errorConverterProvider = errorConverterProvider;
        }

        protected override bool CanProcessSub(IRestResponse response)
        {
            return ProcessorStructure.CanProcess(response);
        }

        protected override TError ProcessSub(IRestResponse response, TSerializer serializer)
        {
            var errorRest = ProcessorStructure.Process(response, serializer);
            return errorConverterProvider.ProvideError(errorRest, response);
        }
    }
}
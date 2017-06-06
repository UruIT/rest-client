using Monad;
using UruIT.Serialization;

namespace UruIT.RESTClient.Processors
{
    /// <summary>
    /// Extensions of ErrorProcessor to configure it.
    /// </summary>
    public static class ErrorProcessorExtensions
    {
        /// <summary>
        /// Configures an error processor with the necessary processors.
        /// </summary>
        /// <typeparam name="TErrorRest">Type of the expected error</typeparam>
        /// <typeparam name="TSerializer">Type of the serializer</typeparam>
        /// <param name="errorProcessor">Processor to configure</param>
        /// <returns>Processor configured</returns>
        public static ErrorProcessor<OptionStrict<TErrorRest>, TSerializer> Default<TErrorRest, TSerializer>(
            this ErrorProcessor<OptionStrict<TErrorRest>, TSerializer> errorProcessor)
            where TSerializer : ISerializer
        {
            var tryContentDeserializationProcessor = new TryContentDeserializationProcessor<TErrorRest, TSerializer>();
            errorProcessor.AddProcessors(tryContentDeserializationProcessor);
            return errorProcessor;
        }
    }
}
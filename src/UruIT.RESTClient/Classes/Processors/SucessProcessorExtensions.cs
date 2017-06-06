using UruIT.Serialization;

namespace UruIT.RESTClient.Processors
{
    /// <summary>
    /// Extensions for SuccessProcessor to create default chainings.
    /// </summary>
    public static class SuccessProcessorExtensions
    {
        /// <summary>
        /// Configures the success processor with the necessary configurations to deserialize a successful response into a result.
        /// </summary>
        /// <typeparam name="TResult">Type of the resulting value</typeparam>
        /// <typeparam name="TSerializer">Type of the serializer</typeparam>
        /// <param name="successProcessor">Processor to configure</param>
        /// <returns>Same processor with all the configurations</returns>
        public static SuccessProcessor<TResult, TSerializer> Default<TResult, TSerializer>(
            this SuccessProcessor<TResult, TSerializer> successProcessor)
            where TSerializer : ISerializer
        {
            var contentDeserializationProcessor = new ContentDeserializationProcessor<TResult, TSerializer>();
            successProcessor.AddProcessors(contentDeserializationProcessor);

            return successProcessor;
        }
    }
}
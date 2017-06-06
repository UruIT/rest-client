using Monad;
using System;
using UruIT.RESTClient.Providers;
using UruIT.Serialization;

namespace UruIT.RESTClient.Processors
{
    /// <summary>
    /// Extensions for the exception processor to create default chainings.
    /// </summary>
    public static class ExceptionProcessorExtensions
    {
        /// <summary>
        /// Configures an exception processor with the necessary processors.
        /// </summary>
        /// <typeparam name="TResult">Resulting type in case of success</typeparam>
        /// <typeparam name="TError">Type of the business error</typeparam>
        /// <typeparam name="TRestError">Type of the REST HTTP error</typeparam>
        /// <typeparam name="TException">Type of the exception to throw</typeparam>
        /// <typeparam name="TSerializer">Type of the serializer</typeparam>
        /// <param name="exceptionProcessor">Processor to configure</param>
        /// <param name="errorConverterProvider">Converts the HTTP error to a business error</param>
        /// <returns>Processor configured</returns>
        public static ExceptionProcessor<TResult, TError, TException, TSerializer>
            Default<TResult, TError, TRestError, TException, TSerializer>(
            this ExceptionProcessor<TResult, TError, TException, TSerializer> exceptionProcessor,
            IErrorConverterProvider<TError, TRestError> errorConverterProvider)
            where TSerializer : ISerializer
            where TException : Exception
        {
            var errorConverterProcessor = new ErrorConverterProcessor<TError, TRestError, TSerializer>(errorConverterProvider);
            var errorProcessor = new ErrorProcessor<OptionStrict<TRestError>, TSerializer>().Default();

            exceptionProcessor.AddProcessors(
                errorConverterProcessor.AddProcessors(
                    errorProcessor));

            return exceptionProcessor;
        }

        /// <summary>
        /// Configures an exception processor with the necessary processors.
        /// </summary>
        /// <typeparam name="TResult">Resulting type in case of success</typeparam>
        /// <typeparam name="TRestBusinessError">Type of the business error</typeparam>
        /// <typeparam name="TRestHttpError">Type of the REST HTTP error</typeparam>
        /// <typeparam name="TRestException">Type of the exception to throw</typeparam>
        /// <typeparam name="TSerializer">Type of the serializer</typeparam>
        /// <param name="exceptionProcessor">Processor to configure</param>
        /// <param name="errorConverterProvider">Converts the HTTP error to a business error</param>
        /// <returns>Processor configured</returns>
        public static RestExceptionProcessor<TResult, TRestBusinessError, TRestHttpError, TRestException, TSerializer>
            Default<TResult, TRestBusinessError, TRestHttpError, TRestException, TSerializer>(
            this RestExceptionProcessor<TResult, TRestBusinessError, TRestHttpError, TRestException, TSerializer> exceptionProcessor)
            where TRestBusinessError : RestBusinessError<TRestBusinessError, TRestHttpError>
            where TRestHttpError : RestHttpError<TRestBusinessError, TRestHttpError>
            where TRestException : RestException<TRestBusinessError, TRestHttpError>
            where TSerializer : ISerializer
        {
            var errorConverterProvider = new RestErrorConverterProvider<TRestBusinessError, TRestHttpError>();
            exceptionProcessor.Default(errorConverterProvider);

            return exceptionProcessor;
        }
    }
}
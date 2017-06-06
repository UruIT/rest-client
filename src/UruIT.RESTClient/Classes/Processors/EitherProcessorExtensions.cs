using Monad;
using System;
using UruIT.RESTClient.Providers;
using UruIT.Serialization;

namespace UruIT.RESTClient.Processors
{
    /// <summary>
    /// Extensions of Either processor to configure it with additional processors.
    /// </summary>
    public static class EitherProcessorExtensions
    {
        /// <summary>
        /// Configures an either processor with the necessary processors.
        /// </summary>
        /// <typeparam name="TError">Type of the error result to return as left</typeparam>
        /// <typeparam name="TResult">Type of the right result</typeparam>
        /// <typeparam name="TRestError">Type of the REST error</typeparam>
        /// <typeparam name="TException">Type of the exception</typeparam>
        /// <typeparam name="TSerializer">Type of the serializer</typeparam>
        /// <param name="eitherProcessor">Either processor to configure</param>
        /// <param name="exceptionProvider">Provider that creates an exception given an error</param>
        /// <param name="errorConverterProvider">Converts a REST error into a business error</param>
        /// <returns>Processor configured</returns>
        public static EitherProcessor<TError, TResult, TSerializer> Default<TError, TResult, TRestError, TException, TSerializer>(
            this EitherProcessor<TError, TResult, TSerializer> eitherProcessor,
            IExceptionProvider<TError, TException> exceptionProvider,
            IErrorConverterProvider<TError, TRestError> errorConverterProvider)
            where TSerializer : ISerializer
            where TException : Exception
        {
            var errorConverterProcessor = new ErrorConverterProcessor<TError, TRestError, TSerializer>(errorConverterProvider);
            var errorProcessor = new ErrorProcessor<OptionStrict<TRestError>, TSerializer>().Default();

            eitherProcessor.AddLeftProcessors(
                errorConverterProcessor.AddProcessors(
                    errorProcessor));

            return eitherProcessor;
        }

        /// <summary>
        /// Configures an either REST exception with the necessary processors.
        /// </summary>
        /// <typeparam name="TRestBusinessError">Type of the "Left" case that represents the business error</typeparam>
        /// <typeparam name="TResult">Type of the result that comes in the "Right" case</typeparam>
        /// <typeparam name="TRestHttpError">Type of the REST HTTP error</typeparam>
        /// <typeparam name="TRestException">Type of the exception</typeparam>
        /// <typeparam name="TSerializer">Type of the serializer</typeparam>
        /// <param name="eitherResultadoProcessor">Either processor to configure</param>
        /// <returns>Processor configured</returns>
        public static EitherRestErrorProcessor<TRestBusinessError, TResult, TRestHttpError, TSerializer> Default<TRestBusinessError, TResult, TRestHttpError, TRestException, TSerializer>(
            this EitherRestErrorProcessor<TRestBusinessError, TResult, TRestHttpError, TSerializer> eitherResultadoProcessor)
            where TSerializer : ISerializer
            where TRestBusinessError : RestBusinessError<TRestBusinessError, TRestHttpError>
            where TRestHttpError : RestHttpError<TRestBusinessError, TRestHttpError>
            where TRestException : RestException<TRestBusinessError, TRestHttpError>
        {
            eitherResultadoProcessor.Default<TRestBusinessError, TResult, TRestHttpError, TRestException, TSerializer>(new RestErrorExceptionProvider<TRestBusinessError, TRestHttpError, TRestException>(),
                new RestErrorConverterProvider<TRestBusinessError, TRestHttpError>());

            return eitherResultadoProcessor;
        }

        /// <summary>
        /// Configures an either REST exception with the necessary processors.
        /// </summary>
        /// <typeparam name="TResult">Type of the result that comes in the "Right" case</typeparam>
        /// <typeparam name="TSerializer">Type of the serializer</typeparam>
        /// <param name="eitherResultadoProcessor">Either processor to configure</param>
        /// <returns>Processor configured</returns>
        public static EitherRestErrorProcessor<TResult, TSerializer> Default<TResult, TSerializer>(
            this EitherRestErrorProcessor<TResult, TSerializer> eitherResultadoProcessor)
            where TSerializer : ISerializer
        {
            eitherResultadoProcessor.Default<RestBusinessError, TResult, RestHttpError, RestException, TSerializer>();

            return eitherResultadoProcessor;
        }
    }
}
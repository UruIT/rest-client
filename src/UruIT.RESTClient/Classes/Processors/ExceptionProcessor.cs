using System;
using UruIT.Serialization;

namespace UruIT.RESTClient.Processors
{
    /// <summary>
    /// Processor that throws an exception with the information of the REST response error.
    /// </summary>
    /// <typeparam name="TResult">Type of the result</typeparam>
    /// <typeparam name="TError">Type of the error to thrown in the exception</typeparam>
    /// <typeparam name="TException">Type of the exception to throw</typeparam>
    /// <typeparam name="TSerializer">Type of the serializer</typeparam>
    public class ExceptionProcessor<TResult, TError, TException, TSerializer>
        : RecursiveProcessorNode<TResult, TError, TSerializer>
        where TException : Exception
        where TSerializer : ISerializer
    {
        private readonly IExceptionProvider<TError, TException> exceptionProvider;

        public ExceptionProcessor(IExceptionProvider<TError, TException> exceptionProvider)
        {
            this.exceptionProvider = exceptionProvider;
        }

        protected override bool CanProcessSub(IRestResponse response)
        {
            return ProcessorStructure.CanProcess(response);
        }

        protected override TResult ProcessSub(IRestResponse response, TSerializer serializer)
        {
            var error = ProcessorStructure.Process(response, serializer);
            throw exceptionProvider.ProvideException(error);
        }
    }
}
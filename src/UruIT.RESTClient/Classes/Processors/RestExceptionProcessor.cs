using UruIT.RESTClient.Providers;
using UruIT.Serialization;

namespace UruIT.RESTClient.Processors
{
    /// <summary>
    /// Processor to process REST exceptions.
    /// </summary>
    public class RestExceptionProcessor<TResult, TRestBusinessError, TRestHttpError, TRestException, TSerializer>
        : ExceptionProcessor<TResult, TRestBusinessError, TRestException, TSerializer>
        where TRestBusinessError : RestBusinessError<TRestBusinessError, TRestHttpError>
        where TRestHttpError : RestHttpError<TRestBusinessError, TRestHttpError>
        where TRestException : RestException<TRestBusinessError, TRestHttpError>
        where TSerializer : ISerializer
    {
        public RestExceptionProcessor()
            : base(new RestErrorExceptionProvider<TRestBusinessError, TRestHttpError, TRestException>())
        {
        }
    }

    /// <summary>
    /// REST Exception processor using a JSON Serializer.
    /// </summary>
    public class RestExceptionProcessor<TResult, TRestBusinessError, TRestHttpError, TRestException>
        : RestExceptionProcessor<TResult, TRestBusinessError, TRestHttpError, TRestException, IJsonSerializer>
        where TRestBusinessError : RestBusinessError<TRestBusinessError, TRestHttpError>
        where TRestHttpError : RestHttpError<TRestBusinessError, TRestHttpError>
        where TRestException : RestException<TRestBusinessError, TRestHttpError>
    {
    }

    /// <summary>
    /// REST Exception processor using default types.
    /// </summary>
    public class RestExceptionProcessor<TResult>
        : RestExceptionProcessor<TResult, RestBusinessError, RestHttpError, RestException>
    {
    }
}
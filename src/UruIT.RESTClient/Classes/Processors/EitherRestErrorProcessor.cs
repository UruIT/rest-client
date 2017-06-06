using UruIT.Serialization;

namespace UruIT.RESTClient.Processors
{
    /// <summary>
    /// Either processor that returns REST errors in its left case.
    /// </summary>
    /// <typeparam name="TRestBusinessError">Type of the business error</typeparam>
    /// <typeparam name="TResult">Resulting type</typeparam>
    /// <typeparam name="TRestHttpError">ype of the REST HTTP error</typeparam>
    /// <typeparam name="TSerializer">Type of the serializer</typeparam>
    public class EitherRestErrorProcessor<TRestBusinessError, TResult, TRestHttpError, TSerializer>
        : EitherProcessor<TRestBusinessError, TResult, TSerializer>
        where TRestBusinessError : RestBusinessError<TRestBusinessError, TRestHttpError>
        where TRestHttpError : RestHttpError<TRestBusinessError, TRestHttpError>
        where TSerializer : ISerializer
    {
        public EitherRestErrorProcessor()
        {
        }

        public EitherRestErrorProcessor(IProcessorStructure<TResult, TSerializer> processorStructure)
            : base(processorStructure)
        {
        }
    }

    /// <summary>
    /// Either processor that returns REST errors in its left case.
    /// </summary>
    /// <typeparam name="TResult">Type of the result</typeparam>
    /// <typeparam name="TSerializer">Type of the serializer</typeparam>
    public class EitherRestErrorProcessor<TResult, TSerializer> : EitherRestErrorProcessor<RestBusinessError, TResult, RestHttpError, TSerializer>
        where TSerializer : ISerializer
    {
    }

    /// <summary>
    /// Either procesor that uses a JSON serializer
    /// </summary>
    /// <typeparam name="TResult">Type of the serializer</typeparam>
    public class EitherRestErrorProcessor<TResult> : EitherRestErrorProcessor<TResult, IJsonSerializer>
    {
    }
}
using Monad;

namespace UruIT.RESTClient
{
    /// <summary>
    /// It's in charge of converting a response error in to a business error.
    /// </summary>
    /// <typeparam name="TError">Business error type</typeparam>
    /// <typeparam name="TErrorRest">Response error type</typeparam>
    public interface IErrorConverterProvider<TError, TErrorRest>
    {
        /// <summary>
        /// Converts the response error into a business one. If the response error is empty, then it generates a value by default with the information in the response.
        /// </summary>
        /// <param name="errorRest">Response error, or empty if it couldn't be deserialized</param>
        /// <param name="response">Complete response</param>
        /// <returns>Business error with the necessary information</returns>
        TError ProvideError(OptionStrict<TErrorRest> errorRest, IRestResponse response);
    }
}
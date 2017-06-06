using Monad;
using System;

namespace UruIT.RESTClient.Providers
{
    /// <summary>
    /// Provides a conversion between the HTTP error and the business error.
    /// </summary>
    /// <typeparam name="TRestBusinessError">Type of the business error</typeparam>
    /// <typeparam name="TRestHttpError">Type of the HTTP error</typeparam>
    public class RestErrorConverterProvider<TRestBusinessError, TRestHttpError>
        : IErrorConverterProvider<TRestBusinessError, TRestHttpError>
        where TRestBusinessError : RestBusinessError<TRestBusinessError, TRestHttpError>
        where TRestHttpError : RestHttpError<TRestBusinessError, TRestHttpError>
    {
        /// <summary>
        /// Converts the HTTP error to a business error. If there is no error, then it uses default values from the response.
        /// </summary>
        /// <param name="errorRest">Error from the response</param>
        /// <param name="response">Response from the server</param>
        /// <returns>Corresponding business error</returns>
        public TRestBusinessError ProvideError(OptionStrict<TRestHttpError> errorRest, IRestResponse response)
        {
            var result = errorRest.HasValue ? errorRest.Value : WhenCantDeserializeError(response);
            return result.ToBusinessError();
        }

        /// <summary>
        /// Provides a default error with the information of the response.
        /// </summary>
        /// <param name="response">Server's response</param>
        /// <returns>Error with default values </returns>
        protected virtual TRestHttpError WhenCantDeserializeError(IRestResponse response)
        {
            var error = Activator.CreateInstance<TRestHttpError>();

            error.StatusCode = response.StatusCode;
            error.Message = response.ErrorMessage;
            error.Details = response.Content;

            return error;
        }
    }
}
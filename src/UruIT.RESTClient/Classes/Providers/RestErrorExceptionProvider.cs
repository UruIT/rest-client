using System;

namespace UruIT.RESTClient.Providers
{
    /// <summary>
    /// Creates an exception that contains the information of the error.
    /// </summary>
    /// <typeparam name="TRestBusinessError">Type of the business error</typeparam>
    /// <typeparam name="TRestHTTPError">Type of the HTTP error</typeparam>
    /// <typeparam name="TRestException">Type of the exception</typeparam>
    public class RestErrorExceptionProvider<TRestBusinessError, TRestHTTPError, TRestException>
        : IExceptionProvider<TRestBusinessError, TRestException>
        where TRestBusinessError : RestBusinessError<TRestBusinessError, TRestHTTPError>
        where TRestHTTPError : RestHttpError<TRestBusinessError, TRestHTTPError>
        where TRestException : RestException<TRestBusinessError, TRestHTTPError>
    {
        /// <summary>
        /// Creates an exception containing the HTTP error, generated from the business error.
        /// </summary>
        /// <param name="error">Business error</param>
        /// <returns>Exception with the necessary info</returns>
        public TRestException ProvideException(TRestBusinessError error)
        {
            var ex = Activator.CreateInstance<TRestException>();
            ex.HttpError = error.ToHttpError();
            return ex;
        }
    }
}
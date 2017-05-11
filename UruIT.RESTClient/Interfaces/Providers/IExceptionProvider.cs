using System;

namespace UruIT.RESTClient
{
    /// <summary>
    /// Provides the conversion of a business error into an exception that makes available the information inside the error.
    /// </summary>
    /// <typeparam name="TError">Business error type</typeparam>
    /// <typeparam name="TException">Type of the exception</typeparam>
    public interface IExceptionProvider<TError, TException>
        where TException : Exception
    {
        /// <summary>
        /// Creates an exception that contains the information regarding the error.
        /// </summary>
        TException ProvideException(TError error);
    }
}
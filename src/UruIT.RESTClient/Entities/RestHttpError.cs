using System;
using System.Net;

namespace UruIT.RESTClient
{
    /// <summary>
    /// HTTP error returned in the body of the REST response. Can be extended.
    /// </summary>
    /// <typeparam name="TRestBusinessError">Type of the business error associated with the exception</typeparam>
    /// <typeparam name="TRestHttpError">Type of the http error associated with the exception</typeparam>
    [Serializable]
    public class RestHttpError<TRestBusinessError, TRestHttpError>
        where TRestBusinessError : RestBusinessError<TRestBusinessError, TRestHttpError>
        where TRestHttpError : RestHttpError<TRestBusinessError, TRestHttpError>
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; }
        public string Details { get; set; }

        public virtual TRestBusinessError ToBusinessError()
        {
            var error = Activator.CreateInstance<TRestBusinessError>();
            error.ErrorType = RestErrorTypeExtensions.FromHttpStatusCode(StatusCode);
            error.Message = Message;
            error.Details = Details;
            return error;
        }
    }

    [Serializable]
    public class RestHttpError : RestHttpError<RestBusinessError, RestHttpError>
    {
        /// <summary>
        /// Converts the HTTP error into the corresponding exception.
        /// </summary>
        public virtual RestException ToRestException()
        {
            return new RestException()
            {
                HttpError = this
            };
        }
    }
}
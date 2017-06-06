using Monad;
using System;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace UruIT.RESTClient
{
    /// <summary>
    /// Exception returned from a REST request
    /// </summary>
    /// <typeparam name="TRestBusinessError">Type of the business error associated with the exception</typeparam>
    /// <typeparam name="TRestHttpError">Type of the http error associated with the exception</typeparam>
    [Serializable]
    public class RestException<TRestBusinessError, TRestHttpError> : Exception
        where TRestBusinessError : RestBusinessError<TRestBusinessError, TRestHttpError>
        where TRestHttpError : RestHttpError<TRestBusinessError, TRestHttpError>
    {
        /// <summary>
        /// Contains the data of the request error
        /// </summary>
        public TRestHttpError HttpError { get; set; }

        public RestException(TRestHttpError httpError)
            : base(resultado.Message)
        {
            this.HttpError = httpError;
        }

        public RestException(TRestHttpError httpError, Exception innerException)
            : base(httpError.Message, innerException)
        {
            this.HttpError = httpError;
        }

        public RestException()
            : base()
        {
        }

        public RestException(string message)
            : base(message)
        {
        }

        public RestException(string message, Exception innerException)
            : base(message, innerException)
        {}

        protected RestException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            HttpError = Activator.CreateInstance<TRestHttpError>();
            HttpError.Details = info.GetString("HttpError.Details");
            HttpError.Message = info.GetString("HttpError.Message");
            HttpError.StatusCode = (HttpStatusCode)info.GetInt32("HttpError.StatusCode");
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("HttpError.Details", HttpError.Details);
            info.AddValue("HttpError.Message", HttpError.Message);
            info.AddValue("HttpError.StatusCode", (int)HttpError.StatusCode);
        }

        public override string Message
        {
            get
            {
                return HttpError != null ? HttpError.Message : base.Message;
            }
        }

        public override string ToString()
        {
            return string.Format("{{ HttpError: {{'StatusCode': '{1}', 'Message': '{2}', 'Details': '{3}'}}\n, RestOfDetails: {0}.\n}}", base.ToString(), HttpError.StatusCode, HttpError.Message, HttpError.Details);
        }
    }

    [Serializable]
    public class RestException : RestException<RestBusinessError, RestHttpError>
    {
        public RestException()
            : base()
        {
        }

        public RestException(RestBusinessError error)
            : base(error.ToHttpError())
        {
        }

        public RestException(RestHttpError httpError)
            : base(httpError)
        {
        }

        public RestException(string message)
            : base(message)
        {
        }

        public RestException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected RestException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}

namespace Monad
{
    using UruIT.RESTClient;

    public static class RestExceptionExtensions
    {
        /// <summary>
        /// If the result is left, it interprets it as an error and throws and exception.
        /// </summary>
        public static T ThrowIfError<T>(this EitherStrict<RestBusinessError, T> result)
        {
            if (result.IsLeft)
            {
                result.ToResultadoRest().
                throw new result.

                throw new ResultadoException(result.Left);
            }
            else
            {
                return result.Right;
            }
        }
    }
}
using System;

namespace UruIT.RESTClient
{
    /// <summary>
    /// Business error representing the error from a REST call. Can be extended.
    /// </summary>
    /// <typeparam name="TRestBusinessError">Type of the business error associated with the exception</typeparam>
    /// <typeparam name="TRestHttpError">Type of the http error associated with the exception</typeparam>
    [Serializable]
    public class RestBusinessError<TRestBusinessError, TRestHttpError>
        where TRestBusinessError : RestBusinessError<TRestBusinessError, TRestHttpError>
        where TRestHttpError : RestHttpError<TRestBusinessError, TRestHttpError>
    {
        public RestErrorType ErrorType { get; set; }

        public string Message { get; set; }

        public string Details { get; set; }

        public RestBusinessError()
        {
        }

        public RestBusinessError(RestErrorType errorType)
            : this(errorType, string.Empty, string.Empty)
        {
        }

        public RestBusinessError(RestErrorType errorType, Exception ex)
        {
            ErrorType = errorType;
            if (ex != null)
            {
                Message = ex.Message;
                Details = ex.ToString();
            }
        }

        public RestBusinessError(RestErrorType errorType, string message, string details)
        {
            ErrorType = errorType;
            Message = message;
            Details = details;
        }

        public RestBusinessError(RestErrorType errorType, string message)
            : this(errorType, message, message)
        {
        }

        public virtual TRestHttpError ToHttpError()
        {
            var httpError = Activator.CreateInstance<TRestHttpError>();
            httpError.StatusCode = ErrorType.ToHttpStatusCode();
            httpError.Message = Message;
            httpError.Details = Details;
            return httpError;
        }

        public RestBusinessError ToBusinessError()
        {
            return new RestBusinessError
            {
                ErrorType = ErrorType,
                Message = Message,
                Details = Details,
            };
        }

        public override string ToString()
        {
            return string.Format("{{ 'ErrorType:' {0}, 'Message:'\"{1}\", 'Details:'\"{2}\"",
                ErrorType, Message, Details);
        }
    }

    [Serializable]
    public class RestBusinessError : RestBusinessError<RestBusinessError, RestHttpError>
    {
        public RestBusinessError()
        {
        }

        public RestBusinessError(RestErrorType resultado)
            : base(resultado)
        {
        }

        public RestBusinessError(RestErrorType resultado, Exception ex)
            : base(resultado, ex)
        {
        }

        public RestBusinessError(RestErrorType resultado, string Message)
            : base(resultado, Message)
        {
        }

        public RestBusinessError(RestErrorType resultado, string Message, string Detail)
            : base(resultado, Message, Detail)
        {
        }

        #region Equals

        public override bool Equals(object obj)
        {
            if (!(obj is RestBusinessError)) return false;
            return this.Equals((RestBusinessError)obj);
        }

        public bool Equals(RestBusinessError other)
        {
            if (!other.ErrorType.Equals(this.ErrorType)) return false;
            if (!other.Message.Equals(this.Message)) return false;
            if (!other.Details.Equals(this.Details)) return false;

            return true;
        }

        public static bool operator ==(RestBusinessError a, RestBusinessError b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(RestBusinessError a, RestBusinessError b)
        {
            return !(a == b);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 12391;
                hash = hash * 23081 + ErrorType.GetHashCode();
                hash = hash * 4357 + Message.GetHashCode();
                hash = hash * 6427 + Details.GetHashCode();
                return hash;
            }
        }

        #endregion Equals
    }
}
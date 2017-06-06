using System;

namespace UruIT.RESTClient
{
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
            httpError.Details = Message;
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

		public RestBusinessError(RestErrorType resultado, string mensaje)
			: base(resultado, mensaje)
		{
		}

		public RestBusinessError(RestErrorType resultado, string mensaje, string detalle)
			: base(resultado, mensaje, detalle)
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
            if (!other.Resultado.Equals(this.Resultado)) return false;
            if (!other.Mensaje.Equals(this.Mensaje)) return false;
            if (!other.Detalle.Equals(this.Detalle)) return false;

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
                hash = hash * 23081 + Resultado.GetHashCode();
                hash = hash * 4357 + Mensaje.GetHashCode();
                hash = hash * 6427 + Detalle.GetHashCode();
                return hash;
            }
        }

        #endregion
    }
}
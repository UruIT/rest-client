using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace UruIT.RESTClient
{
	[Serializable]
    public class RestHttpError<TRestBusinessError, TRestHttpError> : TRestHttpError
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

        public virtual RestException<TRestBusinessError, TRestHttpError> ToRestException()
        {
            return new RestException<TRestBusinessError, TRestHttpError>
            {
                HttpError = this     
            };
        }
	}

	[Serializable]
	public class RestHttpError : RestHttpError<RestBusinessError, RestHttpError>
	{
	}
}

using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace UruIT.RESTClient
{
    /// <summary>
    /// Type of the error from the REST call
    /// </summary>
    public enum RestErrorType
    {
        /// <summary>
        /// Represents a validation error of the client data.
        /// </summary>
        ValidationError,

        /// <summary>
        /// Represents an internal error of the service called.
        /// </summary>
        InternalError,
    }

    public static class RestErrorTypeExtensions
    {
        private static readonly Dictionary<RestErrorType, HttpStatusCode> errorStatusCodeMapping =
            new Dictionary<RestErrorType, HttpStatusCode>
			{
				{ RestErrorType.ValidationError, HttpStatusCode.BadRequest },
				{ RestErrorType.InternalError, HttpStatusCode.InternalServerError },
			};

        /// <summary>
        /// Given a REST error, returns the corresponding HTTP code.
        /// </summary>
        public static HttpStatusCode ToHttpStatusCode(this RestErrorType tipoResultado)
        {
            return errorStatusCodeMapping[tipoResultado];
        }

        /// <summary>
        /// Given an HTTP code, returns the corresponding error.
        /// </summary>
        public static RestErrorType FromHttpStatusCode(HttpStatusCode statusCode)
        {
            if (errorStatusCodeMapping.ContainsValue(statusCode))
            {
                return errorStatusCodeMapping.FirstOrDefault(x => x.Value == statusCode).Key;
            }
            else if ((int)statusCode >= 400 && (int)statusCode < 500)
            {
                return RestErrorType.ValidationError;
            }
            else
            {
                return RestErrorType.InternalError;
            }
        }
    }
}
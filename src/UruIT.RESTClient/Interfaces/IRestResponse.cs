using System;
using System.Net;

namespace UruIT.RESTClient
{
    /// <summary>
    /// Result of a REST Call
    /// </summary>
    public interface IRestResponse
    {
        /// <summary>
        /// String representation of the body's content.
        /// </summary>
        string Content { get; set; }

        /// <summary>
        /// Content type of the response.
        /// </summary>
        string ContentType { get; set; }

        /// <summary>
        /// HTTP code of the response.
        /// </summary>
        HttpStatusCode StatusCode { get; set; }

        /// <summary>
        /// Error message if the request fails.
        /// </summary>
        string ErrorMessage { get; set; }

        /// <summary>
        /// Failure exception, if the request fails.
        /// </summary>
        Exception ErrorException { get; set; }
    }
}
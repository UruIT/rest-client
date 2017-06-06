using Monad;
using RestSharp;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UruIT.Serialization;

namespace UruIT.RESTClient
{
    /// <summary>
    /// Request of a REST call.
    /// </summary>
    public interface IRestRequest
    {
        /// <summary>
        /// HTTP method of the request.
        /// </summary>
        Method Method { get; set; }

        /// <summary>
        /// Proxy for the request (if it has any).
        /// </summary>
        OptionStrict<System.Net.IWebProxy> Proxy { get; set; }

        /// <summary>
        /// Path or resource of the request.
        /// </summary>
        string Resource { get; set; }

        /// <summary>
        /// Body of the request. Can be empty.
        /// </summary>
        OptionStrict<Body> Body { get; set; }

        /// <summary>
        /// Necessary certificates to make the request.
        /// </summary>
        IEnumerable<X509Certificate> Certificates { get; set; }

        /// <summary>
        /// Customizable headers.
        /// </summary>
        IEnumerable<HttpHeader> Headers { get; set; }

        /// <summary>
        /// Customizable timeout for this specific request.
        /// </summary>
        OptionStrict<int> Timeout { get; set; }
    }

    /// <summary>
    /// HTTP method.
    /// </summary>
    public enum Method
    {
        GET = 0,
        POST = 1,
        PUT = 2,
        DELETE = 3,
        HEAD = 4,
        OPTIONS = 5,
        PATCH = 6,
        MERGE = 7,
    }

    /// <summary>
    /// Content of a REST request.
    /// </summary>
    public class Body
    {
        /// <summary>
        /// Format of the content.
        /// </summary>
        public SerializerFormat Format { get; set; }

        /// <summary>
        /// Content serialized in the specified format.
        /// </summary>
        public string Content { get; set; }
    }
}
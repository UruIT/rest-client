using System;
using System.Net;

namespace UruIT.RESTClient
{
    /// <summary>
    /// REST response taken from a RestSharp response.
    /// </summary>
    public class RestSharpRestResponse : IRestResponse
    {
        private readonly RestSharp.IRestResponse response;

        public string Content { get { return response.Content; } set { response.Content = value; } }

        public string ContentType { get { return response.ContentType; } set { response.ContentType = value; } }

        public HttpStatusCode StatusCode { get { return response.StatusCode; } set { response.StatusCode = value; } }

        public string ErrorMessage { get { return response.ErrorMessage; } set { response.ErrorMessage = value; } }

        public Exception ErrorException { get { return response.ErrorException; } set { response.ErrorException = value; } }

        public RestSharpRestResponse(RestSharp.IRestResponse response)
        {
            this.response = response;
        }
    }
}
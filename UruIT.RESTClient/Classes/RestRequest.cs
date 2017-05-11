using Monad;
using UruIT.RESTClient.Interfaces;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace UruIT.RESTClient.Clases
{
    /// <summary>
    /// Request REST común
    /// </summary>
    public class RestRequest : IRestRequest
    {
        public Method Method { get; set; }

        public string Resource { get; set; }

        public System.Net.IWebProxy Proxy { get; set; }

        public OptionStrict<Body> Body { get; set; }

        public IEnumerable<X509Certificate> Certificates { get; set; }

        public IEnumerable<RestSharp.HttpHeader> Headers { get; set; }

        public OptionStrict<int> Timeout { get; set; }
    }
}
using Monad;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace UruIT.RESTClient
{
    /// <summary>
    /// Normal REST request.
    /// </summary>
    public class RestRequest : IRestRequest
    {
        public Method Method { get; set; }

        public string Resource { get; set; }

        public OptionStrict<System.Net.IWebProxy> Proxy { get; set; }

        public OptionStrict<Body> Body { get; set; }

        public IEnumerable<X509Certificate> Certificates { get; set; }

        public IEnumerable<RestSharp.HttpHeader> Headers { get; set; }

        public OptionStrict<int> Timeout { get; set; }
    }
}
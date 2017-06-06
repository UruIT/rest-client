using System;
using System.Collections.Generic;
using UruIT.Serialization;

namespace UruIT.RESTClient
{
    /// <summary>
    /// An executer of REST requests that utilizes RestSharp.
    /// </summary>
    public class RestSharpRestClientExecuter : IRestClientExecuter
    {
        public IRestResponse Execute(Uri host, IRestRequest request)
        {
            //Initializes the Restsharp client
            var client = new RestSharp.RestClient(host);
            if (request.Proxy.HasValue)
            {
                client.Proxy = request.Proxy.Value;
            }

            //Executes the request and wraps the Restsharp response
            return new RestSharpRestResponse(client.Execute(ToRestSharpClient(request)));
        }

        /// <summary>
        /// Converts a REST request into a Restsharp request.
        /// </summary>
        protected RestSharp.IRestRequest ToRestSharpClient(IRestRequest request)
        {
            var rsRequest = new RestSharp.RestRequest(request.Resource, (RestSharp.Method)request.Method);

            if (request.Timeout.HasValue)
            {
                rsRequest.Timeout = request.Timeout.Value;
            }

            foreach (var header in request.Headers)
            {
                rsRequest.AddHeader(header.Name, header.Value);
            }

            if (request.Body.HasValue)
            {
                var contentTypes = new Dictionary<SerializerFormat, string>
				{
					{ SerializerFormat.Json, "application/json" },
					{ SerializerFormat.Xml, "text/xml" },
				};

                rsRequest.AddParameter(
                    contentTypes[request.Body.Value.Format],
                    request.Body.Value.Content,
                    RestSharp.ParameterType.RequestBody);
            }
            return rsRequest;
        }
    }
}
using UruIT.Serialization.Core;
using UruIT.RESTClient.Interfaces;
using System;
using System.Collections.Generic;

namespace UruIT.RESTClient.Clases
{
    /// <summary>
    /// Adaptador de RestClient de RestSharp al de RESTClient
    /// </summary>
    public class RestSharpRestClientExecuter : IRestClientExecuter
    {
        public IRestResponse Execute(Uri host, IRestRequest request)
        {
            //Inicializa el cliente
            var client = new RestSharp.RestClient(host);
            if (request.Proxy != null)
            {
                client.Proxy = request.Proxy;
            }
            //Realiza las adaptaciónes desde/hacia RestSharp
            return new RestSharpRestResponse(client.Execute(ToRestSharpClient(request)));
        }



        /// <summary>
        /// Convierte un request de RESTClient a uno de RestSharp
        /// </summary>
        protected RestSharp.IRestRequest ToRestSharpClient(IRestRequest request)
        {
            var rsRequest = new RestSharp.RestRequest(request.Resource, (RestSharp.Method)request.Method);

            if (request.Timeout.HasValue)
            {
                rsRequest.Timeout = request.Timeout.Value;
            }

            if (request.Headers != null)
            {
                foreach (var header in request.Headers)
                {
                    rsRequest.AddHeader(header.Name, header.Value);
                }
            }
            
            //Si existe el body lo agrega
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
using Monad;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UruIT.Serialization;

namespace UruIT.RESTClient
{
    /// <summary>
    /// Builder of the REST client.
    /// </summary>
    /// <typeparam name="TResult">Type of the result</typeparam>
    /// <typeparam name="TSerializer">Type of the serializer</typeparam>
    public class RestClientBuilder<TResult, TSerializer> : IRestClientBuilder<TResult, TSerializer>
        where TSerializer : ISerializer
    {
        /// <summary>
        /// Optional proxy of the request
        /// </summary>
        private OptionStrict<System.Net.IWebProxy> proxy;

        /// <summary>
        /// Host of the request
        /// </summary>
        private readonly string host;

        /// <summary>
        /// Path or resource of the request
        /// </summary>
        private readonly string path;

        /// <summary>
        /// HTTP method of the request
        /// </summary>
        private readonly Method method;

        /// <summary>
        /// Optional data to include in the request's body
        /// </summary>
        private readonly OptionStrict<object> data;

        /// <summary>
        /// Serializer of successful responses
        /// </summary>
        private readonly TSerializer serializer;

        /// <summary>
        /// Serializer of error responses
        /// </summary>
        private readonly TSerializer errorSerializer;

        /// <summary>
        /// Processor structure to use to process the response
        /// </summary>
        private IProcessorStructure<TResult, TSerializer> processor;

        /// <summary>
        /// Request's certificates
        /// </summary>
        private readonly ICollection<X509Certificate> certificateList;

        /// <summary>
        /// Request's headers
        /// </summary>
        private readonly ICollection<RestSharp.HttpHeader> headers;

        /// <summary>
        /// Executer of REST requests
        /// </summary>
        private readonly IRestClientExecuter restClientExecuter;

        /// <summary>
        /// Creator of exception processors
        /// </summary>
        private readonly Func<Processors.ExceptionProcessor<TResult, RestBusinessError, RestException, TSerializer>> exProcesorCreator;

        public RestClientBuilder(TSerializer serializer, TSerializer errorSerializer, IRestClientExecuter restClientExecuter, string host, string path, OptionStrict<object> data, Method method, Func<Processors.ExceptionProcessor<TResult, RestBusinessError, RestException, TSerializer>> exProcesorCreator)
        {
            this.host = host;
            this.path = path;
            this.data = data;
            this.method = method;
            this.serializer = serializer;
            this.errorSerializer = errorSerializer;
            this.processor = new ProcessorStructure<TResult, TSerializer>();
            this.certificateList = new List<X509Certificate>();
            this.headers = new List<RestSharp.HttpHeader>();
            this.restClientExecuter = restClientExecuter;
            this.exProcesorCreator = exProcesorCreator;
            this.proxy = OptionStrict<System.Net.IWebProxy>.Nothing;
        }

        public IRestClientBuilder<TResult, TSerializer> AddProcessors(params IProcessorNode<TResult, TSerializer>[] processors)
        {
            processor = new ProcessorStructure<TResult, TSerializer>(processors);
            return this;
        }

        public IRestClientBuilder<TResult, TSerializer> WithSettings(Action<TSerializer> with)
        {
            with(serializer);
            return this;
        }

        public IRestClientBuilder<TResult, TSerializer> WithErrorSettings(Action<TSerializer> with)
        {
            with(errorSerializer);
            return this;
        }

        public IRestClientBuilder<TResult, TSerializer> AddCertificates(params X509Certificate[] certificates)
        {
            foreach (var certificate in certificates)
            {
                this.certificateList.Add(certificate);
            }

            return this;
        }

        public IRestClientBuilder<TResult, TSerializer> AddHeader(RestSharp.HttpHeader header)
        {
            this.headers.Add(header);
            return this;
        }

        public IRestClientBuilder<TResult, TSerializer> SetProxy(System.Net.IWebProxy proxy)
        {
            this.proxy = new JustStrict<System.Net.IWebProxy>(proxy);
            return this;
        }

        public TResult GetResult()
        {
            return GetResult(OptionStrict<int>.Nothing);
        }

        public TResult GetResult(int timeout)
        {
            return GetResult(new JustStrict<int>(timeout));
        }

        private TResult GetResult(OptionStrict<int> timeout)
        {
            //Obtains the HTTP response
            var response = restClientExecuter.Execute(new Uri(host), GetRequest(timeout));

            //Given said response, obtains the final result
            return GetResultFromResponse(response);
        }

        #region Auxiliaries

        public IRestRequest GetRequest(OptionStrict<int> timeout)
        {
            return new RestRequest()
            {
                Proxy = proxy,
                Method = method,
                Resource = path,
                Certificates = certificateList,
                Headers = headers,
                Body = from dataValue in data
                       select new Body
                       {
                           Format = serializer.Format,
                           Content = serializer.SerializeObject(dataValue)
                       },
                Timeout = timeout
            };
        }

        /// <summary>
        /// Given a RestSharp response it returns the final value.
        /// </summary>
        public TResult GetResultFromResponse(IRestResponse response)
        {
            //It always adds the exception processor to the end of the processor list
            processor.Add(exProcesorCreator());

            //The default processor is always added to the end of the list
            ProcessorUtilities.AddNeutralProcessorAtEndsForStructure<TResult, TSerializer>(processor);

            //It sets the error serializer to all processors
            ProcessorUtilities.SetErrorSerializerForStructure<TResult, TSerializer>(processor, errorSerializer);

            //Applies the post-processing to the response
            return processor.Process(response, serializer);
        }

        #endregion Auxiliaries
    }
}
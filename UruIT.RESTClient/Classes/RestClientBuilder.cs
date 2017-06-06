using Monad;
using UruIT.Serialization.Core;
using Movistar.Online.Common.Types.Resultados;
using Movistar.Online.Common.Types.ResultadosRest;
using UruIT.RESTClient.Clases.Procesadores;
using UruIT.RESTClient.Interfaces;
using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;

namespace UruIT.RESTClient.Clases
{
    /// <summary>
    /// Builder del cliente de pedidos REST
    /// </summary>
    /// <typeparam name="TResult">Tipo del resultado</typeparam>
    public class RestClientBuilder<TResult, TSerializer> : IRestClientBuilder<TResult, TSerializer>
        where TSerializer : ISerializer
    {
        //Proxy del pedido
        private System.Net.IWebProxy proxy;

        //Host del pedido HTTP
        private readonly string host;

        //Path relativo del pedido HTTP
        private readonly string path;

        //Método del pedido HTTP
        private readonly Method method;

        //Datos que van en el body (o nada si no hay tal dato)
        private readonly OptionStrict<object> data;

        //Serializador
        private readonly TSerializer serializer;

        //Serializador de errores
        private readonly TSerializer errorSerializer;

        //Estructura de procesadores a utilizar sobre la respuesta
        private IProcessorStructure<TResult, TSerializer> processor;

        //Certificados
        private readonly ICollection<X509Certificate> certificateList;

        //Headers
        private readonly ICollection<RestSharp.HttpHeader> headers;

        //Cliente REST que realiza la llamada misma
        private readonly IRestClientExecuter restClientExecuter;

        //Encargado de obtener el procesador de excepciones
        private readonly Func<ExceptionProcessor<TResult, RestBusinessError, RestException, TSerializer>> exProcesorCreator;

        public RestClientBuilder(TSerializer serializer, TSerializer errorSerializer, IRestClientExecuter restClientExecuter, string host, string path, OptionStrict<object> data, Method method, Func<ExceptionProcessor<TResult, RestBusinessError, RestException, TSerializer>> exProcesorCreator)
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
        }

        public IRestClientBuilder<TResult, TSerializer> AddProcessors(params IProcessorNode<TResult, TSerializer>[] processors)
        {
            //Se crea una estructura de procesadores con la lista utilizada
            processor = new ProcessorStructure<TResult, TSerializer>(processors);

            return this;
        }

        public IRestClientBuilder<TResult, TSerializer> WithSettings(Action<TSerializer> with)
        {
            //Aplico la acción a los settings del converter
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
            this.proxy = proxy;
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
            //Obtiene la respuesta REST
            var response = restClientExecuter.Execute(new Uri(host), GetRequest(timeout));
            return GetResultFromResponse(response);
        }

        #region Auxiliares

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
        /// Dada una respuesta de RestSharp retorna el valor final
        /// </summary>
        public TResult GetResultFromResponse(IRestResponse response)
        {
            //Se agrega SIEMPRE el procesador de excepciones al final (aunque debe estar antes del de por defecto)
            processor.Add(exProcesorCreator());

            //Se agrega el procesador por defecto al final de las listas de procesamiento
            ProcessorUtilities.AddNeutralProcessorAtEndsForStructure<TResult, TSerializer>(processor);

            //Se setea el serializador de errores a los procesadores de errores
            ProcessorUtilities.SetErrorSerializerForStructure<TResult, TSerializer>(processor, errorSerializer);

            //Aplica el post-procesamiento de tal respuesta
            return processor.Process(response, serializer);
        }

        #endregion Auxiliares
    }
}
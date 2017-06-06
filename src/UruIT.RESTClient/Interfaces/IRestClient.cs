using Monad.Utility;
using System;
using System.Security.Cryptography.X509Certificates;
using UruIT.RESTClient.Processors;
using UruIT.Serialization;

namespace UruIT.RESTClient
{
    /// <summary>
    /// HTTP client that allows to make requests following the REST standard.
    /// </summary>
    /// <typeparam name="TSerializer">Serializer of the body of requests/responses</typeparam>
    public interface IRestClient<TSerializer>
        where TSerializer : ISerializer
    {
        /// <summary>
        /// Makes a GET request.
        /// </summary>
        /// <typeparam name="TResult">Result type of the call</typeparam>
        /// <param name="host">Remote hose (protocol + host + port)</param>
        /// <param name="path">Relative path of the resource</param>
        /// <returns>Resulting value of the request</returns>
        IRestClientBuilder<TResult, TSerializer> Get<TResult>(string host, string path);

        /// <summary>
        /// Makes a POST request.
        /// </summary>
        /// <typeparam name="TResult">Result type of the call</typeparam>
        /// <param name="host">Remote hose (protocol + host + port)</param>
        /// <param name="path">Relative path of the resource</param>
        /// <param name="data">Data to send in the body of the request</param>
        /// <returns>Resulting value of the request</returns>
        IRestClientBuilder<TResult, TSerializer> Post<TResult>(string host, string path, object data);

        /// <summary>
        /// Realiza un PUT
        /// </summary>
        /// <typeparam name="TResult">Result type of the call</typeparam>
        /// <param name="host">Remote hose (protocol + host + port)</param>
        /// <param name="path">Relative path of the resource</param>
        /// <param name="data">Data to send in the body of the request</param>
        /// <returns>Resulting value of the request</returns>
        IRestClientBuilder<TResult, TSerializer> Put<TResult>(string host, string path, object data);

        /// <summary>
        /// Realiza un DELETE
        /// </summary>
        /// <typeparam name="TResult">Result type of the call</typeparam>
        /// <param name="host">Remote hose (protocol + host + port)</param>
        /// <param name="path">Relative path of the resource</param>
        /// <param name="data">Data to send in the body of the request</param>
        /// <returns>Resulting value of the request</returns>
        IRestClientBuilder<TResult, TSerializer> Delete<TResult>(string host, string path, object data);
    }

    /// <summary>
    /// Builder responsible of constructing the request.
    /// </summary>
    /// <typeparam name="TResult">Type of the response</typeparam>
    /// <typeparam name="TSerializer">Serializer of the body of requests/responses</typeparam>
    public interface IRestClientBuilder<TResult, TSerializer>
        where TSerializer : ISerializer
    {
        /// <summary>
        /// Allows the construction of the structure of processors, giving it a list of processing nodes.
        /// Simple processors can be instantiated normally, but to construct a recursive processor you need to instantiate a IResponseProcessor and then call "AddProcsesors" or "ToNode".
        /// </summary>
        /// <param name="processors">List of alternative processors</param>
        /// <returns>Builder to allow chaining</returns>
        IRestClientBuilder<TResult, TSerializer> AddProcessors(params IProcessorNode<TResult, TSerializer>[] processors);

        /// <summary>
        /// Allows the modification of the settings of the serializer.
        /// </summary>
        /// <param name="with">Action that modifies the settings</param>
        /// <returns>Builder to allow chaining</returns>
        IRestClientBuilder<TResult, TSerializer> WithSettings(Action<TSerializer> with);

        /// <summary>
        /// Allows the modification of the settings of the serializer of errors.
        /// </summary>
        /// <param name="with">Action that modifies the settings</param>
        /// <returns>Builder to allow chaining</returns>
        IRestClientBuilder<TResult, TSerializer> WithErrorSettings(Action<TSerializer> with);

        /// <summary>
        /// Allows adding a list of certificates.
        /// </summary>
        /// <param name="certificates">List of certificates of the request</param>
        /// <returns>Builder to allow chaining</returns>
        IRestClientBuilder<TResult, TSerializer> AddCertificates(params X509Certificate[] certificates);

        /// <summary>
        /// Allows adding a header.
        /// </summary>
        /// <param name="header">Header to add to the request</param>
        /// <returns>Builder to allow chaining</returns>
        IRestClientBuilder<TResult, TSerializer> AddHeader(RestSharp.HttpHeader header);

        /// <summary>
        /// Allows setting the proxy for the request.
        /// </summary>
        /// <param name="proxy">HTTP Proxy to set</param>
        /// <returns>Builder to allow chaining</returns>
        IRestClientBuilder<TResult, TSerializer> SetProxy(System.Net.IWebProxy proxy);

        /// <summary>
        /// Obtains the final result of the REST request.
        /// </summary>
        /// <returns>Resulting value</returns>
        TResult GetResult();

        /// <summary>
        /// Obtains the final result of the REST request, using a specific timeout.
        /// </summary>
        /// <param name="timeout">Timeout for this request in milliseconds</param>
        /// <returns>Resulting value</returns>
        TResult GetResult(int timeout);
    }

    /// <summary>
    /// Extensions for compound operations on a REST Client.
    /// </summary>
    public static class IRestClientExtensions
    {
        /// <summary>
        /// Makes a POST request synchronously, returning unit.
        /// </summary>
        /// <param name="client">REST Client</param>
        /// <param name="host">Remote hose (protocol + host + port)</param>
        /// <param name="path">Relative path of the resource</param>
        /// <param name="data">Data to send in the body of the request</param>
        public static Unit PostSyncUnit<TSerializer>(this IRestClient<TSerializer> client, string host, string path, object data)
            where TSerializer : ISerializer
        {
            return client.Post<Unit>(host, path, data)
                .AddProcessors(new UnitAsSuccessProcessor<TSerializer>())
                .GetResult();
        }

        /// <summary>
        /// Makes a PUT request synchronously, returning unit.
        /// </summary>
        /// <param name="client">Cliente REST</param>
        /// <param name="host">Remote hose (protocol + host + port)</param>
        /// <param name="path">Relative path of the resource</param>
        /// <param name="data">Data to send in the body of the request</param>
        public static Unit PutSyncUnit<TSerializer>(this IRestClient<TSerializer> client, string host, string path, object data)
            where TSerializer : ISerializer
        {
            return client.Put<Unit>(host, path, data)
                .AddProcessors(new UnitAsSuccessProcessor<TSerializer>())
                .GetResult();
        }

        /// <summary>
        /// Makes a DELETE request synchronously, returning unit.
        /// </summary>
        /// <param name="client">Cliente REST</param>
        /// <param name="host">Remote hose (protocol + host + port)</param>
        /// <param name="path">Relative path of the resource</param>
        /// <param name="data">Data to send in the body of the request</param>
        public static Unit DeleteSyncUnit<TSerializer>(this IRestClient<TSerializer> client, string host, string path, object data)
            where TSerializer : ISerializer
        {
            return client.Delete<Unit>(host, path, data)
                .AddProcessors(new UnitAsSuccessProcessor<TSerializer>())
                .GetResult();
        }
    }
}
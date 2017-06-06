using Monad;
using UruIT.Serialization;

namespace UruIT.RESTClient
{
    /// <summary>
    /// Base class for REST clients
    /// </summary>
    /// <typeparam name="TSerializer">Type of the serializer</typeparam>
    public abstract class RestClient<TSerializer> : BaseRestClient<TSerializer>, IRestClient<TSerializer>
        where TSerializer : ISerializer
    {
        private readonly IRestClientExecuter restClientExecuter;

        protected RestClient(IRestClientExecuter restClientExecuter)
        {
            this.restClientExecuter = restClientExecuter;
        }

        private IRestClientBuilder<TResult, TSerializer> CreateBuilder<TResult>(string host, string path, OptionStrict<object> data, Method method)
        {
            return new RestClientBuilder<TResult, TSerializer>(
                CreateSuccessSerializer(), CreateErrorSerializer(),
                restClientExecuter, host, path, data, method,
                () => ObtainExceptionProcessor<TResult>());
        }

        public IRestClientBuilder<TResult, TSerializer> Get<TResult>(string host, string path)
        {
            return CreateBuilder<TResult>(host, path, OptionStrict<object>.Nothing, Method.GET);
        }

        public IRestClientBuilder<TResult, TSerializer> Post<TResult>(string host, string path, object data)
        {
            return CreateBuilder<TResult>(host, path, data, Method.POST);
        }

        public IRestClientBuilder<TResult, TSerializer> Put<TResult>(string host, string path, object data)
        {
            return CreateBuilder<TResult>(host, path, data, Method.PUT);
        }

        public IRestClientBuilder<TResult, TSerializer> Delete<TResult>(string host, string path, object data)
        {
            return CreateBuilder<TResult>(host, path, data, Method.DELETE);
        }
    }
}
using Monad;
using UruIT.Serialization.Core;
using UruIT.RESTClient.Interfaces;

namespace UruIT.RESTClient.Clases
{
	public abstract class RestClient<TSerializer> : BaseRestClient<TSerializer>, IRestClient<TSerializer>
		where TSerializer : ISerializer
	{
		//Cliente REST que realiza el llamado REST mismo
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
				() => ObtenerProcesadorExcepciones<TResult>());
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
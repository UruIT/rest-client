using Moq;
using UruIT.Serialization.Core;
using UruIT.RESTClient.Interfaces;
using System;
using System.Linq.Expressions;

namespace UruIT.RESTClient.Mocks
{
	/// <summary>
	/// Mock de IRestClient
	/// </summary>
	public class IRestClientMock<TIRestClient, TSerializer> : Mock<TIRestClient>
		where TIRestClient : class, IRestClient<TSerializer>
		where TSerializer : ISerializer
	{
		//El mock del builder. NOTA: Se asume que se hace 1 solo pedido REST.
		private object builderMock;

		/// <summary>
		/// Obtiene el mock del builder, que retorna el tipo deseado
		/// </summary>
		public IRestClientBuilderMock<TResult, TIRestClient, TSerializer> ObtenerBuilderMock<TResult>()
		{
			return (IRestClientBuilderMock<TResult, TIRestClient, TSerializer>)builderMock;
		}

		/// <summary>
		/// Realiza un mock de "Get"
		/// </summary>
		public IRestClientBuilderMock<TResult, TIRestClient, TSerializer> GetMock<TResult>()
		{
			return SetupMock(m => m.Get<TResult>(It.IsAny<string>(), It.IsAny<string>()));
		}

		/// <summary>
		/// Realiza un mock de "Get" con un path específico
		/// </summary>
		public IRestClientBuilderMock<TResult, TIRestClient, TSerializer> GetMock<TResult>(string path)
		{
			return SetupMock(m => m.Get<TResult>(It.IsAny<string>(), path));
		}

		/// <summary>
		/// Realiza un mock de "Post"
		/// </summary>
		public IRestClientBuilderMock<TResult, TIRestClient, TSerializer> PostMock<TResult>()
		{
			return SetupMock(m => m.Post<TResult>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()));
		}

		/// <summary>
		/// Realiza un mock de "Post" con un path específico
		/// </summary>
		public IRestClientBuilderMock<TResult, TIRestClient, TSerializer> PostMock<TResult>(string path)
		{
			return SetupMock(m => m.Post<TResult>(It.IsAny<string>(), path, It.IsAny<object>()));
		}

		/// <summary>
		/// Realiza un mock de "Put"
		/// </summary>
		public IRestClientBuilderMock<TResult, TIRestClient, TSerializer> PutMock<TResult>()
		{
			return SetupMock(m => m.Put<TResult>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()));
		}

		/// <summary>
		/// Realiza un mock de "Put" con un path específico
		/// </summary>
		public IRestClientBuilderMock<TResult, TIRestClient, TSerializer> PutMock<TResult>(string path)
		{
			return SetupMock(m => m.Put<TResult>(It.IsAny<string>(), path, It.IsAny<object>()));
		}

		/// <summary>
		/// Realiza un mock de "Delete"
		/// </summary>
		public IRestClientBuilderMock<TResult, TIRestClient, TSerializer> DeleteMock<TResult>()
		{
			return SetupMock(m => m.Delete<TResult>(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<object>()));
		}

		/// <summary>
		/// Realiza un mock de "Delete" con un path específico
		/// </summary>
		public IRestClientBuilderMock<TResult, TIRestClient, TSerializer> DeleteMock<TResult>(string path)
		{
			return SetupMock(m => m.Delete<TResult>(It.IsAny<string>(), path, It.IsAny<object>()));
		}

		/// <summary>
		/// Generaliza el setup de mocks de cada método
		/// </summary>
		private IRestClientBuilderMock<TResult, TIRestClient, TSerializer> SetupMock<TResult>(Expression<Func<TIRestClient, IRestClientBuilder<TResult, TSerializer>>> exp)
		{
			//Crea un builder del mock del cliente y lo retorna
			var mock = new IRestClientBuilderMock<TResult, TIRestClient, TSerializer>(this, exp);
			mock.Initialize();
			builderMock = mock;
			return mock;
		}
	}

	/// <summary>
	/// Mock de IRestClientBuilder
	/// </summary>
	/// <typeparam name="TResult">Tipo del resultado</typeparam>
	public class IRestClientBuilderMock<TResult, TIRestClient, TSerializer> : Mock<IRestClientBuilder<TResult, TSerializer>>
		where TIRestClient : class, IRestClient<TSerializer>
		where TSerializer : ISerializer
	{
		//Expresión que va a ser mockeada de IRestClient
		private readonly Expression<Func<TIRestClient, IRestClientBuilder<TResult, TSerializer>>> clientExp;

		//Mock del RESTClient
		private readonly IRestClientMock<TIRestClient, TSerializer> client;

		public IRestClientBuilderMock(IRestClientMock<TIRestClient, TSerializer> client, Expression<Func<TIRestClient, IRestClientBuilder<TResult, TSerializer>>> clientExp)
		{
			this.client = client;
			this.clientExp = clientExp;
		}

		/// <summary>
		/// Realiza la configuración inicial del mock
		/// </summary>
		internal void Initialize()
		{
			//Permite utilizar otras funciones con el mock
			this.Setup(m => m.WithSettings(It.IsAny<Action<TSerializer>>()))
				.Returns(this.Object);
			this.Setup(m => m.AddProcessors(It.IsAny<IProcessorNode<TResult, TSerializer>[]>()))
				.Returns(this.Object);
		}

		/// <summary>
		/// Realiza un mock de "GetResult"
		/// </summary>
		public IRestClientMock<TIRestClient, TSerializer> GetResultMock(Func<TResult> callback)
		{
			//Realiza el setup mismo
			this.Setup(m => m.GetResult()).Returns(callback);

			//Retorna el mock de alto nivel con la operación ya mockeada
			client.Setup(clientExp).Returns(this.Object);
			return client;
		}
	}
}
using Monad;
using Moq;
using Movistar.Online.Common.Monad;
using UruIT.Serialization.Core;
using UruIT.RESTClient.Clases;
using UruIT.RESTClient.Interfaces;
using System;
using System.Net;

namespace UruIT.RESTClient.Mocks
{
	/// <summary>
	/// Mock de IRestClientMock
	/// </summary>
	public class IRestClientExecuterMock : Mock<IRestClientExecuter>
	{
		/// <summary>
		/// Mock de "Execute" usando callback
		/// </summary>
		public IRestClientExecuterMock ExecuteMock(Func<Uri, IRestRequest, IRestResponse> callback)
		{
			Setup(m => m.Execute(It.IsAny<Uri>(), It.IsAny<IRestRequest>())).Returns(callback);
			return this;
		}

		#region Otros

		/// <summary>
		/// Mock de "Execute" con un body JSON y callback utilizando objetos (la de/serialización JSON se hace behind-the-scenes)
		/// </summary>
		public IRestClientExecuterMock ExecuteWithJsonContentMock<TRequestBody>(IJsonConverter jsonConverter, Func<RestClientMockRequest<TRequestBody>, RestClientMockResponse> callback)
		{
			Setup(m => m.Execute(It.IsAny<Uri>(), It.IsAny<IRestRequest>())).Returns((Uri host, IRestRequest req) =>
			{
				//Se crea el request del mock
				var mockReq = new RestClientMockRequest<TRequestBody>()
				{
					Host = host,
					Method = req.Method,
					Resource = req.Resource,
					Body = req.Body.Select(b => jsonConverter.DeserializeObject<TRequestBody>(b.Content))
				};

				//Se aplica la funcion
				var mockRes = callback(mockReq);

				//Se crea la respuesta final
				var res = new RestResponse()
				{
					ContentType = "application/json",
					StatusCode = mockRes.StatusCode,
				};

				//Se setea el contenido solamente si viene
                if (mockReq.Body.HasValue)
                {
                    res.Content = jsonConverter.SerializeObject(mockReq.Body.Value);
                }

				return res;
			});
			return this;
		}

		#endregion Otros
	}

	/// <summary>
	/// Respuesta que puede ser utilizada en el mock, utilizando objetos
	/// </summary>
	public class RestClientMockResponse
	{
		public HttpStatusCode StatusCode { get; set; }

		public OptionStrict<object> Body { get; set; }
	}

	/// <summary>
	/// Request que puede ser utilizado en el mock, utilizando objetos
	/// </summary>
	/// <typeparam name="TRequestBody">Tipo del body</typeparam>
	public class RestClientMockRequest<TRequestBody>
	{
		public Uri Host { get; set; }

		public Method Method { get; set; }

		public string Resource { get; set; }

        public OptionStrict<TRequestBody> Body { get; set; }
	}
}
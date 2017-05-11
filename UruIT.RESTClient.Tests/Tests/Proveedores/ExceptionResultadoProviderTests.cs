using Microsoft.VisualStudio.TestTools.UnitTesting;
using Movistar.Online.Common.Types.Resultados;
using Movistar.Online.Common.Types.ResultadosRest;
using UruIT.RESTClient.Clases.Proveedores;

namespace UruIT.RESTClient.Tests.Tests.Proveedores
{
	[TestClass]
	public class ExceptionResultadoProviderTests
	{
		protected readonly ExceptionResultadoProvider<RestBusinessError, RestHttpError, RestException> provider;

		public ExceptionResultadoProviderTests()
		{
			provider = new ExceptionResultadoProvider<RestBusinessError, RestHttpError, RestException>();
		}

		[TestClass]
		public class ProvideException : ExceptionResultadoProviderTests
		{
			[TestMethod]
			public void WhenProvideThenException()
			{
				// arrange
				var error = new RestBusinessError
				{
					Resultado = RestErrorType.ValidationError,
					Mensaje = "Mensaje error",
					Detalle = "Detalle error",
				};

				// act
				var resultado = provider.ProvideException(error);

				// assert
				Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, resultado.HttpError.StatusCode);
				Assert.AreEqual("Mensaje error", resultado.HttpError.Message);
				Assert.AreEqual("Detalle error", resultado.HttpError.Details);
			}
		}
	}
}

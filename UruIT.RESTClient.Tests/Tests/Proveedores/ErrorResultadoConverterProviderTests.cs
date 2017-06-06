using Microsoft.VisualStudio.TestTools.UnitTesting;
using Monad;
using Movistar.Online.Common.Types.Resultados;
using Movistar.Online.Common.Types.ResultadosRest;
using UruIT.RESTClient.Clases;
using UruIT.RESTClient.Clases.Proveedores;

namespace UruIT.RESTClient.Tests.Tests.Proveedores
{
	[TestClass]
	public class ErrorResultadoConverterProviderTests
	{
		protected readonly ErrorResultadoConverterProvider<RestBusinessError, RestHttpError> provider;

		public ErrorResultadoConverterProviderTests()
		{
			provider = new ErrorResultadoConverterProvider<RestBusinessError, RestHttpError>();
		}

		[TestClass]
		public class ProvideError : ErrorResultadoConverterProviderTests
		{
			[TestMethod]
			public void WhenErrorThenConvertError()
			{
				// arrange
				var response = new RestResponse
				{

				};
				var errorRest = new RestHttpError
				{
					StatusCode = System.Net.HttpStatusCode.BadRequest,
					Message = "Error en servicio",
					Details = "Detalle de error en servicio",
				};

				// act
				var resultado = provider.ProvideError(errorRest, response);

				// assert
				Assert.AreEqual(RestErrorType.ValidationError, resultado.Resultado);
				Assert.AreEqual("Error en servicio", resultado.Mensaje);
				Assert.AreEqual("Detalle de error en servicio", resultado.Detalle);
			}

			[TestMethod]
			public void WhenNothingThenDefaultError()
			{
				// arrange
				var response = new RestResponse
				{
					StatusCode = System.Net.HttpStatusCode.InternalServerError,
					ErrorMessage = "ErrorMessage en servicio",
					Content = "{ 'ErrorMessage':'Error en servicio' }",
				};
				var errorRest = OptionStrict<RestHttpError>.Nothing;

				// act
				var resultado = provider.ProvideError(errorRest, response);

				// assert
				Assert.AreEqual(RestErrorType.InternalError, resultado.Resultado);
				Assert.AreEqual("ErrorMessage en servicio", resultado.Mensaje);
				Assert.AreEqual("{ 'ErrorMessage':'Error en servicio' }", resultado.Detalle);
			}
		}
	}
}

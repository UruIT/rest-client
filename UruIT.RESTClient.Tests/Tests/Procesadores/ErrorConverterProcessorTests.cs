using Microsoft.VisualStudio.TestTools.UnitTesting;
using Monad;
using UruIT.Serialization.Core;
using Movistar.Online.Common.Types.Resultados;
using Movistar.Online.Common.Types.ResultadosRest;
using UruIT.RESTClient.Clases;
using UruIT.RESTClient.Clases.Procesadores;
using UruIT.RESTClient.Interfaces;
using UruIT.RESTClient.Tests.Mocks.Proveedores;

namespace UruIT.RESTClient.Tests.Tests
{
	[TestClass]
	public class ErrorResultadoConverterProcessorTests : BaseProcessorTests
	{
		protected readonly IErrorConverterProviderMock<RestBusinessError, RestHttpError> ecpMock = new IErrorConverterProviderMock<RestBusinessError, RestHttpError>();

		protected IProcessorStructure<RestBusinessError, IJsonConverter> CreateProcessorStructure()
		{
			return base.CreateProcessorStructure(
				new ErrorConverterProcessor<RestBusinessError, RestHttpError, IJsonConverter>(ecpMock.Object)
                    .AddProcessors(new ErrorProcessor<OptionStrict<RestHttpError>, IJsonConverter>().Default()));
		}

		[TestClass]
		public class CanProcess : ErrorResultadoConverterProcessorTests
		{
			[TestMethod]
			public void WhenSuccessThenFalse()
			{
				// arrange
				var response = new RestResponse
				{
					StatusCode = System.Net.HttpStatusCode.OK,
				};
				var processor = CreateProcessorStructure();

				// act
				var resultado = processor.CanProcess(response);

				// assert
				Assert.AreEqual(false, resultado);
			}

			[TestMethod]
			public void WhenErrorThenTrue()
			{
				// arrange
				var response = new RestResponse
				{
					StatusCode = System.Net.HttpStatusCode.InternalServerError,
				};
				var processor = CreateProcessorStructure();

				// act
				var resultado = processor.CanProcess(response);

				// assert
				Assert.AreEqual(true, resultado);
			}
		}

		[TestClass]
		public class Process : ErrorResultadoConverterProcessorTests
		{
			[TestMethod]
			public void WhenErrorDeserializadoThenErrorConvertido()
			{
				// arrange
				var response = new RestResponse
				{
					StatusCode = System.Net.HttpStatusCode.BadRequest,
					ErrorMessage = "ErrorMessage en servicio",
					Content = "{ 'StatusCode':400, 'Message':'Error en servicio', 'Details':'Detalle de error en servicio' }",
				};
				var processor = CreateProcessorStructure();
				ProcessorUtilities.SetErrorSerializerForStructure(processor, jsonConverter);
				ecpMock.ProvideErrorMock((rest, resp) => rest.Value.ToBusinessError());

				// act
				var resultado = processor.Process(response, jsonConverter);

				// assert
				Assert.AreEqual(RestErrorType.ValidationError, resultado.Resultado);
				Assert.AreEqual("Error en servicio", resultado.Mensaje);
				Assert.AreEqual("Detalle de error en servicio", resultado.Detalle);
			}

			[TestMethod]
			public void WhenErrorNoDeserializadoThenErrorPorDefecto()
			{
				// arrange
				var response = new RestResponse
				{
					StatusCode = System.Net.HttpStatusCode.InternalServerError,
					ErrorMessage = "ErrorMessage en servicio",
					Content = "{ 'StatusCode':400, 'ErrorMessage':'Error en servicio' }",
				};
				var processor = CreateProcessorStructure();
				ProcessorUtilities.SetErrorSerializerForStructure(processor, jsonConverter);
				ecpMock.ProvideErrorMock((rest, resp) => (rest.HasValue ? rest.Value : new RestHttpError
					{
						StatusCode = response.StatusCode,
						Message = response.ErrorMessage,
						Details = response.Content,
					})
					.ToBusinessError());

				// act
				var resultado = processor.Process(response, jsonConverter);

				// assert
				Assert.AreEqual(RestErrorType.InternalError, resultado.Resultado);
				Assert.AreEqual("ErrorMessage en servicio", resultado.Mensaje);
				Assert.AreEqual("{ 'StatusCode':400, 'ErrorMessage':'Error en servicio' }", resultado.Detalle);
			}
		}
	}
}

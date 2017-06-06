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
	public class ErrorLoggerProcessorTests : BaseProcessorTests
	{
		private readonly IExceptionProviderMock<RestBusinessError, RestException> epMock = new IExceptionProviderMock<RestBusinessError, RestException>();
		private readonly IErrorConverterProviderMock<RestBusinessError, RestHttpError> ecpMock = new IErrorConverterProviderMock<RestBusinessError, RestHttpError>();

		public IProcessorStructure<RestBusinessError, IJsonConverter> CreateProcessorStructure()
		{
			return base.CreateProcessorStructure(new ErrorConverterProcessor<RestBusinessError, RestHttpError, IJsonConverter>(ecpMock.Object)
						.AddProcessors(new ErrorProcessor<OptionStrict<RestHttpError>, IJsonConverter>().Default()));
		}

		[TestClass]
		public class CanProcess : ErrorLoggerProcessorTests
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
					StatusCode = System.Net.HttpStatusCode.InternalServerError
				};
				var processor = CreateProcessorStructure();

				// act
				var resultado = processor.CanProcess(response);

				// assert
				Assert.AreEqual(true, resultado);
			}
		}

		[TestClass]
		public class Process : ErrorLoggerProcessorTests
		{
			[TestMethod]
			public void WhenProcessThenLog()
			{
				// arrange
				var response = new RestResponse
				{
					StatusCode = System.Net.HttpStatusCode.BadRequest,
					Content = "{ 'StatusCode':400, 'Message':'Error en servicio', 'Details':'Detalle error en servicio' }",
				};
				var processor = CreateProcessorStructure();
				ProcessorUtilities.SetErrorSerializerForStructure(processor, jsonConverter);
				ecpMock.ProvideErrorMock((rest, resp) => rest.Value.ToBusinessError());
				epMock.ProvideExceptionMock(res => new RestException(res));

				// act
				var resultado = processor.Process(response, jsonConverter);

				// assert
				Assert.AreEqual(RestErrorType.ValidationError, resultado.Resultado);
				Assert.AreEqual("Error en servicio", resultado.Mensaje);
				Assert.AreEqual("Detalle error en servicio", resultado.Detalle);
			}
		}
	}
}

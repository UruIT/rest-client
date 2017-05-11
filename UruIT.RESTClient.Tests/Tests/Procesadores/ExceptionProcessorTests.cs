using Microsoft.VisualStudio.TestTools.UnitTesting;
using UruIT.Serialization.Core;
using UruIT.Serialization.Core.ContractResolvers;
using Movistar.Online.Common.Types.Resultados;
using Movistar.Online.Common.Types.ResultadosRest;
using UruIT.RESTClient.Clases;
using UruIT.RESTClient.Clases.Procesadores;
using UruIT.RESTClient.Interfaces;
using System.Net;

namespace UruIT.RESTClient.Tests.Tests
{
	[TestClass]
	public class ExceptionResultadoProcessorTests
	{
		[TestMethod]
		public void WhenCodigoErrorThenExceptionResultadoProcessorPuedeProcesarlo()
		{
			// Arrange
			IRestResponse response = new RestResponse()
			{
				StatusCode = HttpStatusCode.InternalServerError,
				ContentType = "application/json",
				Content = "{\"StatusCode\": 200, \"Message\": \"Error generico\", \"Details\": \"Detalles\"}"
			};
			var jsonConverter = new JsonMONConverter();
			var processor = new ProcessorStructure<int, IJsonConverter>(
				new ExceptionResultadoProcessor<int>().Default());

			// Act - Assert
			Assert.IsTrue(processor.CanProcess(response));
		}

		[TestMethod]
		public void WhenCodigoOkThenExceptionResultadoProcessorNoPuedeProcesarlo()
		{
			// Arrange
			IRestResponse response = new RestResponse()
			{
				StatusCode = HttpStatusCode.OK,
				ContentType = "application/json",
				Content = "{\"StatusCode\": 200, \"Message\": \"Error generico\", \"Details\": \"Detalles\"}"
			};
			var jsonConverter = new JsonMONConverter();
			var processor = new ProcessorStructure<int, IJsonConverter>(
				new ExceptionResultadoProcessor<int>().Default());

			// Act - Assert
			Assert.IsFalse(processor.CanProcess(response));
		}

		[TestMethod]
		[ExpectedException(typeof(RestException))]
		public void WhenCodigoErrorThenExceptionResultadoProcessorRetornaBody()
		{
			// Arrange
			IRestResponse response = new RestResponse()
			{
				StatusCode = HttpStatusCode.InternalServerError,
				ContentType = "application/json",
				Content = "{\"StatusCode\": 500, \"Message\": \"Error generico\", \"Details\": \"Detalles\"}"
			};
			var jsonConverter = new JsonMONConverter();
			var processor = new ProcessorStructure<int, IJsonConverter>(
				new ExceptionResultadoProcessor<int>().Default());
			ProcessorUtilities.SetErrorSerializerForStructure(processor, jsonConverter);

			// Act
			try
			{
				Assert.IsTrue(processor.CanProcess(response));
				var res = processor.Process(response, jsonConverter);
			}
			// arrange
			catch (RestException ex)
			{
				Assert.AreEqual(HttpStatusCode.InternalServerError, ex.HttpError.StatusCode);
				Assert.AreEqual("Error generico", ex.HttpError.Message);
				Assert.AreEqual("Detalles", ex.HttpError.Details);
				throw;
			}

			Assert.IsTrue(processor.CanProcess(response));
		}

		[TestMethod]
		[ExpectedException(typeof(RestException))]
		public void WhenErrorNoEsResultadoRestThenRetornaOtroError()
		{
			// Arrange
			string bodyContents = "{\"@class\": 'Clase', \"MensajeError\": \"MensajeError\", \"Otro\": \"Otro\"}";
			IRestResponse response = new RestResponse()
			{
				StatusCode = HttpStatusCode.InternalServerError,
				ContentType = "application/json",
				Content = bodyContents
			};
			var jsonConverter = new JsonMONConverter();
			var jsonErrorSerializer = new JsonMONConverter();
			jsonErrorSerializer.Settings.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Error;
			jsonErrorSerializer.Settings.ContractResolver.ObjectContract = new RequiredAttributesObjectContract(RequiredLevel.AllowNull);

			var processor = new ProcessorStructure<int, IJsonConverter>(
				new ExceptionResultadoProcessor<int>().Default());
			ProcessorUtilities.SetErrorSerializerForStructure(processor, jsonErrorSerializer);

			// Act
			try
			{
				Assert.IsTrue(processor.CanProcess(response));
				var res = processor.Process(response, jsonConverter);
			}
			// arrange
			catch (RestException ex)
			{
				Assert.AreEqual(HttpStatusCode.InternalServerError, ex.HttpError.StatusCode);
				Assert.AreEqual(bodyContents, ex.HttpError.Details);
				throw;
			}

			Assert.IsTrue(processor.CanProcess(response));
		}
	}
}
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Monad;
using UruIT.Serialization.Core;
using UruIT.RESTClient.Clases;
using UruIT.RESTClient.Clases.Procesadores;
using UruIT.RESTClient.Interfaces;
using System.Net;

namespace UruIT.RESTClient.Tests.Tests
{
	[TestClass]
	public class OptionProcessorTests
	{
		[TestMethod]
		public void WhenOptionProcessorCon404ThenProcesarRespuestaRetornaNothing()
		{
			// Arrange
			IRestResponse response = new RestResponse()
			{
				StatusCode = HttpStatusCode.NotFound,
				ContentType = "application/json",
			};
			var jsonConverter = new JsonMONConverter();
			var processor = new ProcessorStructure<OptionStrict<int>, IJsonConverter>(
					new OptionAsNotFoundProcessor<int>().AddProcessors(new SuccessProcessor<int>().Default())
				);

			// Act
			Assert.IsTrue(processor.CanProcess(response));
			var res = processor.Process(response, jsonConverter);

			// Assert
			Assert.IsFalse(res.HasValue);
		}

		[TestMethod]
		public void WhenOptionProcessorConOkThenProcesarRespuestaRetornaJust()
		{
			// Arrange
			IRestResponse response = new RestResponse()
			{
				StatusCode = HttpStatusCode.OK,
				ContentType = "application/json",
				Content = "10"
			};
			var jsonConverter = new JsonMONConverter();
            var processor = new ProcessorStructure<OptionStrict<int>, IJsonConverter>(
					new OptionAsNotFoundProcessor<int>().AddProcessors(new SuccessProcessor<int>().Default())
				);

			// Act
			Assert.IsTrue(processor.CanProcess(response));
			var res = processor.Process(response, jsonConverter);

			// Assert
			Assert.IsTrue(res.HasValue);
			Assert.AreEqual(10, res.Value);
		}

		[TestMethod]
		public void WhenOptionProcessorSinOkNi404ThenNoPuedeProcesar()
		{
			// Arrange
			IRestResponse response = new RestResponse()
			{
				StatusCode = HttpStatusCode.InternalServerError,
				ContentType = "application/json",
			};
			var jsonConverter = new JsonMONConverter();
            var processor = new ProcessorStructure<OptionStrict<int>, IJsonConverter>(
					new OptionAsNotFoundProcessor<int>().AddProcessors(new SuccessProcessor<int>().Default())
				);

			// Act - Assert
			Assert.IsFalse(processor.CanProcess(response));
		}
	}
}
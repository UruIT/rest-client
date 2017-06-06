using Microsoft.VisualStudio.TestTools.UnitTesting;
using Monad;
using Monad.Utility;
using UruIT.Serialization.Core;
using UruIT.RESTClient.Clases;
using UruIT.RESTClient.Clases.Procesadores;
using UruIT.RESTClient.Interfaces;
using System.Net;

namespace UruIT.RESTClient.Tests.Tests
{
	[TestClass]
	public class UnitAsSuccessProcessorTests
	{
		[TestMethod]
		public void WhenOptionYUnitProcessorCon404ThenProcesarRespuestaRetornaNothing()
		{
			// Arrange
			IRestResponse response = new RestResponse()
			{
				StatusCode = HttpStatusCode.NotFound,
				ContentType = "application/json",
				Content = "10"
			};
			var jsonConverter = new JsonMONConverter();
            var processor = new ProcessorStructure<OptionStrict<Unit>, IJsonConverter>(new OptionAsNotFoundProcessor<Unit>().AddProcessors(new UnitAsSuccessProcessor()));

			// Act
			Assert.IsTrue(processor.CanProcess(response));
			var res = processor.Process(response, jsonConverter);

			// Assert
			Assert.IsFalse(res.HasValue);
		}

		[TestMethod]
		public void WhenOptionYUnitProcessorConOkThenProcesarRespuestaRetornaUnit()
		{
			// Arrange
			IRestResponse response = new RestResponse()
			{
				StatusCode = HttpStatusCode.OK,
				ContentType = "application/json",
				Content = "10"
			};
			var jsonConverter = new JsonMONConverter();
            var processor = new ProcessorStructure<OptionStrict<Unit>, IJsonConverter>(new OptionAsNotFoundProcessor<Unit>().AddProcessors(new UnitAsSuccessProcessor()));

			// Act
			Assert.IsTrue(processor.CanProcess(response));
			var res = processor.Process(response, jsonConverter);

			// Assert
			Assert.IsTrue(res.HasValue);
			Assert.AreEqual(Unit.Default, res.Value);
		}
	}
}
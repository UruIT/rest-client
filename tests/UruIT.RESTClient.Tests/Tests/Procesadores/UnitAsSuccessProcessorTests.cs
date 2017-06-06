using Microsoft.VisualStudio.TestTools.UnitTesting;
using Monad;
using Monad.Utility;
using System.Net;
using UruIT.RESTClient.Processors;
using UruIT.Serialization;

namespace UruIT.RESTClient.Tests
{
    [TestClass]
    public class UnitAsSuccessProcessorTests
    {
        [TestMethod]
        public void WhenOptionAndUnitProcessorWith404ThenReturnsNothing()
        {
            // Arrange
            IRestResponse response = new RestResponse()
            {
                StatusCode = HttpStatusCode.NotFound,
                ContentType = "application/json",
                Content = "10"
            };
            var jsonConverter = new JsonSerializer();
            var processor = new ProcessorStructure<OptionStrict<Unit>, IJsonSerializer>(new OptionAsNotFoundProcessor<Unit>().AddProcessors(new UnitAsSuccessProcessor()));

            // Act
            Assert.IsTrue(processor.CanProcess(response));
            var res = processor.Process(response, jsonConverter);

            // Assert
            Assert.IsFalse(res.HasValue);
        }

        [TestMethod]
        public void WhenOptionAndUnitWith200ThenReturnsUnit()
        {
            // Arrange
            IRestResponse response = new RestResponse()
            {
                StatusCode = HttpStatusCode.OK,
                ContentType = "application/json",
                Content = "10"
            };
            var jsonConverter = new JsonSerializer();
            var processor = new ProcessorStructure<OptionStrict<Unit>, IJsonSerializer>(new OptionAsNotFoundProcessor<Unit>().AddProcessors(new UnitAsSuccessProcessor()));

            // Act
            Assert.IsTrue(processor.CanProcess(response));
            var res = processor.Process(response, jsonConverter);

            // Assert
            Assert.IsTrue(res.HasValue);
            Assert.AreEqual(Unit.Default, res.Value);
        }
    }
}
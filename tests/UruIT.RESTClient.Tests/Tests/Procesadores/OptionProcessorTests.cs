using Microsoft.VisualStudio.TestTools.UnitTesting;
using Monad;
using System.Net;
using UruIT.RESTClient.Processors;
using UruIT.Serialization;

namespace UruIT.RESTClient.Tests
{
    [TestClass]
    public class OptionProcessorTests
    {
        [TestMethod]
        public void When404ThenProcessorReturnsNothing()
        {
            // Arrange
            IRestResponse response = new RestResponse()
            {
                StatusCode = HttpStatusCode.NotFound,
                ContentType = "application/json",
            };
            var jsonConverter = new JsonSerializer();
            var processor = new ProcessorStructure<OptionStrict<int>, IJsonSerializer>(
                    new OptionAsNotFoundProcessor<int>().AddProcessors(new SuccessProcessor<int>().Default())
                );

            // Act
            Assert.IsTrue(processor.CanProcess(response));
            var res = processor.Process(response, jsonConverter);

            // Assert
            Assert.IsFalse(res.HasValue);
        }

        [TestMethod]
        public void When200ThenProcessorReturnsJust()
        {
            // Arrange
            IRestResponse response = new RestResponse()
            {
                StatusCode = HttpStatusCode.OK,
                ContentType = "application/json",
                Content = "10"
            };
            var jsonConverter = new JsonSerializer();
            var processor = new ProcessorStructure<OptionStrict<int>, IJsonSerializer>(
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
        public void WhenDifferentCodeThenCantProcessIt()
        {
            // Arrange
            IRestResponse response = new RestResponse()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                ContentType = "application/json",
            };
            var jsonConverter = new JsonSerializer();
            var processor = new ProcessorStructure<OptionStrict<int>, IJsonSerializer>(
                    new OptionAsNotFoundProcessor<int>().AddProcessors(new SuccessProcessor<int>().Default())
                );

            // Act - Assert
            Assert.IsFalse(processor.CanProcess(response));
        }
    }
}
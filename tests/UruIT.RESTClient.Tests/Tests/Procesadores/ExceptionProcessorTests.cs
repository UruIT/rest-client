using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Net;
using UruIT.RESTClient.Processors;
using UruIT.Serialization;

namespace UruIT.RESTClient.Tests
{
    [TestClass]
    public class ExceptionResultadoProcessorTests
    {
        [TestMethod]
        public void WhenErrorCodeThenProcessorCanProcessIt()
        {
            // Arrange
            IRestResponse response = new RestResponse()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                ContentType = "application/json",
                Content = "{\"StatusCode\": 200, \"Message\": \"Generic error\", \"Details\": \"Details\"}"
            };
            var jsonConverter = new JsonSerializer();
            var processor = new ProcessorStructure<int, IJsonSerializer>(
                new RestExceptionProcessor<int>().Default());

            // Act - Assert
            Assert.IsTrue(processor.CanProcess(response));
        }

        [TestMethod]
        public void WhenOkCodeThenProcessorCantProcessIt()
        {
            // Arrange
            IRestResponse response = new RestResponse()
            {
                StatusCode = HttpStatusCode.OK,
                ContentType = "application/json",
                Content = "{\"StatusCode\": 200, \"Message\": \"Generic error\", \"Details\": \"Details\"}"
            };
            var jsonConverter = new JsonSerializer();
            var processor = new ProcessorStructure<int, IJsonSerializer>(
                new RestExceptionProcessor<int>().Default());

            // Act - Assert
            Assert.IsFalse(processor.CanProcess(response));
        }

        [TestMethod]
        [ExpectedException(typeof(RestException))]
        public void WhenErrorCodeThenProcessorReturnsBody()
        {
            // Arrange
            IRestResponse response = new RestResponse()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                ContentType = "application/json",
                Content = "{\"StatusCode\": 500, \"Message\": \"Generic error\", \"Details\": \"Details\"}"
            };
            var jsonConverter = new JsonSerializer();
            var processor = new ProcessorStructure<int, IJsonSerializer>(
                new RestExceptionProcessor<int>().Default());
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
                Assert.AreEqual("Generic error", ex.HttpError.Message);
                Assert.AreEqual("Details", ex.HttpError.Details);
                throw;
            }

            Assert.IsTrue(processor.CanProcess(response));
        }

        [TestMethod]
        [ExpectedException(typeof(RestException))]
        public void WhenErrorIsNotRestErrorThenReturnsAnotherError()
        {
            // Arrange
            string bodyContents = "{\"@class\": 'Clase', \"MessageError\": \"MessageError\", \"Otro\": \"Otro\"}";
            IRestResponse response = new RestResponse()
            {
                StatusCode = HttpStatusCode.InternalServerError,
                ContentType = "application/json",
                Content = bodyContents
            };
            var jsonConverter = new JsonSerializer();
            var jsonErrorSerializer = new JsonSerializer();
            jsonErrorSerializer.Settings.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Error;
            jsonErrorSerializer.Settings.ContractResolver.ObjectContract = new RequiredAttributesObjectContract(RequiredLevel.AllowNull);

            var processor = new ProcessorStructure<int, IJsonSerializer>(
                new RestExceptionProcessor<int>().Default());
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
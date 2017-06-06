using Microsoft.VisualStudio.TestTools.UnitTesting;
using Monad;
using System.Net;
using System.Runtime.Serialization;
using UruIT.RESTClient.Processors;
using UruIT.Serialization;

namespace UruIT.RESTClient.Tests
{
    [TestClass]
    public class TryContentDeserializationProcessorTests : BaseProcessorTests
    {
        protected IProcessorStructure<OptionStrict<T>, IJsonSerializer> CreateProcessorStructure<T>()
        {
            return CreateProcessorStructure(new TryContentDeserializationProcessor<T, IJsonSerializer>());
        }

        [TestClass]
        public class CanProcess : TryContentDeserializationProcessorTests
        {
            [TestMethod]
            public void WhenAnyThenTrue()
            {
                // arrange
                IRestResponse response1 = new RestResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    ContentType = "application/json",
                    Content = "10",
                };
                IRestResponse response2 = new RestResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    ContentType = "application/json",
                    Content = "{ 'error':'Error' }",
                };
                var processor = CreateProcessorStructure<int>();

                // act
                var resultado1 = processor.CanProcess(response1);
                var resultado2 = processor.CanProcess(response2);

                // assert
                Assert.AreEqual(true, resultado1);
                Assert.AreEqual(true, resultado2);
            }
        }

        [TestClass]
        public class Process : TryContentDeserializationProcessorTests
        {
            [TestMethod]
            public void WhenUnexpectedTypeThenReturnNothing()
            {
                // arrange
                var response = new RestResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    ContentType = "application/json",
                    Content = "{ 'result': 44 }",
                };
                var processor = CreateProcessorStructure<int>();

                // act
                var resultado = processor.Process(response, jsonConverter);

                // assert
                Assert.AreEqual(OptionStrict<int>.Nothing, resultado);
            }

            [TestMethod]
            public void WhenExpectedTypeThenDeserialize()
            {
                // Arrange
                IRestResponse response = new RestResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    ContentType = "application/json",
                    Content = "10"
                };
                var processor = CreateProcessorStructure<int>();

                //Act
                var resultado = processor.Process(response, jsonConverter);

                // Assert
                Assert.AreEqual(10, resultado);
            }
        }
    }

    [TestClass]
    public class ContentDeserializationProcessorTests : BaseProcessorTests
    {
        protected IProcessorStructure<T, IJsonSerializer> CreateProcessorStructure<T>()
        {
            return base.CreateProcessorStructure(new ContentDeserializationProcessor<T, IJsonSerializer>());
        }

        [TestClass]
        public class CanProcess : ContentDeserializationProcessorTests
        {
            [TestMethod]
            public void WhenAnyThenTrue()
            {
                // arrange
                IRestResponse response1 = new RestResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    ContentType = "application/json",
                    Content = "10",
                };
                IRestResponse response2 = new RestResponse
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    ContentType = "application/json",
                    Content = "{ 'error': 'Error' }",
                };
                var processor = CreateProcessorStructure<int>();

                // act
                var resultado1 = processor.CanProcess(response1);
                var resultado2 = processor.CanProcess(response2);

                // assert
                Assert.AreEqual(true, resultado1);
                Assert.AreEqual(true, resultado2);
            }
        }

        [TestClass]
        public class Process : ContentDeserializationProcessorTests
        {
            [TestMethod]
            [ExpectedException(typeof(SerializationException))]
            public void WhenUnexpectedTypeThenThrowError()
            {
                // arrange
                var response = new RestResponse
                {
                    StatusCode = HttpStatusCode.OK,
                    ContentType = "application/json",
                    Content = "{ 'result': 44 }",
                };
                var processor = CreateProcessorStructure<int>();

                try
                {
                    // act
                    processor.Process(response, jsonConverter);
                }
                catch (SerializationException ex)
                {
                    // assert
                    Assert.AreEqual("Error deserializing '{ 'result': 44 }' into type System.Int32.", ex.Message);
                    throw;
                }
            }

            [TestMethod]
            public void WhenExpectedTypeThenSerialize()
            {
                // Arrange
                IRestResponse response = new RestResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    ContentType = "application/json",
                    Content = "10"
                };
                var processor = CreateProcessorStructure<int>();

                //Act
                var resultado = processor.Process(response, jsonConverter);

                // Assert
                Assert.AreEqual(10, resultado);
            }
        }
    }
}
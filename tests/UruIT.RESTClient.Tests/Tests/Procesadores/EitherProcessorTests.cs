using Microsoft.VisualStudio.TestTools.UnitTesting;
using Monad;
using System.Net;
using UruIT.RESTClient.Processors;
using UruIT.Serialization;

namespace UruIT.RESTClient.Tests
{
    [TestClass]
    public class EitherProcessorTests
    {
        protected IJsonSerializer jsonConverter;

        public EitherProcessorTests()
        {
            jsonConverter = new JsonSerializer();
        }

        [TestClass]
        public class BasicEitherRestErrorProcessorTests : EitherProcessorTests
        {
            [TestMethod]
            public void WhenOkThenReturnRight()
            {
                // Arrange
                IRestResponse response = new RestResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    ContentType = "application/json",
                    Content = "10"
                };
                var processor = new ProcessorStructure<EitherStrict<RestBusinessError, int>, IJsonSerializer>(
                    new EitherRestErrorProcessor<int>().Default().AddProcessors(
                        new SuccessProcessor<int>().Default()));
                ProcessorUtilities.SetErrorSerializerForStructure(processor, jsonConverter);

                // Act
                Assert.IsTrue(processor.CanProcess(response));
                var res = processor.Process(response, jsonConverter);

                // Assert
                Assert.IsTrue(res.IsRight);
                Assert.AreEqual(10, res.Right);
            }

            [TestMethod]
            public void WhenErrorNoRestThenReturnLeft()
            {
                // Arrange
                IRestResponse response = new RestResponse()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    ContentType = "application/json",
                    Content = "GenericError"
                };

                var processor = new ProcessorStructure<EitherStrict<RestBusinessError, int>, IJsonSerializer>(
                        new EitherRestErrorProcessor<int>().Default()
                            .AddProcessors(new SuccessProcessor<int>().Default())
                    );
                ProcessorUtilities.SetErrorSerializerForStructure(processor, jsonConverter);

                // Act
                Assert.IsTrue(processor.CanProcess(response));
                var res = processor.Process(response, jsonConverter);

                // Assert
                Assert.IsTrue(res.IsLeft);
                Assert.AreEqual(RestErrorType.InternalError, res.Left.ErrorType);
                Assert.AreEqual("GenericError", res.Left.Details);
            }

            [TestMethod]
            public void WhenErrorRestThenReturnLeft()
            {
                // Arrange
                IRestResponse response = new RestResponse()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    ContentType = "application/json",
                    Content = "{'StatusCode': 400, 'Message': 'Message', 'Details': 'Details'}"
                };

                var processor = new ProcessorStructure<EitherStrict<RestBusinessError, int>, IJsonSerializer>(
                        new EitherRestErrorProcessor<int>().Default()
                            .AddProcessors(new SuccessProcessor<int>().Default())
                    );
                ProcessorUtilities.SetErrorSerializerForStructure(processor, jsonConverter);

                // Act
                Assert.IsTrue(processor.CanProcess(response));
                var res = processor.Process(response, jsonConverter);

                // Assert
                Assert.IsTrue(res.IsLeft);
                Assert.AreEqual(RestErrorType.ValidationError, res.Left.ErrorType);
                Assert.AreEqual("Message", res.Left.Message);
                Assert.AreEqual("Details", res.Left.Details);
            }
        }

        [TestClass]
        public class EitherRestErrorProcessorWithOptionProcessorTests : EitherProcessorTests
        {
            [TestMethod]
            public void WhenOkThenReturnRight()
            {
                // Arrange
                IRestResponse response = new RestResponse()
                {
                    StatusCode = HttpStatusCode.OK,
                    ContentType = "application/json",
                    Content = "10"
                };
                var processor = new ProcessorStructure<EitherStrict<RestBusinessError, OptionStrict<int>>, IJsonSerializer>(
                        new EitherRestErrorProcessor<OptionStrict<int>>().Default()
                            .AddProcessors(new OptionAsNotFoundProcessor<int>()
                                .AddProcessors(new SuccessProcessor<int>().Default()))
                    );
                ProcessorUtilities.SetErrorSerializerForStructure(processor, jsonConverter);

                // Act
                Assert.IsTrue(processor.CanProcess(response));
                var res = processor.Process(response, jsonConverter);

                // Assert
                Assert.IsTrue(res.IsRight);
                Assert.IsTrue(res.Right.HasValue);
                Assert.AreEqual(10, res.Right.Value);
            }

            [TestMethod]
            public void WhenNotFoundThenReturnNothing()
            {
                // Arrange
                IRestResponse response = new RestResponse()
                {
                    StatusCode = HttpStatusCode.NotFound,
                    ContentType = "application/json"
                };
                var processor = new ProcessorStructure<EitherStrict<RestBusinessError, OptionStrict<int>>, IJsonSerializer>(
                        new EitherRestErrorProcessor<OptionStrict<int>>().Default()
                            .AddProcessors(new OptionAsNotFoundProcessor<int>()
                                .AddProcessors(new SuccessProcessor<int>().Default()))
                    );
                ProcessorUtilities.SetErrorSerializerForStructure(processor, jsonConverter);

                // Act
                Assert.IsTrue(processor.CanProcess(response));
                var res = processor.Process(response, jsonConverter);

                // Assert
                Assert.IsTrue(res.IsRight);
                Assert.IsFalse(res.Right.HasValue);
            }

            [TestMethod]
            public void WhenErrorThenReturnLeft()
            {
                // Arrange
                IRestResponse response = new RestResponse()
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    ContentType = "application/json",
                    Content = "{'StatusCode': 400, 'Message': 'Message', 'Details': 'Details'}"
                };
                var processor = new ProcessorStructure<EitherStrict<RestBusinessError, OptionStrict<int>>, IJsonSerializer>(
                        new EitherRestErrorProcessor<OptionStrict<int>>().Default()
                            .AddProcessors(new OptionAsNotFoundProcessor<int>()
                                .AddProcessors(new SuccessProcessor<int>().Default()))
                    );
                ProcessorUtilities.SetErrorSerializerForStructure(processor, jsonConverter);

                // Act
                Assert.IsTrue(processor.CanProcess(response));
                var res = processor.Process(response, jsonConverter);

                // Assert
                Assert.IsTrue(res.IsLeft);
            }
        }
    }
}
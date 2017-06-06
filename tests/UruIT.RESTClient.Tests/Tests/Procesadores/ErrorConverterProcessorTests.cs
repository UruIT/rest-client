using Microsoft.VisualStudio.TestTools.UnitTesting;
using Monad;
using UruIT.RESTClient.Processors;
using UruIT.Serialization;

namespace UruIT.RESTClient.Tests
{
    [TestClass]
    public class ErrorConverterProcessorTests : BaseProcessorTests
    {
        protected readonly IErrorConverterProviderMock<RestBusinessError, RestHttpError> ecpMock = new IErrorConverterProviderMock<RestBusinessError, RestHttpError>();

        protected IProcessorStructure<RestBusinessError, IJsonSerializer> CreateProcessorStructure()
        {
            return base.CreateProcessorStructure(
                new ErrorConverterProcessor<RestBusinessError, RestHttpError, IJsonSerializer>(ecpMock.Object)
                    .AddProcessors(new ErrorProcessor<OptionStrict<RestHttpError>, IJsonSerializer>().Default()));
        }

        [TestClass]
        public class CanProcess : ErrorConverterProcessorTests
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
        public class Process : ErrorConverterProcessorTests
        {
            [TestMethod]
            public void WhenErrorInDeserializingThenReturnConvertedError()
            {
                // arrange
                var response = new RestResponse
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    ErrorMessage = "ErrorMessage",
                    Content = "{ 'StatusCode':400, 'Message':'Message in error', 'Details':'Detail in error' }",
                };
                var processor = CreateProcessorStructure();
                ProcessorUtilities.SetErrorSerializerForStructure(processor, jsonConverter);
                ecpMock.ProvideErrorMock((rest, resp) => rest.Value.ToBusinessError());

                // act
                var resultado = processor.Process(response, jsonConverter);

                // assert
                Assert.AreEqual(RestErrorType.ValidationError, resultado.ErrorType);
                Assert.AreEqual("Message in error", resultado.Message);
                Assert.AreEqual("Detail in error", resultado.Details);
            }

            [TestMethod]
            public void WhenOtherErrorThenReturnDefaultError()
            {
                // arrange
                var response = new RestResponse
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    ErrorMessage = "ErrorMessage",
                    Content = "{ 'StatusCode':400, 'ErrorMessage':'Message in error' }",
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
                Assert.AreEqual(RestErrorType.InternalError, resultado.ErrorType);
                Assert.AreEqual("ErrorMessage", resultado.Message);
                Assert.AreEqual("{ 'StatusCode':400, 'ErrorMessage':'Message in error' }", resultado.Details);
            }
        }
    }
}
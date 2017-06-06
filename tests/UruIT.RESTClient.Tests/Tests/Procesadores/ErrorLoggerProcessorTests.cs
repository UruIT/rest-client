using Microsoft.VisualStudio.TestTools.UnitTesting;
using Monad;
using UruIT.RESTClient.Processors;
using UruIT.Serialization;

namespace UruIT.RESTClient.Tests
{
    [TestClass]
    public class ErrorLoggerProcessorTests : BaseProcessorTests
    {
        private readonly IExceptionProviderMock<RestBusinessError, RestException> epMock = new IExceptionProviderMock<RestBusinessError, RestException>();
        private readonly IErrorConverterProviderMock<RestBusinessError, RestHttpError> ecpMock = new IErrorConverterProviderMock<RestBusinessError, RestHttpError>();

        public IProcessorStructure<RestBusinessError, IJsonSerializer> CreateProcessorStructure()
        {
            return base.CreateProcessorStructure(new ErrorConverterProcessor<RestBusinessError, RestHttpError, IJsonSerializer>(ecpMock.Object)
                        .AddProcessors(new ErrorProcessor<OptionStrict<RestHttpError>, IJsonSerializer>().Default()));
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
                    Content = "{ 'StatusCode':400, 'Message':'Service error', 'Details':'Error details' }",
                };
                var processor = CreateProcessorStructure();
                ProcessorUtilities.SetErrorSerializerForStructure(processor, jsonConverter);
                ecpMock.ProvideErrorMock((rest, resp) => rest.Value.ToBusinessError());
                epMock.ProvideExceptionMock(res => new RestException(res));

                // act
                var resultado = processor.Process(response, jsonConverter);

                // assert
                Assert.AreEqual(RestErrorType.ValidationError, resultado.ErrorType);
                Assert.AreEqual("Service error", resultado.Message);
                Assert.AreEqual("Error details", resultado.Details);
            }
        }
    }
}
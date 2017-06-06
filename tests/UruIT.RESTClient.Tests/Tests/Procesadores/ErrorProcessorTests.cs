using Microsoft.VisualStudio.TestTools.UnitTesting;
using Monad;
using UruIT.RESTClient.Processors;
using UruIT.Serialization;

namespace UruIT.RESTClient.Tests
{
    [TestClass]
    public class ErrorProcessorTests : BaseProcessorTests
    {
        protected IProcessorStructure<OptionStrict<T>, IJsonSerializer> CreateProcessorStructure<T>()
        {
            return base.CreateProcessorStructure(new ErrorProcessor<OptionStrict<T>, IJsonSerializer>().Default());
        }

        [TestClass]
        public class CanProcess : ErrorProcessorTests
        {
            private class Error
            {
                public string Detail { get; set; }
            }

            [TestMethod]
            public void WhenSuccessThenFalse()
            {
                // arrange
                var response = new RestResponse
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    ContentType = "application/json",
                    Content = "{ 'Detail':'Error detail' }",
                };
                var processor = CreateProcessorStructure<Error>();

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
                    ContentType = "application/json",
                    Content = "{ 'Detail':'Error detail' }",
                };
                var processor = CreateProcessorStructure<Error>();

                // act
                var resultado = processor.CanProcess(response);

                // assert
                Assert.AreEqual(true, resultado);
            }
        }

        [TestClass]
        public class Process : ErrorProcessorTests
        {
            private class Error
            {
                public string Detail { get; set; }
            }

            [TestMethod]
            public void WhenUnrequitedErrorThenReturnNothing()
            {
                // arrange
                var response = new RestResponse
                {
                    Content = "{ 'Error':'Error detail' }"
                };
                var processor = CreateProcessorStructure<Error>();
                ProcessorUtilities.SetErrorSerializerForStructure(processor, jsonConverter);

                // act
                var resultado = processor.Process(response, jsonConverter);

                // assert
                Assert.AreEqual(OptionStrict<Error>.Nothing, resultado);
            }

            [TestMethod]
            public void WhenRequitedErrorThenReturnParsedError()
            {
                // arrange
                var response = new RestResponse
                {
                    Content = "{ 'Detail':'Error detail' }"
                };
                var processor = CreateProcessorStructure<Error>();
                ProcessorUtilities.SetErrorSerializerForStructure(processor, jsonConverter);

                // act
                var resultado = processor.Process(response, jsonConverter);

                // assert
                Assert.AreEqual("Error detail", resultado.Value.Detail);
            }
        }
    }
}
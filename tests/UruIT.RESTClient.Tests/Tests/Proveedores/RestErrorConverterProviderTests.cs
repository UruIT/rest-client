using Microsoft.VisualStudio.TestTools.UnitTesting;
using Monad;
using UruIT.RESTClient.Providers;

namespace UruIT.RESTClient.Tests.Proveedores
{
    [TestClass]
    public class RestErrorConverterProviderTests
    {
        protected readonly RestErrorConverterProvider<RestBusinessError, RestHttpError> provider;

        public RestErrorConverterProviderTests()
        {
            provider = new RestErrorConverterProvider<RestBusinessError, RestHttpError>();
        }

        [TestClass]
        public class ProvideError : RestErrorConverterProviderTests
        {
            [TestMethod]
            public void WhenErrorThenConvertError()
            {
                // arrange
                var response = new RestResponse
                {
                };
                var errorRest = new RestHttpError
                {
                    StatusCode = System.Net.HttpStatusCode.BadRequest,
                    Message = "Service error",
                    Details = "Error detail",
                };

                // act
                var resultado = provider.ProvideError(errorRest, response);

                // assert
                Assert.AreEqual(RestErrorType.ValidationError, resultado.ErrorType);
                Assert.AreEqual("Service error", resultado.Message);
                Assert.AreEqual("Error detail", resultado.Details);
            }

            [TestMethod]
            public void WhenNothingThenDefaultError()
            {
                // arrange
                var response = new RestResponse
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    ErrorMessage = "ErrorMessage",
                    Content = "{ 'ErrorMessage':'Error' }",
                };
                var errorRest = OptionStrict<RestHttpError>.Nothing;

                // act
                var resultado = provider.ProvideError(errorRest, response);

                // assert
                Assert.AreEqual(RestErrorType.InternalError, resultado.ErrorType);
                Assert.AreEqual("ErrorMessage", resultado.Message);
                Assert.AreEqual("{ 'ErrorMessage':'Error' }", resultado.Details);
            }
        }
    }
}
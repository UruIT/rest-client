using Microsoft.VisualStudio.TestTools.UnitTesting;
using UruIT.RESTClient.Providers;

namespace UruIT.RESTClient.Tests.Proveedores
{
    [TestClass]
    public class RestErrorExceptionProviderTests
    {
        protected readonly RestErrorExceptionProvider<RestBusinessError, RestHttpError, RestException> provider;

        public RestErrorExceptionProviderTests()
        {
            provider = new RestErrorExceptionProvider<RestBusinessError, RestHttpError, RestException>();
        }

        [TestClass]
        public class ProvideException : RestErrorExceptionProviderTests
        {
            [TestMethod]
            public void WhenProvideThenException()
            {
                // arrange
                var error = new RestBusinessError
                {
                    ErrorType = RestErrorType.ValidationError,
                    Message = "Message error",
                    Details = "Detail error",
                };

                // act
                var resultado = provider.ProvideException(error);

                // assert
                Assert.AreEqual(System.Net.HttpStatusCode.BadRequest, resultado.HttpError.StatusCode);
                Assert.AreEqual("Message error", resultado.HttpError.Message);
                Assert.AreEqual("Detail error", resultado.HttpError.Details);
            }
        }
    }
}
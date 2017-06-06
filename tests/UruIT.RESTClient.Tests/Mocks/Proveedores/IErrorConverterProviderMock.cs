using Monad;
using Moq;
using System;

namespace UruIT.RESTClient.Tests
{
    public class IErrorConverterProviderMock<TError, TErrorRest> : Mock<IErrorConverterProvider<TError, TErrorRest>>
    {
        public IErrorConverterProviderMock<TError, TErrorRest> ProvideErrorMock(Func<OptionStrict<TErrorRest>, IRestResponse, TError> callback)
        {
            Setup(x => x.ProvideError(Moq.It.IsAny<OptionStrict<TErrorRest>>(), Moq.It.IsAny<IRestResponse>()))
                .Returns(callback);

            return this;
        }
    }
}
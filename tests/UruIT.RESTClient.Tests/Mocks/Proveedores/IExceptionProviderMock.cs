using Moq;
using System;

namespace UruIT.RESTClient.Tests
{
    public class IExceptionProviderMock<TError, TException> : Mock<IExceptionProvider<TError, TException>>
        where TException : Exception
    {
        public IExceptionProviderMock<TError, TException> ProvideExceptionMock(Func<TError, TException> callback)
        {
            Setup(x => x.ProvideException(Moq.It.IsAny<TError>()))
                .Returns(callback);

            return this;
        }
    }
}
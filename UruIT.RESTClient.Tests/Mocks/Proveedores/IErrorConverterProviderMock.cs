using Monad;
using UruIT.RESTClient.Interfaces;
using UruIT.RESTClient.Interfaces.Proveedores;
using System;

namespace UruIT.RESTClient.Tests.Mocks.Proveedores
{
	public class IErrorConverterProviderMock<TError, TErrorRest> : Moq.Mock<IErrorConverterProvider<TError, TErrorRest>>
	{
		public IErrorConverterProviderMock<TError, TErrorRest> ProvideErrorMock(Func<OptionStrict<TErrorRest>, IRestResponse, TError> callback)
		{
            Setup(x => x.ProvideError(Moq.It.IsAny<OptionStrict<TErrorRest>>(), Moq.It.IsAny<IRestResponse>()))
				.Returns(callback);

			return this;
		}
	}
}

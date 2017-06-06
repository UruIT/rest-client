using UruIT.RESTClient.Interfaces.Proveedores;
using System;

namespace UruIT.RESTClient.Tests.Mocks.Proveedores
{
	public class IExceptionProviderMock<TError, TException> : Moq.Mock<IExceptionProvider<TError, TException>>
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

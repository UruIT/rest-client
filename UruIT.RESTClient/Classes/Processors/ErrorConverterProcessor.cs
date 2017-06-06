using Monad;
using UruIT.Serialization.Core;
using UruIT.RESTClient.Interfaces;
using UruIT.RESTClient.Interfaces.Proveedores;

namespace UruIT.RESTClient.Clases.Procesadores
{
	public class ErrorConverterProcessor<TError, TErrorRest, TSerializer>
        : RecursiveProcessorNode<TError, OptionStrict<TErrorRest>, TSerializer>
		where TSerializer : ISerializer
	{
		private readonly IErrorConverterProvider<TError, TErrorRest> errorConverterProvider;

		public ErrorConverterProcessor(IErrorConverterProvider<TError, TErrorRest> errorConverterProvider)
		{
			this.errorConverterProvider = errorConverterProvider;
		}

		protected override bool CanProcessSub(IRestResponse response)
		{
			return ProcessorStructure.CanProcess(response);
		}

		protected override TError ProcessSub(IRestResponse response, TSerializer serializer)
		{
			var errorRest = ProcessorStructure.Process(response, serializer);
			return errorConverterProvider.ProvideError(errorRest, response);
		}

	}
}

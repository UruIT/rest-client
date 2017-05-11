using UruIT.Serialization.Core;
using UruIT.RESTClient.Interfaces;
using UruIT.RESTClient.Interfaces.Procesadores;
using System.Net;

namespace UruIT.RESTClient.Clases.Procesadores
{
	/// <summary>
	/// Proxy de procesador que restringe el procesador interno a procesar SOLO cuando el resultado es ERROR
	/// </summary>
	/// <typeparam name="TResult">Tipo del resultado</typeparam>
	/// <typeparam name="TSerializer">Tipo del serializador</typeparam>
	public class ErrorProcessor<TErrorRest, TSerializer>
		: RecursiveProcessorNode<TErrorRest, TErrorRest, TSerializer>, IErrorProcessor<TSerializer>
		where TSerializer : ISerializer
	{
		public TSerializer ErrorSerializer { private get; set; }

		protected override bool CanProcessSub(IRestResponse response)
		{
			return !response.StatusCode.IsSuccessful() && ProcessorStructure.CanProcess(response);
		}

		protected override TErrorRest ProcessSub(IRestResponse response, TSerializer serializer)
		{
			return ProcessorStructure.Process(response, ErrorSerializer);
		}
	}
}

using UruIT.Serialization.Core;
using UruIT.RESTClient.Interfaces;
using System.Net;

namespace UruIT.RESTClient.Clases.Procesadores
{
	/// <summary>
	/// Proxy de procesador que restringe el procesador interno a procesar SOLO cuando el resultado es OK
	/// </summary>
	/// <typeparam name="TResult">Tipo del resultado</typeparam>
	/// <typeparam name="TSerializer">Tipo del serializador</typeparam>
	public class SuccessProcessor<TResult, TSerializer> : RecursiveProcessorNode<TResult, TResult, TSerializer>
		where TSerializer : ISerializer
	{
		protected override bool CanProcessSub(IRestResponse response)
		{
			//Solo se procesa si el response es exitoso y el recursivo puede procesar también
			return response.StatusCode.IsSuccessful() && ProcessorStructure.CanProcess(response);
		}

		protected override TResult ProcessSub(IRestResponse response, TSerializer serializer)
		{
			return ProcessorStructure.Process(response, serializer);
		}
	}

	/// <summary>
	/// Procesador de resultado OK con serializador JSON
	/// </summary>
	/// <typeparam name="TResult"></typeparam>
	public class SuccessProcessor<TResult> : SuccessProcessor<TResult, IJsonConverter>
	{
	}
}
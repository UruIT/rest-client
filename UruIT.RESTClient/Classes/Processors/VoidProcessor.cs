using UruIT.Serialization.Core;
using UruIT.RESTClient.Interfaces;
using System;

namespace UruIT.RESTClient.Clases.Procesadores
{
	/// <summary>
	/// Procesador que nunca procesa nada.
	///
	/// Es la identidad cuando se toma a la lista de procesadores "alternativos" como un monoide.
	/// </summary>
	/// <typeparam name="TResult">Tipo del resultado</typeparam>
	public class VoidProcessor<TResult, TSerializer> : ISimpleProcessorNode<TResult, TSerializer>
		where TSerializer : ISerializer
	{
		public bool CanProcess(IRestResponse response)
		{
			return false;
		}

		public TResult Process(IRestResponse response, TSerializer serializer)
		{
			throw new InvalidOperationException("No debería procesarse esta respuesta");
		}
	}

	/// <summary>
	/// Procesador vacío con serializador fijo para JSON
	/// </summary>
	public class VoidProcessor<TResult> : VoidProcessor<TResult, IJsonConverter>
	{
	}
}
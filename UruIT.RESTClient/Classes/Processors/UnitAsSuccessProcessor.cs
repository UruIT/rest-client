using Monad.Utility;
using UruIT.Serialization.Core;
using UruIT.RESTClient.Interfaces;
using System.Net;

namespace UruIT.RESTClient.Clases.Procesadores
{
	/// <summary>
	/// Procesador que mapea códigos HTTP de éxito 2xx a Unit.Default
	/// </summary>
	public class UnitAsSuccessProcessor<TSerializer> : ISimpleProcessorNode<Unit, TSerializer>
		where TSerializer : ISerializer
	{
		public bool CanProcess(IRestResponse response)
		{
			//Solo se procesa si el response dio 2xx
			return response.StatusCode.IsSuccessful();
		}

		public Unit Process(IRestResponse response, TSerializer serializer)
		{
			return Unit.Default;
		}
	}

	/// <summary>
	/// Procesador de Unit con serializador fijo para JSON
	/// </summary>
	public class UnitAsSuccessProcessor : UnitAsSuccessProcessor<IJsonConverter>
	{
	}
}
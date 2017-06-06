using UruIT.Serialization.Core;
using UruIT.RESTClient.Interfaces;
using System.Net;

namespace UruIT.RESTClient.Clases.Procesadores
{
	/// <summary>
	/// Procesador que mapea código HTTP 404 (Not Found) a False
	/// </summary>
	/// <typeparam name="TSerializer">Tipo del serializador</typeparam>
	public class BoolAsNotFoundProcessor<TSerializer> : ISimpleProcessorNode<bool, TSerializer>
		where TSerializer : ISerializer
	{
		public bool CanProcess(IRestResponse response)
		{
			//Solo se procesa si el response dio Ok o 404
			return response.StatusCode == HttpStatusCode.NotFound || response.StatusCode.IsSuccessful();
		}

		public bool Process(IRestResponse response, TSerializer serializer)
		{
			return response.StatusCode != HttpStatusCode.NotFound;
		}
	}

	/// <summary>
	/// Procesador booleano con el serializador fijo para JSON
	/// </summary>
	public class BoolAsNotFoundProcessor : BoolAsNotFoundProcessor<IJsonConverter>
	{
	}
}
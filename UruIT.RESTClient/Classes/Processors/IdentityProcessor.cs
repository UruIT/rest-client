using UruIT.Serialization.Core;
using UruIT.RESTClient.Interfaces;

namespace UruIT.RESTClient.Clases.Procesadores
{
	/// <summary>
	/// Procesador identidad. La composición de este procesador con cualquier otro es equivalente al otro procesador.
	///
	/// Es la identidad cuando se toma la composición de procesadores utilizando solo RecursiveProcessorNode como una
	/// categoría, con los tipos C# como objetos y el constructor de tipos RecursiveProcessorNode&lt;,&gt; como morfismo.
	/// </summary>
	/// <typeparam name="TResult">Tipo del resultado</typeparam>
	/// <typeparam name="TSerializer">Tipo del serializador</typeparam>
	public class IdentityProcessor<TResult, TSerializer> : RecursiveProcessorNode<TResult, TResult, TSerializer>
		where TSerializer : ISerializer
	{
		protected override bool CanProcessSub(IRestResponse response)
		{
			return ProcessorStructure.CanProcess(response);
		}

		protected override TResult ProcessSub(IRestResponse response, TSerializer serializer)
		{
			return ProcessorStructure.Process(response, serializer);
		}
	}

	/// <summary>
	/// Procesador identidad con el serializador fijo para JSON
	/// </summary>
	public class IdentityProcessor<TResult> : IdentityProcessor<TResult, IJsonConverter>
	{

	}
}
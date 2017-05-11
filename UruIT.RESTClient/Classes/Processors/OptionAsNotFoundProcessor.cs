using Monad;
using UruIT.Serialization.Core;
using UruIT.RESTClient.Interfaces;
using System.Net;

namespace UruIT.RESTClient.Clases.Procesadores
{
	/// <summary>
	/// Procesador que, si el tipo resultante es un Option[T], mapea códigos HTTP 404 (Not Found) a Option.Nothing
	/// y objetos a Option.Just
	/// </summary>
	/// <typeparam name="TResult">Tipo parametrizado por el Option</typeparam>
	/// <typeparam name="TSerializer">Tipo del serializador</typeparam>
    public class OptionAsNotFoundProcessor<TResult, TSerializer> : RecursiveProcessorNode<OptionStrict<TResult>, TResult, TSerializer>
		where TSerializer : ISerializer
	{
		public OptionAsNotFoundProcessor()
			: base()
		{
		}

		public OptionAsNotFoundProcessor(IProcessorStructure<TResult, TSerializer> processorStructure)
			: base(processorStructure)
		{
		}

		protected override bool CanProcessSub(IRestResponse response)
		{
			//Solo se procesa si el response dio 404 o si el otro procesador también puede procesarlo
			return response.StatusCode == HttpStatusCode.NotFound || ProcessorStructure.CanProcess(response);
		}

		protected override OptionStrict<TResult> ProcessSub(IRestResponse response, TSerializer serializer)
		{
			//Si respuesta es 404 retorna Nothing, sino returna Just usando el body
			if (response.StatusCode == HttpStatusCode.NotFound)
			{
                return OptionStrict<TResult>.Nothing;
			}
			else
			{
				return ProcessorStructure.Process(response, serializer);
			}
		}
	}

	/// <summary>
	/// Procesador de Option que tiene el serializador fijo para JSON
	/// </summary>
	public class OptionAsNotFoundProcessor<TResult> : OptionAsNotFoundProcessor<TResult, IJsonConverter>
	{
		public OptionAsNotFoundProcessor()
			: base()
		{
		}

		public OptionAsNotFoundProcessor(IProcessorStructure<TResult, IJsonConverter> processorStructure)
			: base(processorStructure)
		{
		}
	}
}
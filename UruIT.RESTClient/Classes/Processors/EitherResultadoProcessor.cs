using UruIT.Serialization.Core;
using Movistar.Online.Common.Types.Resultados;
using Movistar.Online.Common.Types.ResultadosRest;

namespace UruIT.RESTClient.Clases.Procesadores
{
	/// <summary>
	/// Extensión de EitherProcessor para el genérico de Resultado
	/// </summary>
	/// <typeparam name="TResultado">Tipo del resultado Left</typeparam>
	/// <typeparam name="TResult">Tipo del pipeline</typeparam>
	/// <typeparam name="TResultadoRest">Tipo del resultado Left en el formato que viene serializado</typeparam>
	/// <typeparam name="TSerializer">Tipo del serializador</typeparam>
	public class EitherResultadoProcessor<TResultado, TResult, TResultadoRest, TSerializer>
		: EitherProcessor<TResultado, TResult, TSerializer>
		where TResultado : RestBusinessError<TResultado, TResultadoRest>
		where TResultadoRest : RestHttpError<TResultado, TResultadoRest>
		where TSerializer : ISerializer
	{
		public EitherResultadoProcessor()
		{
		}

		public EitherResultadoProcessor(Interfaces.IProcessorStructure<TResult, TSerializer> processorStructure)
			: base(processorStructure)
		{
		}
	}

	/// <summary>
	/// Extensión de EitherProcessor para el base de Resultado
	/// </summary>
	/// <typeparam name="TResult">Tipo del pipeline</typeparam>
	/// <typeparam name="TSerializer">Tipo del serializador</typeparam>
	public class EitherResultadoProcessor<TResult, TSerializer> : EitherResultadoProcessor<RestBusinessError, TResult, RestHttpError, TSerializer>
		where TSerializer : ISerializer
	{
	}

	/// <summary>
	/// Extensión de EitherProcessor para el base de Resultado y serializador JSON
	/// </summary>
	/// <typeparam name="TResult">Tipo del serializador</typeparam>
	public class EitherResultadoProcessor<TResult> : EitherResultadoProcessor<TResult, IJsonConverter>
	{
	}
}

using UruIT.Serialization.Core;
using Movistar.Online.Common.Types.Resultados;
using Movistar.Online.Common.Types.ResultadosRest;
using UruIT.RESTClient.Clases.Proveedores;
using System;

namespace UruIT.RESTClient.Clases.Procesadores
{
	/// <summary>
	/// Extensión de ExceptionProcesor para el genérico de Resultado, ResultadoRest y RestException
	/// </summary>
	public class ExceptionResultadoProcessor<TResult, TResultado, TResultadoRest, TRestException, TSerializer>
		: ExceptionProcessor<TResult, TResultado, TRestException, TSerializer>
		where TResultado : RestBusinessError<TResultado, TResultadoRest>
		where TResultadoRest : RestHttpError<TResultado, TResultadoRest>
		where TRestException : RestException<TResultado, TResultadoRest>
		where TSerializer : ISerializer
	{
		public ExceptionResultadoProcessor()
			: base(new ExceptionResultadoProvider<TResultado, TResultadoRest, TRestException>())
		{
		}
	}

	/// <summary>
	/// ExceptionResultadoProcessor utilizando serializador JSON
	/// </summary>
	public class ExceptionResultadoProcessor<TResult, TResultado, TResultadoRest, TRestException>
		: ExceptionResultadoProcessor<TResult, TResultado, TResultadoRest, TRestException, IJsonConverter>
		where TResultado : RestBusinessError<TResultado, TResultadoRest>
		where TResultadoRest : RestHttpError<TResultado, TResultadoRest>
		where TRestException : RestException<TResultado, TResultadoRest>
	{
	}

	/// <summary>
	/// ExceptionResultadoProcessor utilizando serializador JSON y Resultado y ResultadoRest simples
	/// </summary>
	public class ExceptionResultadoProcessor<TResult>
		: ExceptionResultadoProcessor<TResult, RestBusinessError, RestHttpError, RestException>
	{
	}
}

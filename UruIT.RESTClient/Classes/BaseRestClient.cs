using UruIT.Serialization.Core;
using Movistar.Online.Common.Types.Resultados;
using Movistar.Online.Common.Types.ResultadosRest;
using UruIT.RESTClient.Clases.Procesadores;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UruIT.RESTClient.Clases
{
	public abstract class BaseRestClient<TSerializer>
		where TSerializer : ISerializer
	{
		/// <summary>
		/// Crea el serializador por defecto
		/// </summary>
		/// <returns></returns>
		protected abstract TSerializer CreateSuccessSerializer();

		/// <summary>
		/// Crea el serializador de errores
		/// </summary>
		/// <returns></returns>
		protected abstract TSerializer CreateErrorSerializer();

		/// <summary>
		/// Se encarga de obtener el procesador de excepciones
		/// </summary>
		protected virtual ExceptionProcessor<TResult, RestBusinessError, RestException, TSerializer> ObtenerProcesadorExcepciones<TResult>()
		{
			return new ExceptionResultadoProcessor<TResult, RestBusinessError, RestHttpError, RestException, TSerializer>().Default();
		}
	}
}

using Monad;
using UruIT.Serialization.Core;
using Movistar.Online.Common.Types.Resultados;
using Movistar.Online.Common.Types.ResultadosRest;
using UruIT.RESTClient.Clases.Proveedores;
using UruIT.RESTClient.Interfaces.Proveedores;
using System;

namespace UruIT.RESTClient.Clases.Procesadores
{
	/// <summary>
	/// Extensiones de ExceptionProcessor para crear los encadenamientos usados por defecto
	/// </summary>
	public static class ExceptionProcessorExtensions
	{
		/// <summary>
		/// Configura un exceptionProcessor con los procesadores necesarios para lanzar una
		/// excepción TException cuando la respuesta del servidor es error.
		/// </summary>
		/// <typeparam name="TResult">Tipo del caso de éxito</typeparam>
		/// <typeparam name="TError">Tipo del error de negocio</typeparam>
		/// <typeparam name="TErrorRest">Tipo del error que retorna el servidor</typeparam>
		/// <typeparam name="TException">Tipo de la excepción que contendrá al error de negocio</typeparam>
		/// <typeparam name="TSerializer">Tipo del serializador</typeparam>
		/// <param name="exceptionProcessor">ExceptionProcessor a configurar</param>
		/// <param name="errorConverterProvider">Convierte del error que se espera del servidor al error de negocio</param>
		/// <returns>El mismo ExceptionProcessor ya configurado</returns>
		public static ExceptionProcessor<TResult, TError, TException, TSerializer>
			Default<TResult, TError, TErrorRest, TException, TSerializer>(
			this ExceptionProcessor<TResult, TError, TException, TSerializer> exceptionProcessor,
			IErrorConverterProvider<TError, TErrorRest> errorConverterProvider)
			where TSerializer : ISerializer
			where TException : Exception
		{
			var errorConverterProcessor = new ErrorConverterProcessor<TError, TErrorRest, TSerializer>(errorConverterProvider);
            var errorProcessor = new ErrorProcessor<OptionStrict<TErrorRest>, TSerializer>().Default();
			
			exceptionProcessor.AddProcessors(
				errorConverterProcessor.AddProcessors(
					errorProcessor));

			return exceptionProcessor;
		}

		/// <summary>
		/// Configura un exceptionResultadoProcessor con los procesadores necesarios para lanzar una
		/// excepción TRestException cuando la respuesta del servidor es error.
		/// </summary>
		/// <typeparam name="TResult">Tipo del caso de éxito</typeparam>
		/// <typeparam name="TResultado">Tipo del error de negocio</typeparam>
		/// <typeparam name="TResultadoRest">Tipo del error que retorna el servidor</typeparam>
		/// <typeparam name="TRestException">Tipo de la excepción que contendrá al error de negocio</typeparam>
		/// <typeparam name="TSerializer">Tipo del serializador</typeparam>
		/// <param name="exceptionProcessor">ExceptionProcessor a configurar</param>
		/// <param name="errorConverterProvider">Convierte del error que se espera del servidor al error de negocio</param>
		/// <returns>El mismo ExceptionResultadoProcessor ya configurado</returns>
		public static ExceptionResultadoProcessor<TResult, TResultado, TResultadoRest, TRestException, TSerializer>
			Default<TResult, TResultado, TResultadoRest, TRestException, TSerializer>(
			this ExceptionResultadoProcessor<TResult, TResultado, TResultadoRest, TRestException, TSerializer> exceptionProcessor)
			where TResultado : RestBusinessError<TResultado, TResultadoRest>
			where TResultadoRest : RestHttpError<TResultado, TResultadoRest>
			where TRestException : RestException<TResultado, TResultadoRest>
			where TSerializer : ISerializer
		{
			var errorConverterProvider = new ErrorResultadoConverterProvider<TResultado, TResultadoRest>();
			exceptionProcessor.Default(errorConverterProvider);
			
			return exceptionProcessor;
		}
	}
}

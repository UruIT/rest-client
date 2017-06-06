using Monad;
using UruIT.RESTClient.Interfaces;
using UruIT.RESTClient.Interfaces.Proveedores;
using System;

namespace UruIT.RESTClient.Clases.Proveedores
{
	/// <summary>
	/// Provee la conversión de TResultadoRest en TResultado para el genérico de Resultado y ResultadoRest
	/// </summary>
	/// <typeparam name="TResultado">Tipo del error de negocio</typeparam>
	/// <typeparam name="TResultadoRest">Tipo del error de la respuesta</typeparam>
	public class ErrorResultadoConverterProvider<TResultado, TResultadoRest>
		: IErrorConverterProvider<TResultado, TResultadoRest>
		where TResultado : Common.Types.Resultados.Resultado<TResultado, TResultadoRest>
		where TResultadoRest : Common.Types.ResultadosRest.ResultadoRest<TResultado, TResultadoRest>
	{
		/// <summary>
		/// Convierte de TResultadoRest en TResultado. Si es Nothing, usa valores por defecto del response
		/// </summary>
		/// <param name="errorRest">Error de la respuesta</param>
		/// <param name="response">Respuesta del servidor</param>
		/// <returns>Error de negocio con la información pertinent</returns>
        public TResultado ProvideError(OptionStrict<TResultadoRest> errorRest, IRestResponse response)
		{
            var result = errorRest.HasValue ? errorRest.Value : WhenCantDeserializeError(response);
            return result.ToResultado();
		}

		/// <summary>
		/// Proporciona un error por defecto con la información del response
		/// cuando la deserialización no fue exitosa
		/// </summary>
		/// <param name="response">Respuesta del servidor</param>
		/// <returns>Error del servidor con valores por defecto</returns>
		protected virtual TResultadoRest WhenCantDeserializeError(IRestResponse response)
		{
			var error = Activator.CreateInstance<TResultadoRest>();
			
			error.StatusCode = response.StatusCode;
			error.Message = response.ErrorMessage;
			error.Details = response.Content;
			
			return error;
		}
	}
}

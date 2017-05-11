using Movistar.Online.Common.Types.Resultados;
using Movistar.Online.Common.Types.ResultadosRest;
using UruIT.RESTClient.Interfaces.Proveedores;
using System;

namespace UruIT.RESTClient.Clases.Proveedores
{
	/// <summary>
	/// Crea una TRestException que contiene la información del error
	/// </summary>
	/// <typeparam name="TResultado">Tipo del error de negocio</typeparam>
	/// <typeparam name="TResultadoRest">Tipo del error de la respuesta</typeparam>
	/// <typeparam name="TRestException">Tipo de la excepcion</typeparam>
	public class ExceptionResultadoProvider<TResultado, TResultadoRest, TRestException>
		: IExceptionProvider<TResultado, TRestException>
		where TResultado : RestBusinessError<TResultado, TResultadoRest>
		where TResultadoRest : RestHttpError<TResultado, TResultadoRest>
		where TRestException : RestException<TResultado, TResultadoRest>
	{
		/// <summary>
		/// Crea una TRestException que contiene el TResultadoRest, generado del TResultado
		/// </summary>
		/// <param name="error">Error de negocio</param>
		/// <returns>Excepción con la información necesaria</returns>
		public TRestException ProvideException(TResultado error)
		{
			var ex = Activator.CreateInstance<TRestException>();
			ex.HttpError = error.ToHttpError();
			return ex;
		}
	}
}

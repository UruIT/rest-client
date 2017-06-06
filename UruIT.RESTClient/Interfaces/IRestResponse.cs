using System;
using System.Net;

namespace UruIT.RESTClient
{
	/// <summary>
	/// Resultado de una llamada REST
	/// </summary>
	public interface IRestResponse
	{
		/// <summary>
		/// Representación en string del contenido del body
		/// </summary>
		string Content { get; set; }

		/// <summary>
		/// Content type de la respuesta
		/// </summary>
		string ContentType { get; set; }

		/// <summary>
		/// Código HTTP de la respuesta
		/// </summary>
		HttpStatusCode StatusCode { get; set; }

		/// <summary>
		/// Mensaje de error si falla el request
		/// </summary>
		string ErrorMessage { get; set; }

		/// <summary>
		/// Excepción de falla del request
		/// </summary>
		Exception ErrorException { get; set; }
	}
}
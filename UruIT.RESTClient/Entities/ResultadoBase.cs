using System;

namespace UruIT.RESTClient
{
	[Serializable]
	public class ResultadoBase<TResultado>
	{
		public ResultadoBase()
		{
		}

		public ResultadoBase(TResultado resultado, Exception ex)
		{
			Resultado = resultado;
			if (ex != null)
			{
				Mensaje = ex.Message;
				Detalle = ex.ToString();
			}
		}

		public ResultadoBase(TResultado resultado, string mensaje, string detalle)
		{
			Resultado = resultado;
			Mensaje = mensaje;
			Detalle = detalle;
		}

		public ResultadoBase(ResultadoBase<TResultado> resultado)
		{
			Resultado = resultado.Resultado;
			Mensaje = resultado.Mensaje;
			Detalle = resultado.Detalle;
		}

		public TResultado Resultado { get; set; }

		public string Mensaje { get; set; }

		public string Detalle { get; set; }

		public override string ToString()
		{
			return string.Format("{{ Resultado:{0}, Mensaje:\"{1}\", Detalle:\"{2}\"",
				Resultado, Mensaje, Detalle);
		}
	}
}
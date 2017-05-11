using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace UruIT.RESTClient
{
    [Serializable]
    public class ResultadoException<TResultado, TResultadoRest> : Exception
        where TResultado : RestBusinessError<TResultado, TResultadoRest>
        where TResultadoRest : ResultadosRest.ResultadoRest<TResultado, TResultadoRest>
    {
        public TResultado Resultado { get; set; }

        public ResultadoException(TResultado resultado)
            : base(resultado.Mensaje)
        {
            this.Resultado = resultado;
        }

        public ResultadoException()
            : base()
        {
        }

        public ResultadoException(string message)
            : base(message)
        {
        }

        public ResultadoException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected ResultadoException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Resultado = Activator.CreateInstance<TResultado>();
            Resultado.Detalle = info.GetString("Resultado.Detalle");
            Resultado.Mensaje = info.GetString("Resultado.Mensaje");
            Resultado.Resultado = (RestErrorType)info.GetInt32("Resultado.Resultado");
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("Resultado.Detalle", Resultado.Detalle);
            info.AddValue("Resultado.Mensaje", Resultado.Mensaje);
            info.AddValue("Resultado.Resultado", (int)Resultado.Resultado);
        }

        public override string Message
        {
            get
            {
                return Resultado != null ? Resultado.Mensaje : base.Message;
            }
        }

        public override string ToString()
        {
            return string.Format("{{Resultado: {{'Resultado': '{1}', 'Mensaje': '{2}', 'Detalle': '{3}'}}\n, Detalle: {0}.\n}}",
                base.ToString(), Resultado.Resultado, Resultado.Mensaje, Resultado.Detalle);
        }
    }

    [Serializable]
    public class ResultadoException : ResultadoException<RestBusinessError, ResultadosRest.ResultadoRest>
    {
        public ResultadoException()
            : base()
        {
        }

        public ResultadoException(string message)
            : base(message)
        {
        }

        public ResultadoException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public ResultadoException(RestBusinessError resultado)
            : base(resultado)
        {
        }

        protected ResultadoException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
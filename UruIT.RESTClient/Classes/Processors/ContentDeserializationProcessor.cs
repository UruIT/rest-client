using Monad;
using UruIT.Serialization.Core;
using UruIT.RESTClient.Interfaces;
using System.Runtime.Serialization;

namespace UruIT.RESTClient.Clases.Procesadores
{
	/// <summary>
	/// Procesador que intenta deserializar el contenido
	/// Trata de deserializar el body de la respuesta HTTP a un objecto C#. Si falla la deserialización retorna Nothing.
	/// </summary>
	/// <typeparam name="TResult">Tipo resultante de la deserialización</typeparam>
	/// <typeparam name="TSerializer">Tipo del serializador</typeparam>
	public class TryContentDeserializationProcessor<TResult, TSerializer> : ISimpleProcessorNode<OptionStrict<TResult>, TSerializer>
		where TSerializer : ISerializer
	{
		public bool CanProcess(IRestResponse response)
		{
			return true;
		}

        public OptionStrict<TResult> Process(IRestResponse response, TSerializer serializer)
		{
			return serializer.TryDeserialize<TResult>(response.Content);
		}
	}

	/// <summary>
	/// Procesador que usa TryContentDeserializationProcessor y lanza una excepción si no se pudo deserializar
	/// </summary>
	/// <typeparam name="TResult">Tipo resultante de la deserialización</typeparam>
	/// <typeparam name="TSerializer">Tipo del serializador</typeparam>
	public class ContentDeserializationProcessor<TResult, TSerializer> : ISimpleProcessorNode<TResult, TSerializer>
		where TSerializer : ISerializer
	{
        private readonly IResponseProcessor<OptionStrict<TResult>, TSerializer> innerProcessor;

		public ContentDeserializationProcessor()
		{
			innerProcessor = new TryContentDeserializationProcessor<TResult, TSerializer>();
		}

		public bool CanProcess(IRestResponse response)
		{
			return innerProcessor.CanProcess(response);
		}

		public TResult Process(IRestResponse response, TSerializer serializer)
		{
            var res = innerProcessor.Process(response, serializer);
            if (!res.HasValue){
                var message = string.Format("Error deserializando '{0}' en el tipo {1}.",
						response.Content, typeof(TResult).FullName);
					throw new SerializationException(message);
            }
            return res.Value;
		}
	}

	/// <summary>
	/// ContentDeserializationProcessor con serialiador JSON
	/// </summary>
	/// <typeparam name="TResult">Tipo resultado de la deserialización</typeparam>
	public class ContentDeserializationProcessor<TResult> : ContentDeserializationProcessor<TResult, IJsonConverter>
	{
	}
}

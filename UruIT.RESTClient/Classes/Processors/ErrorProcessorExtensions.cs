using Monad;
using UruIT.Serialization.Core;

namespace UruIT.RESTClient.Clases.Procesadores
{
	/// <summary>
	/// Extensiones de ErrorProcessor para crear los encadenamientos usados por defecto
	/// </summary>
	public static class ErrorProcessorExtensions
	{
		/// <summary>
		/// Configura un errorProcessor con los procesadores necesarios para intentar deserializar
		/// la respuesta en TErrorRest en caso de error
		/// </summary>
		/// <typeparam name="TErrorRest">Tipo esperado ante error en la respuesta</typeparam>
		/// <typeparam name="TSerializer">Tipo del serializador</typeparam>
		/// <param name="errorProcessor">ErrorProcessor a configurar</param>
		/// <returns>El mismo errorProcessor ya configurado</returns>
        public static ErrorProcessor<OptionStrict<TErrorRest>, TSerializer> Default<TErrorRest, TSerializer>(
            this ErrorProcessor<OptionStrict<TErrorRest>, TSerializer> errorProcessor)
			where TSerializer : ISerializer
		{
			var tryContentDeserializationProcessor = new TryContentDeserializationProcessor<TErrorRest, TSerializer>();
			errorProcessor.AddProcessors(tryContentDeserializationProcessor);
			return errorProcessor;
		}
	}
}

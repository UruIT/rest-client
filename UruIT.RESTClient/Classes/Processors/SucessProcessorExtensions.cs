using UruIT.Serialization.Core;

namespace UruIT.RESTClient.Clases.Procesadores
{
	/// <summary>
	/// Extensiones de SuccessProcessor para crear los encadenamientos usados por defecto
	/// </summary>
	public static class SuccessProcessorExtensions
	{
		/// <summary>
		/// Configura un successProcessor con los procesadores necesarios para deserializar
		/// la respuesta en TResult en caso de éxito
		/// </summary>
		/// <typeparam name="TResult">Tipo esperado ante éxito en la respuesta</typeparam>
		/// <typeparam name="TSerializer">Tipo del serializador</typeparam>
		/// <param name="successProcessor">SuccessProcessor a configurar</param>
		/// <returns>El mismo successProcessor ya configurado</returns>
		public static SuccessProcessor<TResult, TSerializer> Default<TResult, TSerializer>(
			this SuccessProcessor<TResult, TSerializer> successProcessor)
			where TSerializer : ISerializer
		{
			var contentDeserializationProcessor = new ContentDeserializationProcessor<TResult, TSerializer>();
			successProcessor.AddProcessors(contentDeserializationProcessor);

			return successProcessor;
		}
	}
}

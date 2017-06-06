using Newtonsoft.Json;

namespace UruIT.Serialization
{
	/// <summary>
	/// JSON serializer
	/// </summary>
	public interface IJsonSerializer : ISerializer
	{
		/// <summary>
		/// Serialization settings
		/// </summary>
        JsonSerializerSettings Settings { get; }

		/// <summary>
		/// Agregar un convertidor de json.
		/// </summary>
		/// <param name="converter">Convertidor de json</param>
        IJsonSerializer AddConverter(JsonConverter converter);
	}
}
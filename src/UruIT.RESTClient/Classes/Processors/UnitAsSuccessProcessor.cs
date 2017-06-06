using Monad.Utility;
using System.Net;
using UruIT.Serialization;

namespace UruIT.RESTClient.Processors
{
    /// <summary>
    /// Processor that maps 2xx HTTP codes to unit.
    /// </summary>
    /// <typeparam name="TSerializer">Type of the serializer</typeparam>
    public class UnitAsSuccessProcessor<TSerializer> : ISimpleProcessorNode<Unit, TSerializer>
        where TSerializer : ISerializer
    {
        public bool CanProcess(IRestResponse response)
        {
            return response.StatusCode.IsSuccessful();
        }

        public Unit Process(IRestResponse response, TSerializer serializer)
        {
            return Unit.Default;
        }
    }

    /// <summary>
    /// Unit processor with JSON serializer.
    /// </summary>
    public class UnitAsSuccessProcessor : UnitAsSuccessProcessor<IJsonSerializer>
    {
    }
}
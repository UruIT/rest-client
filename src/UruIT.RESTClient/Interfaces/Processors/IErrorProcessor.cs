using UruIT.Serialization;

namespace UruIT.RESTClient
{
    /// <summary>
    /// Processor in charge of serializing errors.
    /// </summary>
    public interface IErrorProcessor<TSerializer>
        where TSerializer : ISerializer
    {
        TSerializer ErrorSerializer { set; }
    }
}
using UruIT.Serialization;

namespace UruIT.RESTClient
{
    /// <summary>
    /// REST Client whos body is in XML
    /// </summary>
    public interface IXmlRestClient : IRestClient<IXmlSerializer>
    {
    }
}
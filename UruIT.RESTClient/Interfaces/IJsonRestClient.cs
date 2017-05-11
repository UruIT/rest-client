using UruIT.Serialization;

namespace UruIT.RESTClient
{
    /// <summary>
    /// REST client whose body is in JSON
    /// </summary>
    public interface IJsonRestClient : IRestClient<IJsonSerializer>
    {
    }
}
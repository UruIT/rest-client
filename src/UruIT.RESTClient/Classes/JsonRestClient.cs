using Newtonsoft.Json;
using UruIT.Serialization;

namespace UruIT.RESTClient
{
    /// <summary>
    /// Rest client with a JSON serializer
    /// </summary>
    public class JsonRestClient : RestClient<IJsonSerializer>, IJsonRestClient
    {
        public JsonRestClient(IRestClientExecuter restClientExecuter)
            : base(restClientExecuter)
        {
        }

        protected override IJsonSerializer CreateSuccessSerializer()
        {
            return new Serialization.JsonSerializer();
        }

        protected override IJsonSerializer CreateErrorSerializer()
        {
            var serializer = new Serialization.JsonSerializer();

            serializer.Settings.ContractResolver.ObjectContract = new RequiredAttributesObjectContract(RequiredLevel.AllowNull);
            serializer.Settings.MissingMemberHandling = MissingMemberHandling.Error;

            return serializer;
        }
    }
}
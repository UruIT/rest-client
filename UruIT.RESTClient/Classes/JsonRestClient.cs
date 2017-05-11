using UruIT.Serialization.Core;
using UruIT.Serialization.Core.ContractResolvers;
using Movistar.Online.Common.Types.Resultados;
using Movistar.Online.Common.Types.ResultadosRest;
using UruIT.RESTClient.Clases.Procesadores;
using UruIT.RESTClient.Interfaces;
using Newtonsoft.Json;

namespace UruIT.RESTClient.Clases
{
	public class JsonRestClient : RestClient<IJsonConverter>, IJsonRestClient
	{
		public JsonRestClient(IRestClientExecuter restClientExecuter)
			: base(restClientExecuter)
		{
		}

		protected override IJsonConverter CreateSuccessSerializer()
		{
			return new JsonMONConverter();
		}

		protected override IJsonConverter CreateErrorSerializer()
		{
			var serializer = new JsonMONConverter();

			serializer.Settings.ContractResolver.ObjectContract = new RequiredAttributesObjectContract(RequiredLevel.AllowNull);
			serializer.Settings.MissingMemberHandling = MissingMemberHandling.Error;

			return serializer;
		}
	}
}

using Monad;
using Monad.Utility;
using System.Globalization;
using UruIT.RESTClient.Processors;

namespace UruIT.RESTClient.Sample.Console
{
    public class HttpClient
    {
        private readonly IJsonRestClient restClient;
        private static readonly string Host = "http://localhost:13788";

        public HttpClient(IJsonRestClient restClient)
        {
            this.restClient = restClient;
        }

        public EitherStrict<RestBusinessError, OptionStrict<OperationResult>> WithNotFoundAndError(string argument)
        {
            return restClient
                .Get<EitherStrict<RestBusinessError, OptionStrict<OperationResult>>>(Host, string.Format(CultureInfo.InvariantCulture, "/Sample/WithNotFoundAndError/{0}", argument))
                .AddProcessors(new EitherRestErrorProcessor<OptionStrict<OperationResult>>().Default()
                    .AddProcessors(new OptionAsNotFoundProcessor<OperationResult>()))
                .GetResult();
        }

        public EitherStrict<RestBusinessError, Unit> NoContentWithError(string argument)
        {
            return restClient
                .Post<EitherStrict<RestBusinessError, Unit>>(Host, string.Format(CultureInfo.InvariantCulture, "/Sample/NoContentWithError/{0}", argument), Unit.Default)
                .AddProcessors(new EitherRestErrorProcessor<Unit>().Default()
                    .AddProcessors(new UnitAsSuccessProcessor()))
                .GetResult();
        }
    }
}
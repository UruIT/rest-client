using Monad.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace UruIT.RESTClient.Sample.Console
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var options = new CommandOptions();
            var isValid = CommandLine.Parser.Default.ParseArgumentsStrict(args, options);

            if (isValid)
            {
                try
                {
                    var client = new HttpClient(new JsonRestClient(new RestSharpRestClientExecuter()));

                    System.Console.WriteLine(options.Operation);

                    new Dictionary<string, Func<Unit>>
                    {
                        {
                            "WithNotFoundAndError", () =>
                            {
                                System.Console.WriteLine(" > Calling 'WithNotFoundAndError'");

                                var resEither = client.WithNotFoundAndError(options.Argument);

                                resEither.Match(
                                    Left: err =>
                                    {
                                        System.Console.WriteLine(string.Format(CultureInfo.InvariantCulture, " > Returned error: Type: {0}, Message: {1}, Detail: {2} ", err.ErrorType.ToString(), err.Message, err.Details));
                                    },
                                    Right: resOpt =>
                                    {
                                        resOpt.Match<Unit>(
                                            Nothing: () =>
                                            {
                                                System.Console.WriteLine(" > Returned NotFound ");

                                                return Unit.Default;
                                            },
                                            Just: val =>
                                            {
                                                System.Console.WriteLine(string.Format(CultureInfo.InvariantCulture, " > Returned Value: Value1: {0}, Value2: {1}", val.Value1, val.Value2.ToString(CultureInfo.InvariantCulture)));

                                                return Unit.Default;
                                            });
                                    });

                                return Unit.Default;
                            }
                        },
                        {
                            "NoContentWithError", () =>
                            {
                                System.Console.WriteLine(" > Calling 'NoContentWithError'");

                                var resEither = client.NoContentWithError(options.Argument);

                                resEither.Match(
                                    Left: err =>
                                    {
                                        System.Console.WriteLine(string.Format(CultureInfo.InvariantCulture, " > Returned error: Type: {0}, Message: {1}, Detail: {2} ", err.ErrorType.ToString(), err.Message, err.Details));
                                    },
                                    Right: _ =>
                                    {
                                        System.Console.WriteLine(" > Returned NoContent ");
                                    });

                                return Unit.Default;
                            }
                        },
                    }[options.Operation]();
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine(" -- An error ocurred: --");
                    System.Console.WriteLine(ex.Message);
                    System.Console.WriteLine(ex.StackTrace);
                }
            }
            else
            {
                System.Console.WriteLine(" -- Invalid Arguments --");
                System.Console.WriteLine(" Use -h for help");
            }
        }
    }
}
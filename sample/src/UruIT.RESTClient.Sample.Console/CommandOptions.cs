using CommandLine;

namespace UruIT.RESTClient.Sample.Console
{
    public class CommandOptions
    {
        [Option('o', "operation", Required = true, HelpText = "Operation to call. 'WithNotFoundAndError' or 'NoContentWithError' ")]
        public string Operation { get; set; }

        [Option('a', "argument", Required = true, HelpText = "Argument to pass to operation. 'Ok', 'InternalErrorWithResult', 'BadRequestWithResult', 'NotFound' or 'NoContent")]
        public string Argument { get; set; }
    }
}
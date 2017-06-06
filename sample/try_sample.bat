REC Runs the CMD client to test calls to the sample server using RESTClient

src\UruIT.RESTClient.Sample.Console\bin\Debug\UruIT.RESTClient.Sample.Console.exe -o WithNotFoundAndError -a Ok
src\UruIT.RESTClient.Sample.Console\bin\Debug\UruIT.RESTClient.Sample.Console.exe -o WithNotFoundAndError -a InternalErrorWithResult
src\UruIT.RESTClient.Sample.Console\bin\Debug\UruIT.RESTClient.Sample.Console.exe -o WithNotFoundAndError -a BadRequestWithResult
src\UruIT.RESTClient.Sample.Console\bin\Debug\UruIT.RESTClient.Sample.Console.exe -o WithNotFoundAndError -a NotFound
src\UruIT.RESTClient.Sample.Console\bin\Debug\UruIT.RESTClient.Sample.Console.exe -o NoContentWithError -a NoContent
src\UruIT.RESTClient.Sample.Console\bin\Debug\UruIT.RESTClient.Sample.Console.exe -o NoContentWithError -a InternalErrorWithResult
src\UruIT.RESTClient.Sample.Console\bin\Debug\UruIT.RESTClient.Sample.Console.exe -o NoContentWithError -a BadRequestWithResult

pause
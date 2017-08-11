# rest-client

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](https://opensource.org/licenses/MIT)
[![GitHub stars](https://img.shields.io/github/stars/UruIT/rest-client.svg)](https://github.com/UruIT/rest-client/stargazers)


__Rest Client__ is a C# library that allows you to consume an _HTTP REST service_, mapping _HTTP_ response codes to different C# types allowing you full control over what you expect when calling such service.

## Description

This library uses a fluent API that is separated in three parts:

* Specifies the HTTP method, host, resource and optionally the body (passing in a C# object).
* Sets up a pipeline of processors that process the response in sequence, building the return type incrementally and compositionally.
* (Optional) Configures settings of the JSON serializer, both for the request body and the response body.
* (Optional) Configures other aspects of the HTTP request like headers, TLS certificates, etc.

### Usage

An example is the following:

> ![restclient example](docs/img/restclient_example_1.png?raw=true)

In this case, we want to call the API *[http://Host/Sample/WithNotFoundAndError/](# "Exampple API Url")* using a variable argument. 
Based on the returning HTTP response, we want to get a different value.

* If the api returns a `2XX` *Success* code, we want to deserialize the JSON body into a `OperationResult` type.
* If the api returns a `401` *Not Found* code, we want to return a null type, indicating that the resource doesn't exist.
* If the api returns a `5XX` or `4XX` status code, we want to return a value that indicates an error has occurred (without throwing an exception).

To achieve this, we use the [Csharp-Monad](https://github.com/louthy/csharp-monad) library, using its `EitherStrict` and `OptionStrict` types (check out the [library](https://github.com/louthy/csharp-monad "CSharp Monad") for more information about those types). 

We want to map `401` codes to `OptionStrict.Nothing` values, and we want to map 5XX and 4XX codes to `EitherStrict.Left` values. 
If we get a 200 success code, we map it to a `EitherStrict.Right` and `OptionStrict.Just`, returning a value of type `OperationResult`.

Basically, the mapping is done this way:

* If the HTTP response has a `4XX` or `5XX` status code we return `EitherStrict.Left(error)`
    * `error` indicates information about the error (it has the error message, whether the error was a `4XX` or a `5XX` one, etc).
* If the HTTP response has a `401` status code, we return `EitherStrict.Right(OptionStrict.Nothing)`
* If the HTTP response has a `2XX` status code, we return `EitherStrict.Right(OptionStrict.Just(operationResult))`
    * where `operationResult` is the data typed obtained by deserializing the JSON response body.

No matter what the API responds with, we return the user a type specifying the information he wants from the API, mapping the HTTP response to a C# type (that is composed of various types like `EitherStrict`, `OptionStrict`, etc).

### Processors

To achieve this, `rest-client` uses a pipeline of processors that check whether they find a specific HTTP status code, and if they find it they return a value of the returning type.

* `EitherRestErrorProcessor` returns values of type `EitherStrict<RestBusinessError, TResult>`. 
    * It checks if the response has an unsuccessful status code (one different than `2XX`). If it does, then it returns a `RestBusinessError`; if it doesn't, then it delegates the processing to the rest of the pipeline so it returns a `TResult` value.
* Next comes `OptionAsNotFoundProcessor`, which returns values of type `OptionStrict<TResult>`. 
    * It checks if the response has a `401` status code. If it does, then it returns a `OptionStrict.Nothing`. If it doesn't, then it delegates the processing to the rest of the pipeline so it returns a `TResult` value.
* Next there is an implicit processor that returns values of type `TResult`. 
    * It checks if the response has a *2XX* status code, and if it does it deserializes its JSON body into the expecting `TResult` type

You can compose the three processors with calls to `AddProcessors`, and in each case the type `TResult` is substituted with the resulting type of the processor next in the pipeline. 
This way, you can mix and match processors until you get the type you want, all you need to do is call `AddProcessors` with the corresponding processor.

## Sample

This library includes a sample application. 
You can use it to view the usage of this library, as well as for making tests when developing or making changes to this library.

## Authors

[<img alt="carloluis" src="https://avatars3.githubusercontent.com/u/6629501?v=4&s=400" width="117">](https://github.com/gonzaw) |
:---: |
[gonzaw](https://github.com/gonzaw) |


## License

Licensed under the MIT License, Copyright © 2017 UruIT.

See [LICENSE](./LICENSE.txt) for more information.

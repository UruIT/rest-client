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

## Basic Usage

We'll describe some sample usages of this library, from simple cases to more complex ones

Our first example is the following:

```csharp
public class Cat
{
    public string Name { get; set; }

    public int Age { get; set; }

    public int Lives { get; set; }
}					

public Cat SearchCatByName(string catName)
{
    var host = "http://api.catsforall.com";
    var resource = string.Format(CultureInfo.InvariantCulture, "/cats/{0}", catName);
    return restClient
        .Get<Cat>(host, resource)            
        .GetResult();
}
```

In this case, we cant to call the API "GET *[http://api.catsforall.com/cats/myCatName](# "Example API Get Url")*", allowing us to search cats by name. This API returns JSON data about the cat, for example `{ "Name": "Whiskers", "Age": 5, "Lives": 9 }`, which we want to represent as the `Cat` C# type.

The API is simple, it either returns a `200` status Code with the JSON body, or it returns a `4xx` or `5xx` status code, in which case the `rest-client` library throws an exception.

Now, what if we want to add a new cat by calling "POST *[http://api.catsforall.com/cats](# "Example API Post Url")*"? We do so like this:

```csharp
public void AddCat(Cat cat)
{
    var host = "http://api.catsforall.com";
    restClient.Post(host, "/cats", cat);
}
```

We can also do the usual PUT and DELETE calls:

```csharp
public void UpdateCat(Cat cat)
{
    var host = "http://api.catsforall.com";
    var resource = string.Format(CultureInfo.InvariantCulture, "/cats/{0}", cat.Name);
    restClient.Put(host, resource, cat);
}
```


```csharp
public void DeleteCat(string catName)
{
    var host = "http://api.catsforall.com";
    var resource = string.Format(CultureInfo.InvariantCulture, "/cats/{0}", catName);
    restClient.Delete(host, resource);
}
```

In the case of `DeleteCat`, we also need to pass `Unit.Default` to indicate that we don't want anything passed in the request body. If we needed to call DELETE passing a JSON object in the request body, we'd replace `Unit.Default` with the object to serialize.

## Managing 404 status codes

Since our API is RESTful, whenever the client requests a resource that doesn't exist the API returns a `404` *Not Found* status code. However, what if we want to handle that case programatically? We can do that in the following way:

```csharp
public OptionStrict<Cat> SearchCatByName(string catName)
{
    var host = "http://api.catsforall.com";
    var resource = string.Format(CultureInfo.InvariantCulture, "/cats/{0}", catName);
    return restClient
        .Get<OptionStrict<Cat>>(host, resource)
        .AddProcessors(new OptionAsNotFoundProcessor<Cat>())
        .GetResult();
}
```

The only thing that changed from the original method is the return type, which changes from `Cat` to `OptionStrict<Cat>`, and we added a line `AddProcessors(new OptionAsNotFoundProcessor<Cat>())`. 

Let's talk about the change in the returning type first. `OptionStrict<T>` is a type from the [Csharp-Monad](https://github.com/louthy/csharp-monad) library, it represents a nullable type (which can be either a value type or reference type). A value with type `OptionStrict<T>` is either empty or has a value. We can use this type to represent our new method: If the API call returns a `200` status code with the JSON body, then we return a `Cat` value; however, if the API call returns a `404` status code, we return an empty value (representd by `Option<Cat>.Nothing`).

To make this possible, we need to introduce processors, as we did with `AddProcessors(new OptionAsNotFoundProcessor<Cat>())`

## Processors

`rest-client` uses a pipeline of processors that check whether they find a specific HTTP status code, and if they find it they return a value of the returning type.

In the above case, `OptionAsNotFoundProcessor<TResult>` is a processor that returns values of type `OptionStrict<TResult>`. It checks if a response has `404` status code; if it does it returns `OptionStrict<TResult>.Nothing`; if it doesn't, then it delegates the processing to the rest of the pipeline, which will return a `TResult` value.

In every call there is an implicit processor that returns values of type `TResult`. It checks if the response has a `2xx` status code, and if it does it deserializes its JSON body into the expected `TResult` type. This processor is the one that is used behind the scenes in all the previous _GET_ examples we've seen.

Without adding a processor like `OptionAsNotFoundProcessor<TResult>` our library would find that the HTTP response has a `404` status code, and it won't know what to do with it. Thus, processors are a way to tell the library how to handle status code and determine what C# type it must return depending on the status code.

## Managing 4xx / 5xx status codes

With `rest-client` we can handle an even more complex subject, like managing HTTP errors with C# types. Until know, every `4xx` and `5xx` error was not known to our program, i.e it was not represented in the return type. Instead, if the API returned such status codes, our program thre and exception and then forgot about it. There exist cases where we want our own program to handle such errors programatically, allowing such errors to alter the control flow of our program. Using `try-catch` blocks would not be appropriate in that case, since try-catch is not the best approach for control flow.

We can handle such errors by having a return type that determines if there was an error or not; if there was an error it should return specific data about the error (like an error message); if there was no error then it should return the data type we want.

We use `EitherStrict<RestBusinessError, TResult>` in such case. `EitherStrict<TLeft, TRight>` is a type from the [Csharp-Monad](https://github.com/louthy/csharp-monad) library; it represents an union type, where it either has a `TLeft` value or a `TRight` value, and you can check which one is which (for example by the `bool IsLeft` and `bool IsRight` properties). In rest-client, we determine `TLeft` to be the error case, and we represent it with the `RestBusinessError` type, which is the following:

```csharp
public enum RestErrorType
{
    ValidationError,
    InternalError
}

public class RestBusinessError
{
    public RestErrorType ErrorType { get; set; }

    public string Message { get; set; }

    public string Details { get; set; }
}
```

`RestErrorType` is `ValidationError` when the API returned a `4xx` status code, and `InternalError` when the API returned a `5xx` status code. `Message` and `Details` provide additional information about the HTTP error.

With such types, we can then update our previous API call with the following:

```csharp
public EitherStrict<RestBusinessError, OptionStrict<Cat>> SearchCatByName(string catName)
{
    var host = "http://api.catsforall.com";
    var resource = string.Format(CultureInfo.InvariantCulture, "/cats/{0}", catName);
    return restClient
        .Get<EitherStrict<RestBusinessError, OptionStrict<Cat>>>(host, resource)
        .AddProcessors(new EitherRestErrorProcessor<OptionStrict<Cat>>().Default()
            .AddProcessors(new OptionAsNotFoundProcessor<Cat>()))
        .GetResult();

}
```

As expected, we changed the return type from `OptionStrict<Cat>` to `EitherStrict<RestBusinessError, OptionStrict<Cat>>`. 

We also added a new processor, called `EitherRestErrorProcessor`. Just like with `OptionStrict`, we need to tell our library how to handle the error status codes and tell him that he should return a `RestBusinessError` value if he encounters such status codes. This is done in the following way:

* `EitherRestErrorProcessor` returns values of type `EitherStrict<RestBusinessError, TResult>`. It checks if the response has an unsuccessful status code (one different than `2xx`). If it does, then it returns a `RestBusinessError`; if it doesn't, then it delegates the processing to the rest of the pipeline so it returns a `TResult` value.
* Next comes `OptionAsNotFoundProcessor`, which returns values of type `OptionStrict<TResult>`. It checks if the response has a `401` status code. If it does, then it returns a `OptionStrict.Nothing`; if it doesn't, then it delegates the processing to the rest of the pipeline so it returns a `TResult` value.
* Next, the implicit processor that returns values of type `TResult` checks if the response has a `2XX` status code. If it does then it deserializes its JSON body into the expecting `TResult` type

You can compose the three processors with calls to `AddProcessors`, and in each case the type `TResult` is substituted with the resulting type of the processor next in the pipeline. This way, you can mix and match processors until you get the type you want, all you need to do is call `AddProcessors` with the corresponding processor.

## Sample

This library includes a sample application. 
You can use it to view the usage of this library, as well as for making tests when developing or making changes to this library.

## Authors

[<img alt="gonzaw" src="https://avatars3.githubusercontent.com/u/6629501?v=4&s=400" width="117">](https://github.com/gonzaw) |
:---: |
[gonzaw](https://github.com/gonzaw) |


## License

Licensed under the MIT License, Copyright © 2017 UruIT.

See [LICENSE](./LICENSE.txt) for more information.

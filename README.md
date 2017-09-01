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

We'll describe some sample usages of this library, from simple cases to more complex ones.

In our example, we have an API for managing information about cats. First of all we cant to call the API "GET *[http://api.catsforall.com/cats/myCatName](# "Example API Get Url")*", allowing us to search cats by name. This API returns JSON data about the cat, for example `{ "Name": "Whiskers", "Age": 5, "Lives": 9 }`, which we want to represent as the `Cat` C# type.

```csharp
public class Cat
{
    public string Name { get; set; }

    public int Age { get; set; }

    public int Lives { get; set; }
}	
```

The API is simple, it either returns a `200` status Code with the JSON body, or it returns a `4xx` or `5xx` status code.

Using `rest-client` we can call this API in the following way:

```csharp
public Cat SearchCatByName(string catName)
{
    return restClient
        .Get<Cat>("http://api.catsforall.com", "/cats/" + catName)            
        .GetResult();
}
```

Now, what if we want to add a new cat by calling "POST *[http://api.catsforall.com/cats](# "Example API Post Url")*"? We do so like this:

```csharp
public void AddCat(Cat cat)
{
    restClient.PostDefault("http://api.catsforall.com", "/cats", cat);
}
```

We can also do the usual PUT and DELETE calls:

```csharp
public void UpdateCat(Cat cat)
{
    restClient.PutDefault("http://api.catsforall.com", "/cats/" + cat.Name, cat);
}
```

```csharp
public void DeleteCat(string catName)
{
    restClient.DeleteDefault("http://api.catsforall.com", "/cats/" + catName);
}
```

## Managing 404 status codes

Since our API is RESTful, whenever the client requests a resource that doesn't exist the API returns a `404` *Not Found* status code. However, what if we want to handle that case programatically? With our previous `SearchCatByName` function, `rest-client` throws a `RestException` whenever it encounters a `4xx` or `5xx` status code, including `404`. An example of handling the `404` case would be the following:

```csharp
public void ShowCatLives(string catName)
{
    string messageToShow = string.Empty;
    try
    {
        var cat = SearchCatByName(catName);
        messageToShow = catName + " has " + cat.Lives + " lives";
    }
    catch (RestException ex)
    {
        if (ex.HttpError.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            messageToShow = "Cat doesn't exist";
        }
        else
        {
            throw;
        }
    }

    lblCatLives.Text = messageToShow;
}
```

We can improve this way of handling `404` cases in the following way:

```csharp
public OptionStrict<Cat> SearchCatByName(string catName)
{
    return restClient
        .Get<OptionStrict<Cat>>("http://api.catsforall.com", "/cats/" + catName)
        .AddProcessors(new OptionAsNotFoundProcessor<Cat>())
        .GetResult();
}
```

The only thing that changed from the original method is the return type, which changes from `Cat` to `OptionStrict<Cat>`, and we added a line `AddProcessors(new OptionAsNotFoundProcessor<Cat>())`. 

Let's talk about the change in the returning type first. `OptionStrict<T>` is a type from the [Csharp-Monad](https://github.com/louthy/csharp-monad) library, it represents a nullable type (which can be either a value type or reference type). A value with type `OptionStrict<T>` is either empty or has a value. We can use this type to represent our new method: If the API call returns a `200` status code with the JSON body, then we return a `Cat` value; however, if the API call returns a `404` status code, we return an empty value (represented by `Option<Cat>.Nothing`).

With this new design, our previous example can be simplified:

```csharp
public void ShowCatLives(string catName)
{
    var catOpt = SearchCatByName(catName);
    var messageToShow = catOpt.Match(
        Nothing: () => "Cat doesn't exist",
        Just: cat => catName + " has " + cat.Lives + " lives");

    lblCatLives.Text = messageToShow;
}
```

The `Match` function pattern matches on the state of the optional. Its implementation is equivalent to this:

```csharp
public TResult Match<TResult, TValue>(OptionStrict<TValue> opt, Func<TValue, TResult> Just, Func<TResult> Nothing)
{
    return opt.HasValue ? Just(opt.Value) : Nothing();
} 
```

To make this implementation possible, we need to introduce processors, as we did with `AddProcessors(new OptionAsNotFoundProcessor<Cat>())`

## Processors

`rest-client` uses a pipeline of processors that check whether they find a specific HTTP status code, and if they find it they return a value of the returning type.

The processor pipeline is the following:

![Processor Pipeline](docs/img/processors_option_as_not_found.png?raw=true)

Each processor can take an action with the HTTP response, or pass the response to the next processor in the pipeline. This way we can compose processors to add functionality to our return types based on the HTTP status code we want to handle.

These processors can also incrementally build types using values obtained from the rest of the pipeline. In the above case, `OptionAsNotFoundProcessor` returns `OptionStrict<TResult>`, but uses the `TResult` value returned by `SuccessProcessor`. We could add new processors on top of `OptionAsNotFoundProcessor` to extend the returned C# type if we wanted (as we'll see in the next section when handling `4xx` and `5xx` status codes).

## Managing 4xx / 5xx status codes

Like it was stated previously, whenever a `4xx` or `5xx` status code arrives, `rest-client` throws an exception. To handle such cases you need to catch a `RestException`, like this:

```csharp
public void ShowCatLives(string catName)
{
    string messageToShow = string.Empty;
    try
    {
        var catOpt = SearchCatByName(catName);
        messageToShow =  catOpt.Match(
            Nothing: () => "Cat doesn't exist",
            Just: cat => catName + " has " + cat.Lives + " lives");
    }
    catch (RestException ex)
    {
        messageToShow = "Error with status code " + ex.HttpError.StatusCode + " and message " + ex.Message;
    }

    lblCatLives.Text = messageToShow;
}
```
However, with `rest-client` we can handle HTTP errors with C# types without forcing the user to use `try-catch` blocks for managing control flow.

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
    return restClient
        .Get<EitherStrict<RestBusinessError, OptionStrict<Cat>>>("http://api.catsforall.com", "/cats/" + catName)
        .AddProcessors(new EitherRestErrorProcessor<OptionStrict<Cat>>().Default()
            .AddProcessors(new OptionAsNotFoundProcessor<Cat>()))
        .GetResult();
}
```

As expected, we changed the return type from `OptionStrict<Cat>` to `EitherStrict<RestBusinessError, OptionStrict<Cat>>`. 

We also added a new processor, called `EitherRestErrorProcessor`. Just like with `OptionStrict`, we need to tell our library how to handle the error status codes and tell him that he should return a `RestBusinessError` value if he encounters such status codes. 

`EitherRestErrorProcessor` is added at the top of the pipeline, and checks if the response has a `4xx` or `5xx` status code. If it does then it returns `RestBusinessError`, if not it delegates the processing to the rest of the pipeline (as seen in the previous section).

We can rewrite our previous method using this new API call:

```csharp
public void ShowCatLives(string catName)
{
    var catRes = SearchCatByName(catName);
    var messageToShow = catRes.Match(
        Left: err => "Error with status code " + err.ErrorType.ToHttpStatusCode() + " and message " + err.Message,
        Right: catOpt => catOpt.Match(
            Nothing: () => "Cat doesn't exist",
            Just: cat => catName + " has " + cat.Lives + " lives"));

    lblCatLives.Text = messageToShow;
}
```

Just like `OptionStrict`, the `Match` function on `EitherStrict` uses pattern matching, in this way:

```csharp
public TResult Match<TLeft, TRight, TResult>(EitherStrict<TLeft, TRight> either, Func<TRight, TResult> Right, Func<TLeft, TResult> Left)
{
    return either.IsLeft ? Left(either.Left) : Right(either.Right);
}
```

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

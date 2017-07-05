# rest client Sample

Contains a sample application that uses this rest-client library. 

## Architecture

### UruIT.RESTClient.Sample.Server

ASP.NET server (built using NancyFX) that returns various HTTP responses in various endpoints.
Have it run on _localhost_ at the 13788 port.

### UruIT.RESTClient.Sample.Console

Console application that uses RESTClient to call the sample server.

## Running it
* Open the Visual Studio solution.
* Restore the NuGet packages, and add the UruIT.RESTClient package (from nuget.org, or locally if you have already built it).
* Build and Publish the server using the _Debug_ configuration.
* Create an IIS application at port 13788, and access it at _localhost:13788_
* Build de console application
* Run _try_sample.bat_. This script executes the console and prints to stdout the results of various types of calls to the server.
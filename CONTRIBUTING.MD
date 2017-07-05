# How to contribute

Feel free to contribute to this project. If you find a bug or believe there is an interesting feature to introduce into this project, open an issue. If you want to make changes, fork the repository, make your changes, and submit a pull request.

## Good pull request

A pull request has higher chances of being accepted if it fulfills the following:
* Follows the [C# Coding Conventions](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/coding-conventions).
* Passes the Code Analysis static checker using [Microsoft's Managed Recommended Rules](https://msdn.microsoft.com/en-us/library/dd264893.aspx).
* Includes unit tests.
* Updates the documentation (if applicable).

## Development
* To develop new features, you need a Windows machine with .NET Framework v4.0 at minimum.
* To try generating the nuget package locally, open the Nuget Package Console and execute `nuget pack .\src\UruIT.RESTClient\UruIT.RESTClient.nuspec -IncludeReferencedProjects`.
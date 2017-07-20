REM Creates the NuGet package for UruIT.RESTClient

nuget pack .\src\UruIT.RESTClient\UruIT.RESTClient.nuspec -IncludeReferencedProjects -Build -Symbols -Properties Configuration=Release;Platform=AnyCPU

pause
# Installation

There are multiple ways to install and use the mdbooksharp.

## Prerequirements

Heavenly recommended install  [Visual Studio Code](https://code.visualstudio.com/){target=_blank}.

## Pre-compiled binaries

Executable binaries are available for download on the [GitHub Releases page][releases].
Download the binary for your platform (Windows, or Linux) and extract the archive.
The archive contains an `mdbooksharp` executable which you can run to build your books.

[releases]: https://github.com/aneteanetes/mdbooksharp/releases

## Build from source using .NET

To build the `mdbooksharp` executable from source, you will first need to install [.NET 10 SDK](https://dotnet.microsoft.com/en-us/download/dotnet/10.0){target=_blank}.

Once you have installed SDK, the following command can be used to build and install mdBook:

```sh
dotnet build mdbooksharp/MdBookSharp/MdBookSharp.csproj -c Release -o ./outputDirectory
```

## NuGet
You can install mdbooksharp via [NuGet package](https://www.nuget.org/packages/mdbooksharplib).

See about integration in .net on [for developers](../develop/integration.md) chapter.
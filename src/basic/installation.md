# Installation

There are multiple ways to install and use the mdbooksharp.

## Prerequirements

Heavenly recommended use [mdbook sharp template](https://github.com/aneteanetes/mdbooksharp-template){target=_blank} because mdbooksharp **can't'** initialize new book by CLI. Template contains:
* CI script for auto-publish book to [Github Pages](https://pages.github.com/){target=_blank}
* [Lua Syntax Injector](https://marketplace.visualstudio.com/items?itemName=aneteanetes.lua-syntax-injector){target=_blank} for simplify scripting
* Setted up SUMMARY and scripts folder

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

## Continuous integration
If you want use mdbooksharp by automatic deployment, check out the [continuous integration] page for *yaml* file example.

[continuous integration]: ../continuous-integration.md
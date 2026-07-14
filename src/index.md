# Introduction

**mdbooksharp** is a command line tool to create books with Markdown. 

Based on `mdbook` with rewrited backend.

It is good for creating script-based books. If you do not need lua scripts or deep customization, better choose original [mdBook](https://rust-lang.github.io/mdBook/index.html){target=_blank}.

> [!IMPORTANT]
> Thanks to [mdbook](https://github.com/rust-lang/mdBook/){target=_blank}! Without them, this project wouldn't be exists!

* Lightweight [Markdown] files
* Integrated [search] (based on `mdbook`). 
* *Search improvements*
  * `mdbooksharp` support any languages for search include multiple in one book for search.
* Customizing by css files
* [Extensions] provide custom page processing on all generation steps
* [Lua] support for scripting without modifying `mdbookshap`
* [Localization] by *i18n*
* Integrated .NET solution

This guide is an example of what mdbooksharp produces.
mdbooksharp is used by the [Nabunassar TTRPG](https://aneteanetes.github.io/nabtrpg/).

[Markdown]: additional/markdown.html
[search]: basic/reading.html#Search
[Localization]: additional/localization.html
[Lua]: additional/lua.html
[Extensions]: additional/extensions.html

## Contributing

mdbooksharp is free and open source. You can find the source code on
[GitHub](https://github.com/aneteanetes/mdbooksharp) and issues and feature requests can be posted on
the [GitHub issue tracker](https://github.com/aneteanetes/mdbooksharp/issues). You can feel free to send [pull requests](https://github.com/aneteanetes/mdbooksharp/pulls).

## License

**Based on mdBook.**

The mdbooksharp source and documentation are released under
the [Mozilla Public License v2.0](https://www.mozilla.org/MPL/2.0/).
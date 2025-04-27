# MdBookSharp
Not a direct fork of https://github.com/rust-lang/mdBook

## License
All the code in this repository is released under the ***Mozilla Public License v2.0***, for more information take a look at the [LICENSE] file.

## Usage
Run as console with argument 'root of folder with src'. Other is very similar to **[User Guide]** of mdBook.

## Differences
This repository uses mdBook frontend part, but have full rewrited code of backend on C#.
Differences is:
* Added extensions pipeline
* Extensions available only via code-usage, but in-depth have access to parsed html object, pure html text, original markdown content, and all other information about page
* `theme` folder must be inside of `src` folder, and all of it's content copying into `bin` folder directly
* Different rules of chapter numbering
* Searching is available of `ru` and `en` by default, but can be customized via code (for now)
* Have some beautiful bugs
* `settings.json` for extensions configuration and search tweek

## State of mind
Keep in mind, this is solution for personal purpose, but issues and pull-requests of any kind is appreciate. As of code - it must be fully refactor before adding new features or cleaning of mdBook files/solutions.


[LICENSE]: https://github.com/rust-lang/mdBook/blob/master/LICENSE
[User Guide]: https://rust-lang.github.io/mdBook/
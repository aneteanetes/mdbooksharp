# mdbooksharp
Based on https://github.com/rust-lang/mdBook , but with `Lua injections` and `rewrited on c#`.

## License
All the code in this repository is released under the ***Mozilla Public License v2.0***, for more information take a look at the [LICENSE] file.

## Usage
Run as console with argument 'root of folder with src'. Other is very similar to **[User Guide]** of mdBook.

## Example
Live example available: https://aneteanetes.github.io/nabtrpg/
Repository: https://github.com/aneteanetes/nabtrpg 

## Differences
mdbooksharp uses part of `mdBook` frontend files, but have full rewrited code of backend on C#.
* .net backend
* rewrited sidebar
* rewrited search generation

### Summary.md
SUMMARY.md file was reworked:
```md
# Book name
[page without counter](./link.md)
- [counted page](./link.md) // commentary
- [counted static text]
- [counted page level 2](./link.md)
--- //delimiter
+ [collapsible page](./link.md) 
  [page inside collapsible](./link.md){_blank} //you can set target='' for <a> element
  - [counted page inside collabsible](./link.md)
-+ [counted collapsible page](./link.md)
-+ [counted collapsible static text]

```
Symbols:
* `-`: counted
* `+`: collapsible
* `---`: delimiter
* `()`: link

### Lua interop
mdbooksharp has in-build lua execution engine. Lua injections available by `//%` and `//%()` sequences:
* Set **ScriptsFolder** in extensions inside `settings.json`
* All scripts inside folder will be load before generation
* All lua code inside `//%()` and `//%` will be executed for each file

You can use https://github.com/aneteanetes/lua-syntax-injector in vs code for highlightin and autcomplete. 

> //% and //%() was chosen for safety in c-like languages (like JavaScript, TypeScript, etc)
  
### Files changes

* `theme` folder must be inside of `src` folder
* Style and scripts must be inside `theme` folder.

All of `theme`  content copying into `bin` folder directly.

### Searching
Search is available of `ru` and `en` by default. Can be customized via code.

### Settings
mdbooksharp use `settings.json` instead of `*.toml` files:
```json
{
  "IsIncrementalBuild": false,
  "SearchResultOptions": {
    "limit_results":15    
  },
  "Extensions": {
    "DiceExtension": {
      "IconSize": 32
    }
  }
}
```

* **IsIncrementalBuild** - building book without menu re-rendering and copy only changed files
* **SearchResultOptions** - settings for searching for tweaks:
  * limit_results
  * teaser_word_count
* **Extensions** - all extensions have same settings hierarchy, see example below

### Extensions pipeline
Extensions available via code-usage, but in-depth have access to: 
* parsed html object
* pure html text
* original markdown content
* additional information about page and menu

### Build-in extensions

* **ChangelogExtension** - autogenerate changelog page and main page snippet (from *json* file).
* **LuaExtension** - lua execution during book generation.
* **SearchExtension** - mark headings for search engine.
* **WrappedTableExtension** - wrap table with div.
* **ImageTokenExtension** - replace tokens with mapped images (like d4, HP, etc)
* **MDLinkToHtmlExtension** - replace (../file.md) links to (../file.html)
* **NavbarImageExtension** - adds your image logo in top of sidebar
* **FragmentsExtension** - append html from files inside `theme/fragments`

## State of mind
Issues and pull-requests of any kind is appreciate.

## mdbook reference
Original license https://github.com/rust-lang/mdBook/blob/master/LICENSE
Original user guide: https://rust-lang.github.io/mdBook/
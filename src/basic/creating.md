# Creating a book

Once you have the `mdbooksharp`, you can use it to create and generate a book.

## Initializing a book

mdbooksharp support **init** command:


You can init new book by CLI:
```sh
mdbooksharp.exe init {bookdirectory}
``` 
mdbooksharp will create [template](./template.md) in the directory.

## Manually create book from template
See [template](./template.md) page.

## Generation a book
For generation you can run mdbooksharp two ways:
- put `mdbooksharp` near book folder and run without args
- run `mdbooksharp` and pass *path_to_book_folder* as argument 

## Smart creation and generation
You can run `mdbooksharp.exe` without `init` command, and it will init or generate book near executable file.

```sh
/usr/ mdbooksharp
## produce /usr/book
/usr/ mdbooksharp
## generate /usr/book/bin
```

## Anatomy of a book

A book is built from several files which define the settings and layout of the book.

### Settings.json

In the `src` folder of your book, there is a `settings.json` file which contains settings for describing how to build your book.

All supported settings is:

```json
{
  "Language":"en", // default front-end language
  "IsIncrementalBuild": false, //incremental build copy only changed files **without** changes in sidebar. Otherwise folder will be fully recreated.
  "SearchResultOptions": { //lunr.js search options
    "limit_results":30, //default value
    "teaser_word_count":30 //default value
  },
  "Extensions": { //extensions settings
    "LuaExtension":{ //specific extension
      "ScriptsFolder":"lua" //extension settings
    }
  }
}
```

### SUMMARY.md

The next major part of a book is the summary file located at `src/SUMMARY.md`.
This file contains a list of all the chapters in the book.
Before a chapter can be viewed, it must be added to this list.

Here's a example SUMMARY.md file with a basic usages:

```md
# Book name                                   // summary.md support comments

[//%(book.inruductionName)](README.md)        //summary.md support lua injections

- [My First Chapter](my-first-chapter.md)              //counted page
- [Nested example](nested/README.md)
  - [Sub-chapter](nested/sub-chapter.md)               //sub chapter
[Second part]                                      //static text
---                                                               //delimiter
+ [Collapsed example](collapsed.md)                 //collapsed menu with page link
  -+ [Collapsed counted](counted.md)          //collapsed can be unlimited nested
      [Some page](page.md)
[Static text]
+ [Static collapsed]                         //static text can be collapsed
  [Otherside](page.md)

```
> [!IMPORTANT]  
> Levels of menu counts by `2 spaces`!

If any of the chapter files do not exist, `mdbooksharp` will throw error.

### Source files

The content of your book is all contained in the `src` directory.
Each chapter is a separate Markdown file.
Typically, each chapter starts with a level 1 heading with the title of the chapter.

```md
# My First Chapter

Fill out your content here.
```

The precise layout of the files is up to you.
The organization of the files will correspond to the HTML files generated, so keep in mind that the file layout is part of the URL of each chapter.

* All html files in `src` copying to output
* All `images` directory content copying to output
* All `theme` directory content copying to output

All other files not inside `images` and `theme` directories *does not* will be included in the output.

Include images and other files somewhere in `images` or `theme` directories.

> [!WARNING]  
> mdbooksharp does not support `serve` option

## Publishing a book

Once you've written your book, you may want to host it somewhere for others to view.
The first step is to build the output of the book.
This can be done with the run `mdbooksharp` with **path to folder contains `src` directory** argument:

```sh
dotnet mdbooksharp "/usr/mybook/"
```

This will generate a directory named `bin` inside `"/usr/mybook/"` which contains the HTML content of your book.
You can then place this directory on any web server to host it.

> [!TIP]
> Use [mdbooksharp template](https://github.com/aneteanetes/mdbooksharp-template) for instant host your book on Github Pages!
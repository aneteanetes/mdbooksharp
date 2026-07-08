# Markdown

mdbooksharp parse and compile markdown with [Markdig](https://github.com/xoofx/markdig) library. It fully supports [CommonMark](https://commonmark.org/) specification.

> [!TIP]
> For a more in-depth experience, check out the [Markdown Guide](https://www.markdownguide.org).

## Supported syntax

* Text and paragraphs 
* Headings
* Lists
* Links
  * By `MDLinkToHtmlExtension` you can use relative *inline* links to md files
  * Footnotes links support only `*.html`
* Images
* Strikethrough
* Footnotes
  * But not in quotes `>`
* Tables
* Task lists
* Smart punctuation
* Html attributes
  * `{target=_blank}`
* Definition lists
* Admonitions

> *Zoom-in* is not supported

## Markdig

Extensions enabled by default:
* PipeTableExtension
* AlertBlocks
* Abbreviations
* AutoIdentifiers
* Citations
* CustomContainers
* DefinitionLists
* EmphasisExtras
* Figures
* Footers
* Footnotes
* GridTables
* Mathematics
* MediaLinks
* PipeTables
* ListExtras
* TaskLists
* Diagrams
* AutoLinks
* GenericAttributes
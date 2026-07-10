# Reading books

This chapter is very similar to [mdBook origin](https://rust-lang.github.io/mdBook/guide/reading.html) and an introduction on how to interact with a book produced by `mdbooksharp`.
This assumes you are reading an HTML book, because this is the only supported format by mdbooksharp for now.

As original mdbook, books provided by `mdbooksharp` is organized into *chapters*.

## Navigation

> [!IMPORTANT]
> This heading is equals to mdbook and get *as is* from [mdbook documentation](https://rust-lang.github.io/mdBook/guide/reading.html)

There are several methods for navigating through the chapters of a book.

The **sidebar** on the left provides a list of all chapters.
Clicking on any of the chapter titles will load that page.

The sidebar may not automatically appear if the window is too narrow, particularly on mobile displays.
In that situation, the menu icon (three horizontal bars) at the top-left of the page can be pressed to open and close the sidebar.

The **arrow buttons** at the bottom of the page can be used to navigate to the previous or the next chapter.

The **left and right arrow keys** on the keyboard can be used to navigate to the previous or the next chapter.

## Top menu bar

The menu bar at the top of the page provides some icons for interacting with the book, but less than original `mdbook`. 

| Icon | Description |
|------|-------------|
| <i class="fa fa-bars"></i> | Opens and closes the chapter listing sidebar. |
| <i class="fa fa-paint-brush"></i> | Opens a picker to choose a different color theme. |
| <i class="fa fa-search"></i> | Opens a search bar for searching within the book. |

Tapping the menu bar will scroll the page to the top.

## Search

> [!TIP] 
> Unlike the original, mdbooksharp support **any** language to search, includes multiple languages in book for search

Each book has a built-in search system.
Pressing the search icon <i class="fas fa-magnifying-glass"></i> in the menu bar, or push the <kbd>Ctrl+F</kbd> keys on the keyboard will open an input box for entering search terms.
Typing some terms will show matching chapters and sections in real time.

Clicking any of the results will jump to that section.
The up and down arrow keys can be used to navigate the results, and enter will open the highlighted section.

After loading a search result, the matching search terms will be highlighted in the text.
Clicking a highlighted word or pressing the <kbd>Escape</kbd> key will remove the highlighting.
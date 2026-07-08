# Template mdbooksharp 

This section fully describe usage and contents of [mdbooksharp template](https://github.com/aneteanetes/mdbooksharp-template){target=_blank}

## Download template

You can download template two ways:
* Manually from GitHub by click `code->download zip`
* `git clone https://github.com/aneteanetes/mdbooksharp.git`

## Create book from template
* Highly recommended install [Visual Studio Code](https://code.visualstudio.com/){target=_blank}
* Download book
* If you want serve book by GitHub Pages, change `.vscode\tasks.json` file:
  * command:7 line - set mdbooksharp path
  * cwd:12 line - set mdbooksharp work folder
* If you installed Visual Studio Code, it will prompt you to install the [lua-syntax-injector](https://marketplace.visualstudio.com/items?itemName=aneteanetes.lua-syntax-injector) extension.
* Press `F5` to build your book

## Template content
mdbooksharp template contains:
```md
mdbooksharp-template
├── .github/workflows
│   │
│   └── mdbooksharp.yml                       //GitHub pages CI file
│
├── .vscode
│   │
│   ├── build.js                              // empty js file for build task
│   │
│   ├── extensions.json                       // vs code extensions recommendation 
│   │
│   ├── launch.json                           // book generation runner
│   │
│   ├── tasks.json                            // tasks for book generation
│   │
│   └── settings.json                         // empty vs code settings file
├── src
│   │
│   ├── folder                                // Folder for pages
│   │   |
│   │   └── *.md                              // Examples of pages
│   │
│   ├── lua                                   // Directory for lua scripts
│   │   |
│   │   └── book.lua                          // Lua file with book object
│   │
│   ├── index.md                              // First page of the template
│   │
│   ├── settings.json                         // Default settings for book generation
│   │
│   └── SUMMARY.md                            // Template for book summary
│   
├── README.md                                 // default readme file
│   
└── .gitignore                                // default ignore file for book

```
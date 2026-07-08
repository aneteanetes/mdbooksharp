# Localization

mdbooksharp localize only small part of user interface.

## Change locale
You can change locale by `settings.json`:
```json
{
  "Language":"en"
}
```
List of available locales you can see [here](https://github.com/aneteanetes/mdbooksharp/tree/main/MdBookSharp/Resources/locales){target=_blank}

## Add locale
You can add locale by open [pull request](https://github.com/aneteanetes/mdbooksharp/pulls) or add to folder inside project:
```md
book root
...

├── src
│   │
│   ├── locales                                // Folder for locales
│   │   |
        └── locale.json                              // locale file
...
```

Localization based on i18n **files**:
```json
{
  "sidebar": "Toggle Table of Contents",
  "theme": "Change theme",
  "theme_light": "Rust",
  "theme_dark": "Navy",
  "search": "Search",
  "search_book": "Search this book...",
  "print": "Print this book",
  "back": "Previous chapter",
  "next": "Next chapter"
}
```

## Search
Search functions do not require localization. During book generation, a token directory based on UTF-8 characters is compiled, enabling full-text search based on character alphabets rather than language.
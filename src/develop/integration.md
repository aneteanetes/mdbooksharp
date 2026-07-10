# Code integration

You can integrate `mdbooksharp` into any .NET application via [NuGet package](https://www.nuget.org/packages/mdbooksharplib).

## Documentation reference

mdbooksharp have multiple ways to generate book or book parts via code.

### BookWizard

High-level fascade for book init and generation, use it if you do not need any settings up:
```C#
public static class BookWizard
{
    /// <summary>
    /// Generate or init book based on existance of summary.md file
    /// </summary>
    /// <param name="isLog"></param>
    public static void GenerateOrInit(bool isLog = true);

    /// <summary>
    /// Generate book with default extension list
    /// </summary>
    /// <param name="bookPath"></param>
    /// <param name="isLog"></param>
    public static void Generate(string bookPath, bool isLog = true);

    /// <summary>
    /// Generate book with specific extension list (lua automatically included)
    /// </summary>
    /// <param name="bookPath"></param>
    /// <param name="extensions"></param>
    /// <param name="isLog"></param>
    public static void Generate(string bookPath, List<MdBookExtension> extensions, bool isLog=true);

    /// <summary>
    /// Init new book template inside directory path
    /// </summary>
    /// <param name="bookPath"></param>
    /// <param name="isLog"></param>
    public static void Init(string bookPath, bool isLog = true);

    /// <summary>
    /// Check book exists by summary.md file
    /// </summary>
    /// <param name="bookPath"></param>
    /// <returns></returns>
    public static bool IsBookExists(string bookPath);
}
```

### BookLoader

Used for loading `settings.json`, parse `summary.md`, initialize manifest for `incremental builds` and handle `localization` (built-in and in-book):

```C#
public class BookLoader
{
    /// <summary>
    /// Load book into object model
    /// </summary>
    /// <param name="path">path to directory with src folder</param>
    /// <param name="isDebug">if set true, throw exeptions</param>
    /// <param name="lua">lua extension for process summary.md</param>
    /// <param name="extensions">list of extensions for init</param>
    /// <returns>Book object</returns>
    public static Book Load(string path, bool isDebug, LuaExtension lua, List<MdBookExtension> extensions);
}
```

### BookRenderer

Used for render markdown files and run extensions on it:
```C#
public class BookRenderer
{
    /// <summary>
    /// Generate html from md files and process all output pages
    /// </summary>
    /// <param name="book">book for processing pages</param>
    /// <param name="extensions">list of extensions used for processing</param>
    public static void Render(Book book, List<MdBookExtension> extensions);
}
```

### BookBuilder
Used for `build search`, write `pages`, copying book files, process and copy `static html` files, images, and manifest for `incremental builds`: 

```C#
public class BookBuilder
{
    /// <summary>
    /// Save book on disk
    /// </summary>
    /// <param name="book">book to save</param>
    /// <param name="extensions">extensions for process static files</param>
    public static void Build(Book book, List<MdBookExtension> extensions);
}
```

## Usage
You can call `Load`, `Render` and `Build` operations separatly, but attempting to build without rendering will result in an empty html files.
using Geranium.Reflection;
using I18Next.Net;
using Markdig;
using MdBookSharp.Books;
using MdBookSharp.Extensions;
using MdBookSharp.Extensions.LuaScriptExtension;
using MdBookSharp.Resources;
using System.Text.Json;

namespace MdBookSharp
{
    internal class BookLoader
    {
        public static Book Load(string path, bool isDebug, LuaExtension lua, List<MdBookExtension> extensions)
        {
            var rootpath = path;

            path = Path.Combine(path, "src") + Path.DirectorySeparatorChar;

            var summarymd = "SUMMARY.md";
            var summaryPath = Path.Combine(path, summarymd);

            var book = new Book();
            book.ProjectPath = path;
            book.ProjectRootPath = rootpath;
            book.ProjectBinPath = Path.Combine(rootpath,book.Binpath);
            book.DevRootPath = Path.Combine(rootpath, book.Binpath) + "/";
            book.DevRootPath = book.DevRootPath.Replace("\\", "/");

            ConsoleLog.WriteLine("Read settings...");

            var settingsPath = Path.Combine(path, "settings.json");
            if (File.Exists(settingsPath))
            {
                using var stream = File.OpenRead(settingsPath);
                book.Settings = JsonSerializer.Deserialize<Configuration>(stream);
            }

            if (book.Settings.IsIncrementalBuild)
                book.Settings.IsClearFolder = false;

            if (isDebug && book.Settings != null)
                book.Settings.Exceptions = isDebug;

            // extensions load
            ExtensionInitializer.Init(book, extensions);

            ConsoleLog.WriteLine("Parse Summary...");
            SummaryParser.CsParse(book, path, lua);

            ConsoleLog.WriteLine("Read/Write manifest...");
            var resultPath = Path.Combine(book.ProjectRootPath, book.Binpath);
            if (Directory.Exists(resultPath))
            {
                if (book.Settings.IsIncrementalBuild)
                {
                    // read manifest
                    var manifestpath = Path.Combine(resultPath, "navbar.manifest");
                    if (File.Exists(manifestpath))
                    {
                        var manifestwritetime = File.GetLastWriteTime(manifestpath);
                        var summarywritetime = File.GetLastWriteTime(summaryPath);

                        if (summarywritetime < manifestwritetime)
                            book.IsNeedGenerateNavBar = false;

                        using var manifestStream = File.OpenRead(Path.Combine(resultPath, "navbar.manifest"));
                        book.Manifest = JsonSerializer.Deserialize<Dictionary<string, string>>(manifestStream);
                    }
                }
                else if (book.Settings.IsClearFolder)
                {
                    Directory.Delete(resultPath, true);
                }
            }

            var favicon = Directory.GetFiles(path, "favicon*", SearchOption.AllDirectories).LastOrDefault();

            book.IsFaviconSvg = Path.GetExtension(favicon) == ".svg";

            if (!book.IsFaviconSvg)
                book.IsFaviconPng = Path.GetExtension(favicon) == ".png";

            if (favicon.IsNotEmpty())
                book.FaviconExt = Path.GetExtension(favicon);

            ConsoleLog.WriteLine("Process other files...");
            ProcessAdditionalFiles(book);

            book.ExtensionCssFiles = EmbeddedResources.GetEmbeddedFolder("extensions.css");
            for (int i = 0; i < book.ExtensionCssFiles.Count; i++)
            {
                book.ExtensionCssFiles[i].Path = book.ExtensionCssFiles[i].Path.Replace("\\", "/");
            }

            return book;
        }

        private static void ProcessAdditionalFiles(Book book)
        {
            var themePath = Path.Combine(book.ProjectPath, "theme");

            if (!Directory.Exists(themePath))
                return;

            var cssfolder = Path.Combine(themePath, "css");
            if (Directory.Exists(cssfolder))
            {
                foreach (var cssfile in Directory.GetFiles(cssfolder, "*.css", SearchOption.AllDirectories))
                {
                    var path = cssfile.Replace(Path.Combine(book.ProjectPath, "theme"), "");
                    book.AdditionalCssFiles.Add(path.Substring(1).Replace("\\", "/"));
                }
            }

            var jsfolder = Path.Combine(themePath, "js");
            if (Directory.Exists(jsfolder))
            {
                foreach (var jsFile in Directory.GetFiles(jsfolder, "*.js", SearchOption.AllDirectories))
                {
                    var path = jsFile.Replace(Path.Combine(book.ProjectPath, "theme"), "");
                    book.AdditionalJsFiles.Add(path.Replace("\\", "/"));
                }
            }
        }
    }
}

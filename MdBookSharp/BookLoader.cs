using Geranium.Reflection;
using MdBookSharp.Books;
using MdBookSharp.Resources;
using System.Text.Json;

namespace MdBookSharp
{
    internal class BookLoader
    {
        public static Book Load(string path, bool isDebug)
        {
            var rootpath = path;

            path = Path.Combine(path, "src") + "\\";

            var summarymd = "SUMMARY.md";
            var summaryPath = Path.Combine(path, summarymd);

            var book = SummaryParser.CsParse(path);
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
                book.Configuration = JsonSerializer.Deserialize<Configuration>(stream);
            }

            if (book.Configuration.IsIncrementalBuild)
                book.Configuration.IsClearFolder = false;

            if (isDebug && book.Configuration != null)
                book.Configuration.Exceptions = isDebug;

            ConsoleLog.WriteLine("Read/Write manifest...");
            var resultPath = Path.Combine(book.ProjectRootPath, book.Binpath);
            if (Directory.Exists(resultPath))
            {
                if (book.Configuration.IsIncrementalBuild)
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
                else if (book.Configuration.IsClearFolder)
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

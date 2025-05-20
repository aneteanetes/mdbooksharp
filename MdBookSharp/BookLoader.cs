using MdBookSharp.Books;
using MdBookSharp.Resources;
using Newtonsoft.Json;

namespace MdBookSharp
{
    internal class BookLoader
    {
        public static Book Load(string path)
        {
            var rootpath = path;

            path = Path.Combine(path, "src") + "\\";

            var summarymd = "SUMMARY.md";
            var summaryPath = Path.Combine(path, summarymd);

            var book = BookParser.Parse(path);
            book.ProjectPath = path;
            book.ProjectRootPath = rootpath;
            book.ProjectBinPath = Path.Combine(rootpath,book.Binpath);
            book.DevRootPath = Path.Combine(rootpath, book.Binpath) + "/";
            book.DevRootPath = book.DevRootPath.Replace("\\", "/");

            var settingsPath = Path.Combine(path, "settings.json");
            if (File.Exists(settingsPath))
                book.Configuration = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(settingsPath));

            if (book.Configuration.IsIncrementalBuild)
                book.Configuration.IsClearFolder = false;

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

                        book.Manifest = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(Path.Combine(resultPath, "navbar.manifest")));
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

using Geranium.Reflection;
using Geranium.ResourceManager;
using MdBookSharp.Books;
using MdBookSharp.Resources;
using MdBookSharp.Search;
using Newtonsoft.Json;

namespace MdBookSharp
{
    internal class BookBuilder
    {
        /// <summary>
        /// Сохраняет файлы
        /// </summary>
        public static void Build(Book book)
        {
            Console.WriteLine("Writing book...");

            var searchIndexJson = SearchIndexBuilder.BuildIndex(book);

            var firstPage = book.Pages.ElementAtOrDefault(0);
            if (firstPage != null)
            {
                var path = Path.Combine(book.ProjectRootPath, book.Binpath, "index.html");
                var dir = Path.GetDirectoryName(path);

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                File.WriteAllText(path, firstPage.Content);
            }

            foreach (var page in book.Pages.Where(x => x.Path.IsNotEmpty()))
            {
                WritePage(book, page);
            }

            Console.WriteLine($"Copying embedded 'book' content...");
            foreach (var file in EmbeddedResources.GetEmbeddedFolder("book"))
            {
                var path = Path.Combine(book.ProjectRootPath, book.Binpath, file.FileName);
                ValidateDirectory(path);

                File.WriteAllBytes(path, file.Content.ToArray());
            }

            Console.WriteLine($"Copying extensions content...");
            foreach (var file in EmbeddedResources.GetEmbeddedFolder("extensions.css"))
            {
                var path = Path.Combine(book.ProjectRootPath, book.Binpath, file.FileName);
                ValidateDirectory(path);

                File.WriteAllBytes(path, file.Content.ToArray());
            }

            Console.WriteLine($"Writing images...");
            if (book.Configuration.IsIncrementalBuild)
            {
                CopyImageThemeFilesByTracking(book);
            }
            else
            {
                CopyImagesAndThemeNOTIncremental(book);
            }

            Console.WriteLine($"Writing manifest...");

            var summaryfilepath = Path.Combine(book.ProjectPath, "SUMMARY.md");
            ValidateDirectory(summaryfilepath);
            var summarydestpath = Path.Combine(book.ProjectRootPath, book.Binpath, "SUMMARY.md");
            File.Copy(summaryfilepath, summarydestpath, true);

            var binRoot = Path.Combine(book.ProjectRootPath, book.Binpath);

            File.WriteAllText(Path.Combine(binRoot, "navbar.manifest"), JsonConvert.SerializeObject(book.Manifest));


            File.WriteAllText(Path.Combine(binRoot, "searchindex.json"), searchIndexJson);
            File.WriteAllText(Path.Combine(binRoot, "searchindex.js"), @"Object.assign(window.search, " + searchIndexJson + ")");

        }

        private static void CopyImageThemeFilesByTracking(Book book)
        {
            var tracker = new ResourceTracker(new BookTracker(book));
            tracker.Track();
        }

        private static void CopyImagesAndThemeNOTIncremental(Book book)
        {
            Console.WriteLine($"Copying images ...");
            foreach (var image in Directory.GetFiles(Path.Combine(book.ProjectPath, "images"), ".", SearchOption.AllDirectories))
            {
                var path = Path.Combine(book.ProjectRootPath, book.Binpath, image.Replace(book.ProjectPath, ""));
                ValidateDirectory(path);
                File.Copy(image, path, true);

            }

            Console.WriteLine($"Copying theme content ...");
            var root = new DirectoryInfo(book.ProjectPath).FullName;
            var themePath = Path.Combine(book.ProjectPath, "theme");

            if (Directory.Exists(themePath))
            {
                foreach (var file in Directory.GetFiles(themePath, ".", SearchOption.AllDirectories))
                {
                    if (Path.GetExtension(file) == ".md")
                        continue;

                    var path = Path.Combine(book.ProjectRootPath, book.Binpath, file.Replace(root, "").Replace("theme", "").Trim('\\'));
                    ValidateDirectory(path);
                    try
                    {
                        var bytes = File.ReadAllBytes(file);
                        File.WriteAllBytes(path, bytes);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }
                }
            }
        }

        private static string ValidateDirectory(string path)
        {
            var dir = Path.GetDirectoryName(path);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            return path;
        }

        private static void WritePage(Book book,Page page)
        {
            var path = Path.Combine(book.ProjectRootPath, book.Binpath, page.Path.Replace(".md", ".html"));

            ValidateDirectory(path);

            File.WriteAllText(path, page.Content);
        }
    }
}

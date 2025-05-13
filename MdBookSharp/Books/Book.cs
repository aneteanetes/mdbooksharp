using Geranium.Reflection;
using HandlebarsDotNet;
using HtmlAgilityPack;
using Markdig;
using MdBookSharp.Extensions;
using MdBookSharp.MdBook;
using MdBookSharp.Resources;
using MdBookSharp.Search;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace MdBookSharp.Books
{
    internal partial class Book
    {
        public string Title { get; set; }

        public string DefaultTheme { get; set; } = "rust";

        public string PathToRoot { get; set; } = "./";

        public bool IsFaviconSvg { get; set; }

        public bool IsPrintEnable { get; set; }

        public List<string> AdditionalCssFiles { get; set; } = new();

        public List<EmbeddedResource> ExtensionCssFiles { get; set; } = [];

        public List<string> AdditionalJsFiles { get; set; } = new();

        public bool IsFaviconPng { get; set; }

        public string Language { get; set; } = "ru";

        public List<Page> Pages { get; set; } = new();

        public List<Page> PagesHierarchy { get; set; } = new();

        public string RenderedNavbar { get; set; }

        public string ProjectPath { get; set; }

        public string ProjectRootPath { get; set; }

        public string Binpath { get; set; } = "bin";

        private JsonNode Settings { get; set; }

        private Configuration _configuration { get; set; }
        public Configuration Configuration
        {
            get
            {
                if (_configuration == null)
                    _configuration = JsonSerializer.Deserialize<Configuration>(Settings);

                return _configuration;
            }
        }

        public bool IsNeedGenerateNavBar { get; set; } = true;

        public Dictionary<string, string> Manifest = new();

        public static Book Load(string path)
        {
            var rootpath=path;

            path = Path.Combine(path, "src") + "\\";

            var summarymd = "SUMMARY.md";
            var summerypath = Path.Combine(path, summarymd);

            var book = SummaryParser.Parse(summerypath);
            book.ProjectPath = path;
            book.ProjectRootPath = rootpath;

            var settingsPath = Path.Combine(path, "settings.json");
            if (File.Exists(settingsPath))
                book.Settings = JsonObject.Parse(File.ReadAllText(settingsPath));

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
                        var summarywritetime = File.GetLastWriteTime(summerypath);

                        if (summarywritetime < manifestwritetime)
                            book.IsNeedGenerateNavBar = false;

                        book.Manifest = JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(Path.Combine(resultPath, "navbar.manifest")));
                    }
                }
                else if (book.Configuration.IsClearFolder)
                {
                    Directory.Delete(resultPath, true);
                }
            }

            var favicon = Directory.GetFiles(path, "favicon*",SearchOption.AllDirectories).LastOrDefault();
            book.IsFaviconSvg = Path.GetExtension(favicon) == ".svg";
            if (!book.IsFaviconSvg)
                book.IsFaviconPng = Path.GetExtension(favicon) == ".png";

            ProcessAdditionalFiles(book);

            book.ExtensionCssFiles = EmbeddedResources.GetEmbeddedFolder("extensions.css");

            int internalCounter = 1;

            int internalIndex = 0;

            for (int i = 0; i < book.Pages.Count; i++)
            {
                book.BindPage(book.Pages.ElementAtOrDefault(i), path, internalCounter, internalIndex);
                internalIndex++;
            }

            foreach (var page in book.Pages)
            {
                if (page.RelativePath.IsEmpty())
                    continue;

                if (page.RelativePath.IsNotEmpty())
                    page.PathToRoot = Path.GetRelativePath(page.RelativePath.Replace(".md",".html"), "./");

                if (page.PathToRoot.IsNotEmpty() && page.PathToRoot.StartsWith("..\\"))
                    page.PathToRoot = "./" + page.PathToRoot.Substring(3);

                if (page.PathToRoot.IsNotEmpty())
                {
                    page.PathToRoot = page.PathToRoot.Replace("\\", "/");

                    if (page.PathToRoot == "..")
                        page.PathToRoot = "./";
                    else
                        page.PathToRoot += "/";
                }
            }

            return book;
        }

        private static void ProcessAdditionalFiles(Book book)
        {
            var themePath = Path.Combine(book.ProjectPath, "theme");

            if (!Directory.Exists(themePath))
                return;

            foreach (var cssfile in Directory.GetFiles(Path.Combine(themePath,"css"),"*.css", SearchOption.AllDirectories))
            {
                var path = cssfile.Replace(Path.Combine(book.ProjectPath, "theme"), "");
                book.AdditionalCssFiles.Add(path);
            }

            foreach (var jsFile in Directory.GetFiles(Path.Combine(themePath, "js"), "*.js", SearchOption.AllDirectories))
            {
                var path = jsFile.Replace(Path.Combine(book.ProjectPath, "theme"), "");
                book.AdditionalJsFiles.Add(path);
            }
        }

        private void BindPage(Page page, string absPath, int internalCounter, int internalIndex, string parentCount="")
        {
            page.ExtensionCssFiles = this.ExtensionCssFiles.Select(x=>x.FileName).ToArray();

            var prev = Pages.ElementAtOrDefault(internalIndex - 1);
            if (prev != null && prev.Path.IsEmpty())
            {
                int minus = 2;
                while (prev.Path.IsEmpty())
                {
                    prev = Pages.ElementAtOrDefault(internalIndex - minus);
                    minus++;

                    if (minus >= Pages.Count) break;
                }
            }

            var next = Pages.ElementAtOrDefault(internalIndex + 1);
            if (next != null && next.Path.IsEmpty())
            {
                int plus = 2;
                while (next.Path.IsEmpty())
                {
                    next = Pages.ElementAtOrDefault(internalIndex + plus);
                    plus++;

                    if (plus >= Pages.Count) break;
                }
            }

            if (page.Path.IsEmpty())
                return;

            page.Path = page.Path.Replace("./", ProjectPath);

            page.MdContent = File.ReadAllText(page.Path);
            page.RelativePath = page.Path.Replace(absPath, "");

            if (prev != default)
            {
                page.Prev = Path.GetRelativePath(page.RelativePath.Replace(".md", ".html"), prev.RelativePath.Replace(".md", ".html"));

                if (page.Prev.StartsWith("..\\"))
                    page.Prev = "./" + page.Prev.Substring(3);

                    page.Prev = page.Prev.Replace("\\", "/");
            }

            if (next != default)
            {
                page.Next = Path.GetRelativePath(page.RelativePath.Replace(".md", ".html"), next.Path.Replace(absPath,"").Replace(".md", ".html"));

                if (page.Next.StartsWith("..\\"))
                    page.Next = "./" + page.Next.Substring(3);

                page.Next = page.Next.Replace("\\", "/");
            }

            internalIndex++;
        }

        public Book Render(List<MdBookExtension> extensions)
        {
            foreach (var extension in extensions)
            {
                var cfgType = extension.GetSettingsType();
                var node = Settings["extensions"][extension.GetType().Name];
                if(node != null)
                extension.BindSettings(JsonSerializer.Deserialize(node.ToString(), returnType: cfgType));
            }

            var pipelineBuilder = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UsePipeTables();
            var pipeline = pipelineBuilder.Build();

            foreach (var page in Pages)
            {
                if (page.MdContent.IsEmpty())
                    continue;

                Console.WriteLine($"Processing {Pages.IndexOf(page)} page of {Pages.Count}...");

                page.Html = Markdown.ToHtml(page.MdContent, pipeline);

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(page.Html);
                page.HtmlDocument = htmlDoc;

                extensions.ForEach(extension => extension.Process(page));
                page.IsRendered = true;
            }

            return this;
        }

        public void Build()
        {
            RenderPages();

            var searchIndexJson = SearchIndexBuilder.BuildIndex(this);

            var firstPage = Pages.ElementAtOrDefault(0);
            if (firstPage != null)
            {
                var path = Path.Combine(ProjectRootPath, Binpath, "index.html");
                var dir = Path.GetDirectoryName(path);

                if (!Directory.Exists(dir))
                {
                    Directory.CreateDirectory(dir);
                }
                File.WriteAllText(path, firstPage.Content);

                Console.WriteLine($"Writing first page...");
            }

            foreach (var page in Pages.Where(x=>x.Path.IsNotEmpty()))
            {
                WritePage(page);

                Console.WriteLine($"Writing {Pages.IndexOf(page)} page of {Pages.Count}...");
            }

            Console.WriteLine($"Copying embedded 'book' content...");
            foreach (var file in EmbeddedResources.GetEmbeddedFolder("book"))
            {
                var path = Path.Combine(ProjectRootPath, Binpath, file.FileName);
                ValidateDirectory(path);

                File.WriteAllBytes(path, file.Content.ToArray());
            }

            Console.WriteLine($"Copying extensions content...");
            foreach (var file in EmbeddedResources.GetEmbeddedFolder("extensions.css"))
            {
                var path = Path.Combine(ProjectRootPath, Binpath, file.FileName);
                ValidateDirectory(path);

                File.WriteAllBytes(path, file.Content.ToArray());
            }

            Console.WriteLine($"Copying images ...");
            foreach (var image in Directory.GetFiles(Path.Combine(ProjectPath, "images"),".", SearchOption.AllDirectories))
            {
                var path = Path.Combine(ProjectRootPath, Binpath, image.Replace(ProjectPath, ""));
                ValidateDirectory(path);
                File.Copy(image,path,true);

            }

            Console.WriteLine($"Copying theme content ...");
            var root = new DirectoryInfo(ProjectPath).FullName;
            var themePath = Path.Combine(ProjectPath, "theme");

            if (Directory.Exists(themePath))
            {
                foreach (var file in Directory.GetFiles(themePath, ".", SearchOption.AllDirectories))
                {
                    if (Path.GetExtension(file) == ".md")
                        continue;

                    var path = Path.Combine(ProjectRootPath, Binpath, file.Replace(root, "").Replace("theme", "").Trim('\\'));
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

            Console.WriteLine($"Writing manifest...");

            var summaryfilepath = Path.Combine(ProjectPath, "SUMMARY.md");
            ValidateDirectory(summaryfilepath);
            var summarydestpath = Path.Combine(ProjectRootPath, Binpath, "SUMMARY.md");
            File.Copy(summaryfilepath, summarydestpath,true);

            var binRoot = Path.Combine(ProjectRootPath, Binpath);

            File.WriteAllText(Path.Combine(binRoot, "navbar.manifest"), JsonSerializer.Serialize(Manifest));


            File.WriteAllText(Path.Combine(binRoot, "searchindex.json"), searchIndexJson);
            File.WriteAllText(Path.Combine(binRoot, "searchindex.js"), @"Object.assign(window.search, "+searchIndexJson+")");

        }

        private string ValidateDirectory(string path)
        {
            var dir = Path.GetDirectoryName(path);

            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            return path;
        }

        private void WritePage(Page page)
        {
            var path = Path.Combine(ProjectRootPath, Binpath, page.RelativePath.Replace(".md",".html"));

            ValidateDirectory(path);

            File.WriteAllText(path, page.Content);
        }

        private void BuildNavbar(Page renderingPage)
        {
            RenderedNavbar = "";
            int level = 0;

            foreach (var page in Pages)
            {
                if (page.RelativePath == renderingPage.RelativePath)
                    page.IsActive = "active";

                if (page.Name.IsEmpty())
                {
                    RenderedNavbar += EmbeddedResources.GetEmbeddedFileContent("navbar/spacer.html");
                    continue; // ?
                }

                var hbs = string.Empty;

                if (page.Path.IsEmpty())
                    hbs = EmbeddedResources.GetEmbeddedFileContent("navbar/static.hbs");
                else
                    hbs = EmbeddedResources.GetEmbeddedFileContent($"navbar/item.hbs");

                var tpl = Handlebars.Compile(hbs);


                if (page.Level > level)
                {
                    level++;
                    RenderedNavbar += "<li><ol class=\"section\">";
                }

                if (page.Level < level)
                {
                    RenderedNavbar += string.Join("", Enumerable.Range(0, level - page.Level).Select(x => "</ol></li>"));
                    level = page.Level;
                }

                page.Url = page.RelativePath;

                if (!page.RelativePath.IsEmpty() && !renderingPage.RelativePath.IsEmpty() && page != renderingPage)
                    page.Url = Path.GetRelativePath(renderingPage.RelativePath, page.RelativePath);

                if (page.IsActive == "active")
                    page.Url = (page.PathToRoot == "./" ? "./" : "../") + page.Url;

                if (page.Url.IsNotEmpty())
                    page.Url = page.Url.Replace(".md", ".html");

                if (page.Url.IsNotEmpty() && page.Url.StartsWith("..\\"))
                    page.Url = ".\\" + page.Url.Substring(3);

                RenderedNavbar += tpl.Invoke(page);

                page.IsActive = "";

            }

            //foreach (var page in Pages)
            //{
            //    if (page.RelativePath == renderingPage.RelativePath)
            //        page.IsActive = "active";

            //    if (page.Name.IsEmpty())
            //    {
            //        RenderedNavbar += EmbeddedResources.GetEmbeddedFileContent("navbar/spacer.html");
            //        continue; // ?
            //    }

            //    var hbs = string.Empty;

            //    if (page.Path.IsEmpty())
            //        hbs = EmbeddedResources.GetEmbeddedFileContent("navbar/static.hbs");
            //    else
            //        hbs = EmbeddedResources.GetEmbeddedFileContent($"navbar/item.hbs");

            //    var tpl = Handlebars.Compile(hbs);

            //    if (page.Level > level)
            //    {
            //        level++;
            //        RenderedNavbar += "<li><ol class=\"section\">";
            //    }

            //    if (page.Level < level)
            //    {
            //        RenderedNavbar += string.Join("", Enumerable.Range(0, level - page.Level).Select(x => "</ol></li>"));
            //        level = page.Level;
            //    }

            //    page.Url = page.RelativePath;

            //    if (!page.RelativePath.IsEmpty() && !renderingPage.RelativePath.IsEmpty() && page != renderingPage)
            //        page.Url = Path.GetRelativePath(renderingPage.RelativePath, page.RelativePath);

            //    if (page.IsActive == "active")
            //        page.Url = (page.Level==0 ? "./" : "../") + page.Url;

            //    if (page.Url.IsNotEmpty())
            //        page.Url = page.Url.Replace(".md", ".html");

            //    if (page.Url.IsNotEmpty() && page.Url.StartsWith("..\\"))
            //        page.Url = ".\\" + page.Url.Substring(3);

            //    RenderedNavbar += tpl.Invoke(page);

            //    page.IsActive = "";
            //}
        }

        private void RenderPages()
        {
            var template = EmbeddedResources.GetEmbeddedFileContent("page.hbs");
            var bindHtml = Handlebars.Compile(template);

            foreach (var page in Pages)
            {
                if (page.Path.IsEmpty())
                    continue;

                if (IsNeedGenerateNavBar)
                {
                    BuildNavbar(page);
                    Manifest[page.Path] = RenderedNavbar;
                }
                else
                {
                    RenderedNavbar = Manifest[page.Path];
                }

                Console.WriteLine($"Rendering {Pages.IndexOf(page)} page of {Pages.Count}...");

                page.Content = bindHtml.Invoke(new
                {
                    Language,
                    DefaultTheme,
                    Title,
                    page.Name,
                    IsFaviconPng,
                    IsFaviconSvg,
                    page.PathToRoot,
                    AdditionalCssFiles,
                    ExtensionCssFiles,
                    AdditionalJsFiles,
                    RenderedNavbar,
                    IsPrintEnable,
                    page.Html,
                    page.Prev,
                    page.IsPrevExists,
                    page.Next,
                    page.IsNextExists,
                });
            }

        }

    }
}
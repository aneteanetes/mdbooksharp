using Geranium.Reflection;
using HandlebarsDotNet;
using HtmlAgilityPack;
using Markdig;
using MdBookSharp.Books;
using MdBookSharp.Extensions;
using MdBookSharp.Resources;
using Newtonsoft.Json;

namespace MdBookSharp
{
    internal class BookRenderer
    {
        /// <summary>
        /// Превращает md в html и обрабатывает html
        /// </summary>
        /// <param name="extensions"></param>
        /// <returns></returns>
        public static void Render(Book book, List<MdBookExtension> extensions)
        {
            Console.WriteLine("Rendering book...");

            foreach (var extension in extensions)
            {
                var cfgType = extension.GetSettingsType();
                var extName = extension.GetType().Name;
                if (book.Configuration.Extensions.ContainsKey(extName))
                {
                    var node = book.Configuration.Extensions[extName];
                    var settings = JsonConvert.DeserializeObject(node.ToString(), cfgType);
                    extension.BindSettings(settings);
                    book.Configuration.SettingsDictionary.Add(cfgType, settings);
                }
            }

            var pipelineBuilder = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UsePipeTables();
            var pipeline = pipelineBuilder.Build();

            foreach (var page in book.Pages)
            {
                if (page.Path.IsEmpty())
                    continue;

                page.Html = Markdown.ToHtml(page.MdContent, pipeline);

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(page.Html);
                page.HtmlDocument = htmlDoc;

                extensions.ForEach(extension => extension.Process(page));
                page.IsRendered = true;
            }

            RenderPages(book);
        }

        /// <summary>
        /// Рендер страниц в шаблон
        /// </summary>
        private static void RenderPages(Book book)
        {

            var template = EmbeddedResources.GetEmbeddedFileContent("page.hbs");
            var bindHtml = Handlebars.Compile(template);

            Console.WriteLine("Building navbar...");
            var navbarBuilder = new NavbarBuilder(book);

            Console.WriteLine("Rendering pages...");
            foreach (var page in book.Pages)
            {
                if (page.Path.IsEmpty())
                    continue;

                string nav = null;

                if (book.IsNeedGenerateNavBar)
                {
                    nav = navbarBuilder.Build(page);
                    book.Manifest[page.Path] = nav;
                }
                else
                {
                    nav = book.Manifest[page.Path];
                }

                page.Content = bindHtml.Invoke(new
                {
                    RenderedNavbar = nav,
                    page.PathToRoot,
                    page.Name,
                    page.Html,
                    page.IsPrevExists,
                    Prev = NavbarBuilder.GetRelativePath(page, page.Prev),
                    page.IsNextExists,
                    Next = NavbarBuilder.GetRelativePath(page, page.Next),
                    book.Configuration.IsDev,
                    book.DevRootPath,
                    book.Language,
                    book.DefaultTheme,
                    book.Title,
                    book.IsFaviconPng,
                    book.IsFaviconSvg,
                    book.AdditionalCssFiles,
                    book.ExtensionCssFiles,
                    book.AdditionalJsFiles,
                    book.IsPrintEnable,
                });
            }

        }

    }
}

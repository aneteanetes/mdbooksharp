using Geranium.Reflection;
using HandlebarsDotNet;
using HtmlAgilityPack;
using Markdig;
using MdBookSharp.Books;
using MdBookSharp.Extensions;
using MdBookSharp.Resources;
using System.Text.Json;

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
            ConsoleLog.WriteLine("Rendering book...");

            var pipelineBuilder = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UsePipeTables();
            var pipeline = pipelineBuilder.Build();

            foreach (var extension in extensions)
            {
                var cfgType = extension.GetSettingsType();
                var extName = extension.GetType().Name;
                if (book.Configuration.Extensions.ContainsKey(extName))
                {
                    var node = book.Configuration.Extensions[extName];
                    var settings = node.Deserialize(cfgType);// JsonConvert.DeserializeObject(node.ToString(), cfgType);
                    extension.BindSettings(settings);
                    book.Configuration.SettingsDictionary.Add(cfgType, settings);
                }
                extension.Init(book, pipeline);
            }

            ConsoleLog.WriteLine("Extensions initialized...");

            foreach (var page in book.Pages)
            {
                if (page.Path.IsEmpty())
                    continue;

                extensions.Where(x => !x.IsGlobal)
                    .ForEach(extension => {
                        try
                        {
                            page.MdContent = extension.ProcessMd(page, page.MdContent);
                        }
                        catch (Exception)
                        {
                            if (book.Configuration.Exceptions)
                            {
                                throw;
                            }
                            else
                            {
                                ConsoleLog.Error($"{extension.GetType().Name} error.");
                            }
                        }
                    });

                page.Html = Markdown.ToHtml(page.MdContent, pipeline);

                var htmlDoc = new HtmlDocument();
                htmlDoc.LoadHtml(page.Html);
                page.HtmlDocument = htmlDoc;

                extensions.Where(x=>!x.IsGlobal).ForEach(extension => {
                    try
                    {
                        extension.Process(page);
                    }
                    catch (Exception)
                    {
                        if (book.Configuration.Exceptions)
                        {
                            throw;
                        }
                        else
                        {
                            ConsoleLog.Error($"{extension.GetType().Name} error.");
                        }
                    }
                });
                page.IsRendered = true;
            }

            ConsoleLog.WriteLine("Page rendering...");

            RenderPages(book, extensions.Where(x=>x.IsGlobal));
        }

        /// <summary>
        /// Рендер страниц в шаблон
        /// </summary>
        private static void RenderPages(Book book, IEnumerable<MdBookExtension> globalExtensions)
        {
            //CustomRenderer(book, globalExtensions);

            RenderHandleBars(book, globalExtensions);
        }

        private static void RenderHandleBars(Book book, IEnumerable<MdBookExtension> globalExtensions)
        {
            var template = EmbeddedResources.GetEmbeddedFileContent("page.hbs");
            var bindHtml = Handlebars.Compile(template);

            ConsoleLog.WriteLine("Building navbar...");
            var navbarBuilder = new NavbarBuilder(book).Render();

            ConsoleLog.WriteLine("Rendering pages...");
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

                foreach (var globalExt in globalExtensions)
                {
                    try
                    {
                        globalExt.Process(page);
                    }
                    catch
                    {
                        if (book.Configuration.Exceptions)
                        {
                            throw;
                        }
                        else
                        {
                            ConsoleLog.Error($"{globalExt.GetType().Name} error.");
                        }
                    }
                }
            }
        }
    }
}

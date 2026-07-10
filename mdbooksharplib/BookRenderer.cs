using Geranium.Reflection;
using HandlebarsDotNet;
using HtmlAgilityPack;
using I18Next.Net;
using Markdig;
using mdbooksharplib.Books;
using mdbooksharplib.Extensions;
using mdbooksharplib.Resources;
using System.Text.Json;

namespace mdbooksharplib
{
    public class BookRenderer
    {
        /// <summary>
        /// Generate html from md files and process all output pages
        /// </summary>
        /// <param name="book">book for processing pages</param>
        /// <param name="extensions">list of extensions used for processing</param>
        public static void Render(Book book, List<MdBookExtension> extensions)
        {
            ConsoleLog.WriteLine("Rendering book...");

            foreach (var extension in extensions)
            {
                extension.BeforeRender(book);
            }

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
                            if (book.Settings.Exceptions)
                            {
                                throw;
                            }
                            else
                            {
                                ConsoleLog.Error($"{extension.GetType().Name} error.");
                            }
                        }
                    });

                page.Html = Markdown.ToHtml(page.MdContent, book.MarkdownPipeline);

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
                        if (book.Settings.Exceptions)
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

            RenderHandleBars(book, extensions.Where(x=>x.IsGlobal));
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
                    book.Settings.IsDev,
                    book.DevRootPath,
                    book.Settings.Language,
                    book.DefaultTheme,
                    book.Title,
                    book.IsFaviconPng,
                    book.IsFaviconSvg,
                    book.AdditionalCssFiles,
                    book.ExtensionCssFiles,
                    book.AdditionalJsFiles,
                    book.IsPrintEnable,
                    sidebar_l = book.i18n.T("sidebar"),
                    theme_l = book.i18n.T("theme"),
                    theme_light_l = book.i18n.T("theme_light"),
                    theme_dark_l = book.i18n.T("theme_dark"),
                    search_l = book.i18n.T("search"),
                    print_l = book.i18n.T("print"),
                    search_book_l = book.i18n.T("search_book"),
                    back_l = book.i18n.T("back"),
                    next_l = book.i18n.T("next")
                });

                foreach (var globalExt in globalExtensions)
                {
                    try
                    {
                        globalExt.Process(page);
                    }
                    catch
                    {
                        if (book.Settings.Exceptions)
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

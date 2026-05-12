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

        private static void CustomRenderer(Book book, IEnumerable<MdBookExtension> globalExtensions)
        {
            var template = EmbeddedResources.GetEmbeddedFileContent("tpl.hbs");

            ConsoleLog.WriteLine("Building navbar...");
            var navbarBuilder = new NavbarBuilder(book);

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


                var printCss = book.IsPrintEnable
                    ? $"<link rel=\"stylesheet\" href=\"{page.PathToRoot}/css/print.css\" media=\"print\">"
                    : "";
                var printHtml = book.IsPrintEnable
                    ? @$"<a href = ""{page.PathToRoot}print.html"" title = ""Печать"" aria - label = ""Печать"">
                            <i id = ""print-button"" class=""fa fa-print""></i>
                        </a>"
                    : "";
                var printJs = book.IsPrintEnable
                    ? @"<script>
                            window.addEventListener('load', function () {
                                window.setTimeout(window.print, 100);
                            });
                        </script>"
                    : "";

                var favicon = book.FaviconExt.IsNotEmpty()
                    ? $"<link rel=\"icon\" href=\"{page.PathToRoot}/images/favicon.svg\">"
                    : "";

                var additionalCssFiles = book.AdditionalCssFiles.Select(cssFile => $"<link rel=\"stylesheet\" href=\"../{page.PathToRoot}/{cssFile}\">").ToArray();
                var extensionCssFiles = book.ExtensionCssFiles.Select(cssFile => $"<link rel=\"stylesheet\" href=\"../{page.PathToRoot}/{cssFile}\">").ToArray();
                var additionalJsFiles = book.AdditionalJsFiles.Select(jsFile => $"<script src=\"../{page.PathToRoot}/{jsFile}\"></script>").ToArray();

                var mobileNavBar = "";
                var fullNavBar = "";

                if (page.IsPrevExists)
                {
                    mobileNavBar += @$"<a rel=""prev"" href=""{page.Prev}"" class=""mobile-nav-chapters previous"" title=""Назад"" aria-label=""Назад"" aria-keyshortcuts=""Left"">
                                            <i class=""fa fa-angle-left""></i>
                                        </a>";
                    fullNavBar += @$"<a rel=""prev"" href=""{page.Prev}"" class=""nav-chapters previous"" title=""Назад"" aria-label=""Назад"" aria-keyshortcuts=""Left"">
                                        <i class=""fa fa-angle-left""></i>
                                    </a>";
                }

                if (page.IsNextExists)
                {
                    mobileNavBar += @$"<a rel=""next prefetch"" href=""{page.Next}"" class=""mobile-nav-chapters next"" title=""Вперёд"" aria-label=""Вперёд"" aria-keyshortcuts=""Right"">
                                            <i class=""fa fa-angle-right""></i>
                                        </a>";
                    fullNavBar += @$"<a rel=""next prefetch"" href=""{page.Next}"" class=""nav-chapters next"" title=""Вперёд"" aria-label=""Вперёд"" aria-keyshortcuts=""Right"">
                                        <i class=""fa fa-angle-right""></i>
                                    </a>";
                }

                page.Content = template.Replace("{{ Language }}", book.Language)
                    .Replace("{{ DefaultTheme }}", book.DefaultTheme)
                    .Replace("{{ Name }}", page.Name)
                    .Replace("{{ Title }}", book.Title)
                    .Replace("{{ IsPrintableCss }}", printCss)
                    .Replace("{{ IsPrintableHtml }}", printHtml)
                    .Replace("{{ IsPrintableJs }}", printJs)
                    .Replace("{{ AdditionalCssFiles }}", string.Join(Environment.NewLine, additionalCssFiles))
                    .Replace("{{ ExtensionCssFiles }}", string.Join(Environment.NewLine, extensionCssFiles))
                    .Replace("{{ AdditionalJsFiles }}", string.Join(Environment.NewLine, additionalJsFiles))
                    .Replace("{{ IsFavicon }}", favicon)
                    .Replace("{{ RenderedNavbar }}", nav)
                    .Replace("{{ Html }}", page.Html)
                    .Replace("{{ MobileNavBtn }}", mobileNavBar)
                    .Replace("{{ FullNavBtn }}", fullNavBar)
                    .Replace("{{ PathToRoot }}", page.PathToRoot);


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

        private static void RenderHandleBars(Book book, IEnumerable<MdBookExtension> globalExtensions)
        {
            var template = EmbeddedResources.GetEmbeddedFileContent("page.hbs");
            var bindHtml = Handlebars.Compile(template);

            ConsoleLog.WriteLine("Building navbar...");
            var navbarBuilder = new NavbarBuilder(book);

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

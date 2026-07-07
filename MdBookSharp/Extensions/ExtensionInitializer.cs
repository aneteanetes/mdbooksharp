using Markdig;
using MdBookSharp.Books;
using System.Text.Json;

namespace MdBookSharp.Extensions
{
    internal class ExtensionInitializer
    {
        public static void Init(Book book, List<MdBookExtension> extensions)
        {
            var pipelineBuilder = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UsePipeTables();
            var pipeline = pipelineBuilder.Build();

            foreach (var extension in extensions)
            {
                var cfgType = extension.GetSettingsType();
                var extName = extension.GetType().Name;
                if (book.Settings.Extensions.ContainsKey(extName))
                {
                    var node = book.Settings.Extensions[extName];
                    var settings = node.Deserialize(cfgType);
                    extension.BindSettings(settings);
                    book.Settings.SettingsDictionary.Add(cfgType, settings);
                }
                extension.Init(book, pipeline);
            }

            ConsoleLog.WriteLine("Extensions initialized...");
            book.MarkdownPipeline = pipeline;
        }
    }
}

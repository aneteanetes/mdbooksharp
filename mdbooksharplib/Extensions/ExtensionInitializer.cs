using Markdig;
using mdbooksharplib.Books;
using System.Text.Json;
using System.Text.Json.Serialization.Metadata;

namespace mdbooksharplib.Extensions
{
    internal class ExtensionInitializer
    {
        public static void Init(Book book, List<MdBookExtension> extensions)
        {
            var pipelineBuilder = new MarkdownPipelineBuilder()
                .UseAdvancedExtensions()
                .UsePipeTables();
            var pipeline = pipelineBuilder.Build();

            JsonSerializerOptions jsonOpts = new()
            {
                TypeInfoResolver = new DefaultJsonTypeInfoResolver()
            };

            foreach (var extension in extensions)
            {
                var cfgType = extension.GetSettingsType();
                if (cfgType == default)
                    continue;

                var extName = extension.GetType().Name;
                if (book.Settings.Extensions.ContainsKey(extName))
                {
                    var node = book.Settings.Extensions[extName];
                    var settings = node.Deserialize(cfgType, jsonOpts);
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

using I18Next.Net.Backends;
using I18Next.Net.TranslationTrees;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace MdBookSharp.Localization
{
    public class BookJsonBackend : ITranslationBackend
    {
        private readonly string _basePath;

        private readonly ITranslationTreeBuilderFactory _treeBuilderFactory;

        public Encoding Encoding { get; set; } = Encoding.UTF8;

        public BookJsonBackend(string basePath)
            : this(basePath, new GenericTranslationTreeBuilderFactory<HierarchicalTranslationTreeBuilder>())
        {
        }

        public BookJsonBackend(string basePath, ITranslationTreeBuilderFactory treeBuilderFactory)
        {
            _basePath = basePath;
            _treeBuilderFactory = treeBuilderFactory;
        }

        public BookJsonBackend(ITranslationTreeBuilderFactory treeBuilderFactory)
            : this("locales", treeBuilderFactory)
        {
        }

        public BookJsonBackend()
            : this("locales")
        {
        }

        public async Task<ITranslationTree> LoadNamespaceAsync(string language, string @namespace)
        {
            string path = FindFile(language, @namespace);
            if (path == null)
            {
                return null;
            }

            JObject parsedJson;
            using (StreamReader streamReader = new StreamReader(path, Encoding))
            {
                using JsonTextReader reader = new JsonTextReader(streamReader);
                parsedJson = (JObject)(await JToken.ReadFromAsync(reader));
            }

            ITranslationTreeBuilder builder = _treeBuilderFactory.Create();
            PopulateTreeBuilder("", parsedJson, builder);
            return builder.Build();
        }

        private string FindFile(string language, string @namespace)
        {
            string text = Path.Combine(_basePath, language + ".json");
            if (File.Exists(text))
            {
                return text;
            }

            text = Path.Combine(_basePath, BackendUtilities.GetLanguagePart(language) + ".json");
            return (!File.Exists(text)) ? null : text;
        }

        private static void PopulateTreeBuilder(string path, JObject node, ITranslationTreeBuilder builder)
        {
            if (path != string.Empty)
            {
                path += ".";
            }

            foreach (KeyValuePair<string, JToken> item in node)
            {
                string text = path + item.Key;
                if (item.Value is JObject node2)
                {
                    PopulateTreeBuilder(text, node2, builder);
                }
                else if (item.Value is JValue jValue)
                {
                    builder.AddTranslation(text, jValue.Value.ToString());
                }
            }
        }
    }
}

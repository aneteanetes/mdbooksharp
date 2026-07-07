using I18Next.Net.Backends;
using I18Next.Net.TranslationTrees;
using MdBookSharp.Resources;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;

namespace MdBookSharp.Localization
{

    public class EmbeddedJsonFileBackend : ITranslationBackend
    {
        private readonly string _basePath;

        private readonly ITranslationTreeBuilderFactory _treeBuilderFactory;

        public Encoding Encoding { get; set; } = Encoding.UTF8;

        public EmbeddedJsonFileBackend()
        {
            _treeBuilderFactory = new GenericTranslationTreeBuilderFactory<HierarchicalTranslationTreeBuilder>();
        }

        public async Task<ITranslationTree> LoadNamespaceAsync(string language, string @namespace)
        {
            var file = FindFile(language, @namespace);
            if (file == null)
            {
                return null;
            }

            JObject parsedJson;
            using (StreamReader streamReader = new StreamReader(file.Content, Encoding))
            {
                using JsonTextReader reader = new JsonTextReader(streamReader);
                parsedJson = (JObject)(await JToken.ReadFromAsync(reader));
            }

            ITranslationTreeBuilder builder = _treeBuilderFactory.Create();
            PopulateTreeBuilder("", parsedJson, builder);
            return builder.Build();
        }

        private EmbeddedResource FindFile(string language, string @namespace)
        {
            var files = EmbeddedResources.GetEmbeddedFolder("locales");
            return files.FirstOrDefault(f => f.FileName == $"{BackendUtilities.GetLanguagePart(language)}.json");
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

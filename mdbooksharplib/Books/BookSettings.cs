using Geranium.Reflection;
using mdbooksharplib.Search;
using System.Text.Json.Nodes;

namespace mdbooksharplib.Books
{
    public class BookSettings
    {
        public string Language { get; set; } = "ru";

        public bool Exceptions { get; set; } = false;

        public bool IsClearFolder { get; set; } = true;

        public bool IsIncrementalBuild { get; set; } = false;

        public SearchOptions SearchOptions { get; set; }

        public ResultOptions SearchResultOptions { get; set; }

        public Dictionary<string, JsonNode> Extensions { get; set; } = new();

        public bool IsDev { get; set; }

        internal Dictionary<Type, object> SettingsDictionary = new();

        public T GetExtensionSettings<T>()
        {
            if (SettingsDictionary.TryGetValue(typeof(T), out var settings))
                return settings.As<T>();

            return default;
        }
    }
}

using Geranium.Reflection;
using MdBookSharp.Search;
using Newtonsoft.Json.Linq;

namespace MdBookSharp.Books
{
    internal class Configuration
    {
        public bool IsClearFolder { get; set; } = true;

        public bool IsIncrementalBuild { get; set; } = false;

        public SearchOptions SearchOptions { get; set; }

        public ResultOptions SearchResultOptions { get; set; }

        public Dictionary<string,JObject> Extensions { get; set; }

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

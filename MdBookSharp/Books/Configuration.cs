using MdBookSharp.Search;

namespace MdBookSharp.Books
{
    internal class Configuration
    {
        public bool IsClearFolder { get; set; } = true;

        public bool IsIncrementalBuild { get; set; } = false;

        public SearchOptions SearchOptions { get; set; }

        public ResultOptions SearchResultOptions { get; set; }
    }
}

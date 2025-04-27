namespace MdBookSharp.Search
{
    internal class SearchIndex
    {
        public List<string> doc_urls { get; set; } = new();

        public List<SearchDocument> docs { get; set; } = new();

        public ResultOptions results_options { get; set; } = new();

        public SearchOptions search_options { get; set; } = new();

    }
    internal class SearchDocument
    {
        public string body { get; set; }

        public string breadcrumbs { get; set; }

        public string id { get; set; }

        public string title { get; set; }
    }

    internal class ResultOptions
    {
        public int limit_results { get; set; } = 30;

        public int teaser_word_count { get; set; } = 30;
    }

    internal class SearchOptions
    {
        public string @bool { get; set; } = "OR";

        public bool expand { get; set; } = true;

        public SearchFields fields { get; set; } = new();

    }

    internal class SearchFields
    {
        public SearchField title { get; set; } = new SearchField() { boost = 2 };

        public SearchField body { get; set; } = new SearchField() { boost = 1 };

        public SearchField breadcrumbs { get; set; } = new SearchField() { boost = 1 };
    }

    internal class SearchField
    {
        public int boost { get; set; } = 0;
    }
}
using Geranium.Reflection;
using MdBookSharp.Books;
using System.Diagnostics;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace MdBookSharp.Search
{
    public class SearchIndexBuilder
    {
        internal static string BuildIndex(Book book)
        {
            var idx = new SearchIndex();

            var bookCfg = book.Configuration;
            if (bookCfg != null)
            {
                if (bookCfg.SearchOptions != null)
                {
                    idx.search_options = bookCfg.SearchOptions;
                }
                if (bookCfg.SearchResultOptions != null)
                {
                    idx.results_options = bookCfg.SearchResultOptions;
                }
            }

            int _id = 0;

            foreach (var page in book.Pages)
            {
                if (page.HtmlDocument == null)
                    continue;

                var nodes = page.HtmlDocument.DocumentNode.ChildNodes.ToArray();

                SearchDocument prevSearchDoc = null;

                for (int i = 0; i < nodes.Length; i++)
                {
                    var node = nodes[i];
                    if (node.Name.StartsWith("h") && node.Name.Length==2)
                    {
                        if (prevSearchDoc != null && prevSearchDoc.body.IsEmpty())
                            prevSearchDoc.body = prevSearchDoc.title;

                        var title = node.InnerText;

                        idx.docs.Add(prevSearchDoc = new SearchDocument
                        {
                            id = $"{_id++}",
                            title = title,
                            breadcrumbs = GenerateBreadcrumb(book,page,title)
                        });

                        idx.doc_urls.Add($"{page.RelativePath.Replace(".md",".html")}#{title}".Replace(" ","-"));
                    }
                    else
                    {
                        prevSearchDoc.body += Regex.Replace(node.InnerText, @"\s+", " ");
                    }
                }
            }

            return JsonSerializer.Serialize(idx, new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            });
        }

        private static string GenerateBreadcrumb(Book book,Page page, string title)
        {
            if (page.Parent == null)
                return $"{book.Pages[0].Name} » {title}";
            
            return ApplyParentTitleRecursive(page,title);
        }

        private static string ApplyParentTitleRecursive(Page page, string prev)
        {
            if (page.Parent != null)
            {
                prev = page.Parent.Name + " » " + prev;
                return ApplyParentTitleRecursive(page.Parent, prev);
            }

            return prev;
        }
    }
}
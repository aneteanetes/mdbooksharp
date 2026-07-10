using mdbooksharplib.Books;
using System.Diagnostics;

namespace mdbooksharplib.Extensions
{
    public class MDLinkToHtmlExtension : MdBookExtension<MDLinkToHtmlExtensionSettings>
    {
        public override void Process(Page page)
        {
            if (page.Html.Contains(".md)") || page.Html.Contains(".md\""))
            {
                page.Html = page.Html.Replace(".md)", ".html)")
                    .Replace(".md\"", ".html\"");
            }
        }
    }
}

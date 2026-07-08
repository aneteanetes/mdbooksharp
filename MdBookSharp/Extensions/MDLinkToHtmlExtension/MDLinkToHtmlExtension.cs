using MdBookSharp.Books;
using System.Diagnostics;

namespace MdBookSharp.Extensions.MDLinkToHtmlExtension
{
    internal class MDLinkToHtmlExtension : MdBookExtension<MDLinkToHtmlExtensionSettings>
    {
        public override void Process(Page page)
        {
            if (page.Html.Contains(".md)"))
            {
                page.Html = page.Html.Replace(".md)", ".html)");
            }
        }
    }
}

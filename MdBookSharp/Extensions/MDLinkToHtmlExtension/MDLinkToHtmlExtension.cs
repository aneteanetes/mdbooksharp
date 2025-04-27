using MdBookSharp.Books;
using System.Diagnostics;

namespace MdBookSharp.Extensions.MDLinkToHtmlExtension
{
    internal class MDLinkToHtmlExtension : MdBookExtension<MDLinkToHtmlExtensionSettings>
    {
        public override void Process(Page file)
        {
            if (file.Html.Contains(".md\">"))
            {
                file.Html = file.Html.Replace(".md\">", ".html\">");
            }
        }
    }
}

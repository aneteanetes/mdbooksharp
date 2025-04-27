using MdBookSharp.Books;

namespace MdBookSharp.Extensions.Searching
{
    internal class SearchExtension : MdBookExtension<SearchSettings>
    {
        public override void Process(Page file)
        {
            if (!file.Html.Contains("<h"))
                return;

            file.HtmlDocument.DocumentNode.Descendants().Where(x => x.Name.StartsWith("h") && x.Name.Length == 2)
                .ForEach(x =>
                {
                    var linkValue = x.InnerText.Replace(" ", "-");

                    var a= file.HtmlDocument.CreateElement("a");
                    a.AddClass("header");
                    a.SetAttributeValue("href", "#" + linkValue);

                    x.InnerHtml = "<a class=\"header\" href=\"#" + linkValue + "\">" + x.InnerHtml + "</a>";
                    x.SetAttributeValue("id", linkValue);
                });

            file.Html = file.HtmlDocument.DocumentNode.InnerHtml;
        }
    }
}

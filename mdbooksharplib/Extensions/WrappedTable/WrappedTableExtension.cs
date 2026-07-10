using mdbooksharplib.Books;

namespace mdbooksharplib.Extensions
{
    public class WrappedTableExtension : MdBookExtension<WrappedTableSettings>
    {
        public override void Process(Page file)
        {
            if (file.MdContent.Contains(".wrappedtable"))
            {
                foreach (var wrapTable in file.HtmlDocument.DocumentNode.Descendants("table").Where(x=>x.HasClass("wrappedtable")).ToArray())
                {
                    var styles = wrapTable.GetAttributeValue("class","").Replace("wrappedtable","");
                    wrapTable.SetAttributeValue("class","");

                    var div = file.HtmlDocument.CreateElement("div");
                    div.SetAttributeValue("class", styles);

                    wrapTable.ParentNode.InsertBefore(div, wrapTable);                    
                    div.AppendChild(wrapTable.CloneNode(true));
                    wrapTable.ParentNode.RemoveChild(wrapTable);
                }

                file.Html = file.HtmlDocument.DocumentNode.InnerHtml;
            }
        }
    }
}

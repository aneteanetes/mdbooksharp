using mdbooksharplib.Books;

namespace mdbooksharplib.Extensions
{
    public class NabPlateExtension : MdBookExtension<NabPlateExtensionConfig>
    {
        public override void Process(Page file)
        {
            if (file.MdContent.Contains("</nabplate>"))
            {
                foreach (var plate in file.HtmlDocument.DocumentNode.Descendants("nabplate").ToArray())
                {
                    var name = plate.GetAttributeValue("name", "");
                    var icon = plate.GetAttributeValue("icon", "");
                    var desc = plate.GetAttributeValue("desc", "");

                    if (name == null)
                        continue;

                    var div = file.HtmlDocument.CreateElement("div");
                    div.SetAttributeValue("style", "background: #1a1612;margin-bottom: 10px;");
                    div.InnerHtml = @$"
<div style=""background: rgba(0, 0, 0, 0.4); padding: 20px; border: 1px double rgb(61, 52, 40);"">
    <div class=""flex justify-between"">
        <h3 style=""color: rgb(255, 255, 255); margin-top: 0px; text-transform: uppercase;"">
            <i class=""ra {icon}"" style=""margin-right: 4px""></i>{name}
        </h3>
    </div>
    <p class=""mt-1"" style=""color: rgb(209, 184, 148); line-height: 1.6; font-style: italic;"">{desc}</p>
</div>";

                    plate.ParentNode.ReplaceChild(div, plate);
                }

                file.Html = file.HtmlDocument.DocumentNode.InnerHtml;
            }
        }
    }
}

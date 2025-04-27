using HtmlAgilityPack;
using MdBookSharp.Books;
using MdBookSharp.Extensions.Dices;

namespace MdBookSharp.Extensions.WoWPlates
{
    internal class WoWPlateExtension : MdBookExtension<WowPlateExtensionConfig>
    {
        public override void Process(Page file)
        { 
            //not inversed for 'faster' check
            if (file.MdContent.Contains("</plate>"))
            {
                foreach (var plate in file.HtmlDocument.DocumentNode.Descendants("plate").ToArray())
                {
                    var img = plate.GetAttributeValue("img","");
                    var name = plate.GetAttributeValue("name", "");
                    var subtype = plate.GetAttributeValue("subtype", "");
                    var html = plate.InnerHtml;

                    var div = file.HtmlDocument.CreateElement("div");
                    div.AddClass("trait");
                    div.InnerHtml = PanelInnerHtml(img, subtype, name, html);

                    plate.ParentNode.ReplaceChild(div, plate);
                }

                file.Html = file.HtmlDocument.DocumentNode.InnerHtml;
            }
        }

        private string PanelInnerHtml(string img, string subtype, string name, string html) => @$"<div class=""trait"">
    <div class=""wowhead-tooltip"">
      <table class=""wrap-table"">
          <tbody>
            <tr>
                <td class=""wow-tp-td-content"">
                  <table class=""wow-tp-content-title-table"" style=""width: 100%;"">
                      <tbody>
                        <tr>
                            <td>
    <div class=""wow-icon"" style=""float:left"">
      <div class=""iconlarge"" data-env=""live"" data-tree=""live"" data-game=""wow"">    
        <ins style=""background-image: url('../images/icons/{img}');"" class="""">
        </ins>  
        <del>  
        </del>
      </div>
    </div>
                              <span class=""whtt-name"">
<h6 style=""display: inline-block;margin: 0;"">
<b class=""whtt-name"">{name}</b>
</h6>
                              <div class=""subtype"">{subtype}</div>
                            </td>
                        </tr>
                      </tbody>
                  </table>
                  <table class=""wow-tp-content-description-table"">
                      <tbody>
                        <tr>
                            <td>
                              <div class=""text"">
                                {html} <br>
                              </div>
                            </td>
                        </tr>
                      </tbody>
                  </table>
                </td>
                <th class=""wow-tp-th-content""></th>
            </tr>
            <tr class=""wow-tp-bottom-tr"">
                <th class=""wow-tp-bottom"" style=""background-position: bottom left""></th>
                <th class=""wow-tp-bottom-corner"" style=""background-position: bottom right""></th>
            </tr>
          </tbody>
      </table>
    </div>
  </div>";
    }
}

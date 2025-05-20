using MdBookSharp.Books;

namespace MdBookSharp.Extensions.WowIcons
{
    internal class WoWIconExtension : MdBookExtension<WoWIconExtensionSettings>
    {
        public override void Process(Page file)
        {
            if (file.MdContent.Contains("<icon>"))
            {
                foreach (var icon in file.HtmlDocument.DocumentNode.Descendants("icon").ToArray())
                {
                    var div = file.HtmlDocument.CreateElement("div");
                    div.AddClass("wow-icon");
                    div.InnerHtml = IconTemplate(icon.InnerText,file);

                    icon.ParentNode.ReplaceChild(div, icon);
                }

                file.Html = file.HtmlDocument.DocumentNode.InnerHtml;
            }
        }

        private string IconTemplate(string img,Page page) => @$"
      <div class=""iconlarge"" data-env=""live"" data-tree=""live"" data-game=""wow"">    
        <ins style=""background-image: url('{page.PathToRoot}/images/icons/{img}');"" class="""">
        </ins>  
        <del>  
        </del>
      </div>";
    }
}

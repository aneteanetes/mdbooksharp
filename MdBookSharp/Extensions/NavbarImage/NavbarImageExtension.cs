using Geranium.Reflection;
using HtmlAgilityPack;
using MdBookSharp.Books;

namespace MdBookSharp.Extensions.NavbarImage
{
    internal class NavbarImageExtension : MdBookExtension<FragmentsExtensionConfig>
    {
        public override bool IsGlobal => true;

        public override void Process(Page file)
        {
            if (this.Settings.ImageAbsPath.IsEmpty())
                return;

            var htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(file.Content);

            var navbar = htmlDoc.DocumentNode.Descendants().FirstOrDefault(x => x.HasClass("sidebar-scrollbox"));
            if (navbar != default)
            {
                var img = htmlDoc.CreateElement("img");
                img.SetAttributeValue("src", file.PathToRoot + this.Settings.ImageAbsPath);
                img.SetAttributeValue("style", "width:100%");
                navbar.PrependChild(img);
            }

            file.Content = htmlDoc.DocumentNode.InnerHtml;
        }
    }
}

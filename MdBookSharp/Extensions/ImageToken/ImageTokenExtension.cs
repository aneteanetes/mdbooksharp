using MdBookSharp.Books;

namespace MdBookSharp.Extensions.ImageToken
{
    internal class ImageTokenExtension : MdBookExtension<ImageTokenExtensionSettings>
    {
        public override void Process(Page file)
        {
            if (file.MdContent.Contains(Settings.TokenMappings.Select(x => x.Key).ToArray()))
            {
                Settings.TokenMappings.ForEach(x =>
                {
                    var renderedimg = Token(file,x.Key, x.Value);
                    file.Html = file.Html.Replace(x.Key, renderedimg);
                });
            }
        }

        private string Token(Page page, string tokenName, string img)
        {
            var size = Settings.IconSize;
            return $@"<img width=""{size}"" height=""{size}"" class=""dice"" src=""{page.PathToRoot}images/stats/{img}.png"" alt=""{tokenName}"">";
        }
    }
}

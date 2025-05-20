using Geranium.Reflection;
using MdBookSharp.Books;

namespace MdBookSharp.Extensions.ImageToken
{
    internal class ImageTokenExtension : MdBookExtension<ImageTokenExtensionSettings>
    {
        public override void Process(Page file)
        {
            if (file.MdContent.ContainsAny(Settings.Tokens.ToArray()))
            {
                Settings.Tokens.ForEach(token =>
                {
                    var imgName = token;
                    if(Settings.TokenMappings.ContainsKey(token))
                        imgName = Settings.TokenMappings[token];

                    var renderedimg = Token(Settings.IconSize, file, token, imgName);
                    file.Html = file.Html.Replace(token, renderedimg);
                });

                Settings.TokenMappings.ForEach(x =>
                {
                    var renderedimg = Token(Settings.IconSize, file,x.Key, x.Value);
                    file.Html = file.Html.Replace(x.Key, renderedimg);
                });
            }
        }

        public static string Token(int size, Page page, string tokenName, string img)
        {
            if (img.IsEmpty())
                return "";

            return $@"<img width='{size}' height='{size}' class='dice' src='{page.PathToRoot}/images/tokens/{img}.png' alt=''>";
        }
    }
}

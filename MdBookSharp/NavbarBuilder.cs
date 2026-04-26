using Geranium.Reflection;
using MdBookSharp.Books;

namespace MdBookSharp
{
    internal class NavbarBuilder
    {
        public Book Book { get; }

        public string NavBarTemplate { get; private set; }

        public NavbarBuilder(Book book)
        {
            Book = book;
            NavbarRender();
        }

        private void NavbarRender()
        {
            int level = 0;

            foreach (var page in Book.Pages)
            {
                if (page.Name.IsEmpty())
                {
                    NavBarTemplate += "<li class=\"spacer\"></li>";
                    continue;
                }

                var li = string.Empty;

                if (page.Path.IsEmpty())
                    li = $"<li class=\"chapter-item expanded affix \">\r\n<li class=\"part-title\">{(page.IsCounted ? page.Number+"." : "")}{page.Name}</li>";
                else
                    li = NavbarItemTemplate(page);

                if (page.Level > level)
                {
                    level++;
                    NavBarTemplate += "<li><ol class=\"section\">";
                }

                if (page.Level < level)
                {
                    NavBarTemplate += string.Join("", Enumerable.Range(0, level - page.Level).Select(x => "</ol></li>"));
                    level = page.Level;
                }

                NavBarTemplate += li;
            }
        }

        private string NavbarItemTemplate(Page page) => $@"<li class=""chapter-item expanded"">
    <a href=""{{{page.Id}Url}}"">"
            + (page.IsCounted ? $@"<strong aria-hidden=""true"">{page.Number}.</strong>" : "")
            + page.Name + @"
    </a>
</li>";

        public string Build(Page renderingPage)
        {
            var result = NavBarTemplate.Replace("{" + renderingPage.Id + "Url}", @""" class=""active""");

            foreach (var page in Book.Pages)
            {
                if (page == renderingPage || page.Name.IsEmpty() || page.Path.IsEmpty())
                    continue;

                var url = GetRelativePath(renderingPage, page);

                result = result.Replace("{" + page.Id + "Url}", url);
            }

            return result;
        }

        public static string GetRelativePath(Page relativeToPage, Page pagePath)
        {
            if (relativeToPage == null || pagePath == null || relativeToPage == pagePath || relativeToPage.Path == null || pagePath.Path == null)
                return null;

            var pathRel = Path.GetRelativePath(relativeToPage.Path_Html, pagePath.Path_Html)
                    .Replace("\\", "/");

            if(pathRel.Length>1)
                    return pathRel.Substring(3);

            if (pathRel == ".")
                return "./index.html";

            return pathRel;
        }
    }
}